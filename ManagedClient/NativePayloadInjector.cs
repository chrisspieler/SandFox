using System.Diagnostics;

namespace ManagedClient
{
    internal class NativePayloadInjector
    {
        public const string INJECTOR_APP_NAME = "Injector.exe";
        public const string NATIVE_PAYLOAD_NAME = "NativePayload.dll";
        public const string MANAGED_PAYLOAD_FOLDER = "ManagedPayload";

        public NativePayloadInjector( string processName )
        {
            ProcessName = processName;
        }

        public string ProcessName { get; init; }
        public Process? ActiveProcess { get; private set; }

        public void Inject()
        {
            if ( ActiveProcess != null && !ActiveProcess.HasExited )
            {
                return;
            }

            var injectorPath = GetLocalPath(INJECTOR_APP_NAME);
            var nativePayloadPath = GetLocalPath(NATIVE_PAYLOAD_NAME);
            var managedPayloadFolderPath = GetLocalPath(MANAGED_PAYLOAD_FOLDER);

            EnsureManagedPayloadCopied( managedPayloadFolderPath );
            var processId = GetProcessId(ProcessName);
            var args = new string[] { processId.ToString(), nativePayloadPath };
            ActiveProcess = Process.Start(injectorPath, args );
        }

        private static int GetProcessId( string processName)
        {
            var process = Process.GetProcessesByName(processName).FirstOrDefault()
                ?? throw new InvalidOperationException($"Unable to find process: {processName}");
            return process.Id;
        }

        private static string GetLocalPath( string path)
        {
            var fullPath = Path.Combine(Environment.CurrentDirectory, path);
            if( Directory.Exists( fullPath ) )
            {
                return fullPath;
            }
            else if( File.Exists(fullPath) )
            {
                return fullPath;
            }
            throw new FileNotFoundException("Unable to find directory or file.", fullPath);
        }

        private void EnsureManagedPayloadCopied( string sourcePath )
        {
            var process = Process.GetProcessesByName(ProcessName).FirstOrDefault() 
                ?? throw new InvalidOperationException($"Unable to find process: {ProcessName}");
            var processDirectory = Path.GetDirectoryName(process.MainModule.FileName);
            Console.WriteLine($"Copying dependencies to process directory:");
            foreach ( var file in Directory.GetFiles( sourcePath ))
            {
                var fileName = Path.GetFileName( file );
                var destinationPath = Path.Combine(processDirectory, fileName);
                // Overwrite in case there's a newer version of the payload or deps.
                File.Copy( file, destinationPath, true );
                Console.WriteLine($"\t{fileName} copied");
            }
        }
    }
}
