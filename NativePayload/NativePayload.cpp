// NativePayload.cpp : Defines the entry point for the application.
//

#include <Windows.h>

#include "mscoree.h"
#pragma comment (lib, "user32.lib")

typedef HRESULT(STDAPICALLTYPE* FnGetCLRRuntimeHost)(REFIID riid, IUnknown** pUnk);

void ShowError(const char* message)
{
	MessageBoxA(NULL, message, "Error", MB_OK);
}

BOOL WINAPI Attach() 
{
	HMODULE CoreCLRModule;
	CoreCLRModule = GetModuleHandleA("coreclr.dll");
	if (CoreCLRModule == NULL) {
		ShowError("coreclr.dll not found");
		return FALSE;
	}
	ICLRRuntimeHost* RuntimeHost;
	FnGetCLRRuntimeHost pfnGetCLRRuntimeHost = (FnGetCLRRuntimeHost)::GetProcAddress(CoreCLRModule, "GetCLRRuntimeHost");

	if (!pfnGetCLRRuntimeHost)
	{
		ShowError("GetCLRRuntimeHost not found");
		return FALSE;
	}

	HRESULT hr = pfnGetCLRRuntimeHost(IID_ICLRRuntimeHost, (IUnknown**)&RuntimeHost);
	if (FAILED(hr))
	{
		ShowError("GetCLRRuntimeHost failed");
		return FALSE;
	}

	hr = RuntimeHost->Start();
	if (FAILED(hr))
	{
		ShowError("Runtime failed to start");
		return FALSE;
	}

	DWORD ExitCode = -1;
	hr = RuntimeHost->ExecuteInDefaultAppDomain(L"ManagedPayload.dll", L"ManagedPayload.Program", L"PayloadMain", L"", &ExitCode);
	if (FAILED(hr))
	{
		ShowError("Failed execute to load ManagedPayload.Program.Main, hr: " + hr);
		return FALSE;
	}

	return TRUE;
}

BOOL APIENTRY DllMain(HINSTANCE hInstDLL, DWORD nReason, LPVOID lpReserved)
{
	switch (nReason) {
	case DLL_PROCESS_ATTACH:
		CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)Attach, NULL, 0, NULL);
		break;
	case DLL_PROCESS_DETACH:
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	}

	return TRUE;
}