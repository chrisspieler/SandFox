// Injector.cpp : Defines the entry point for the application.
//

#include "Injector.h"
#include <Windows.h>

using namespace std;

int main( int argc, char* argv[] )
{
	if (argv[1] == NULL)
	{
		printf("PID not specified, exiting.\n");
		return -1;
	}
	printf("PID: %i\n", atoi(argv[1]));
	DWORD dwPID = DWORD(atoi(argv[1]));
	if (argv[2] == NULL) {
		printf("DLL path not specified, exiting.\n");
		return -1;
	}
	char* filepath = argv[2];
	GetFileAttributes(filepath);
	if (INVALID_FILE_ATTRIBUTES == GetFileAttributes(filepath) && GetLastError() == ERROR_FILE_NOT_FOUND)
	{
		cout << "File not found: " << filepath << endl;
		return -1;
	}
	// Please don't smash my stackussy. This is all just for fun.
	unsigned int filepathLen = strlen(filepath) + 1;
	printf("DLL path: %s\n", filepath);

	HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, dwPID);
	if ( hProcess == NULL )
	{
		cout << "Failed to open process: " << dwPID << endl;
		return -1;
	}
	LPVOID lpRemoteBuffer = VirtualAllocEx(hProcess, NULL, filepathLen, (MEM_RESERVE | MEM_COMMIT), PAGE_EXECUTE_READWRITE);
	if (lpRemoteBuffer == NULL)
	{
		cout << "Failed to allocate memory in remote process" << endl;
		return -1;
	}
	cout << "Allocated memory in remote process at 0x" << lpRemoteBuffer << endl;
	SIZE_T bytesWritten;
	// Copy the DLL file path to the remote process.
	BOOL result = WriteProcessMemory(hProcess, lpRemoteBuffer, filepath, filepathLen, &bytesWritten);
	if (!result) 
	{
		cout << "Failed to write to remote process." << endl;
		return -1;
	}
	cout << "Wrote " << bytesWritten << " bytes to remote process" << endl;
	// Supposedly, different processes all use the same memory addresses for kernel32 functions such as LoadLibraryA.
	// Here, we tell the remote process to call LoadLibraryA and use the DLL file path we wrote earlier as an argument.
	HANDLE hRemoteThread = CreateRemoteThread(hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)LoadLibraryA, lpRemoteBuffer, 0, NULL);
	if (hRemoteThread == NULL)
	{
		cout << "Failed to create remote thread" << endl;
		return -1;
	}
	cout << "Started LoadLibrary thread " << hRemoteThread << " in remote process." << endl;
	CloseHandle( hProcess );
}
