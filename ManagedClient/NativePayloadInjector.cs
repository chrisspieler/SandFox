using System.Diagnostics;

namespace ManagedClient
{
    internal class NativePayloadInjector
    {
        public const string INJECTOR_APP_NAME = "Injector.exe";
        public const string NATIVE_PAYLOAD_NAME = "NativePayload.dll";
        public const string MANAGED_PAYLOAD_NAME = "ManagedPayload.dll";

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

            var injectorPath = EnsureFile(INJECTOR_APP_NAME);
            var nativePayloadPath = EnsureFile(NATIVE_PAYLOAD_NAME);
            var managedPayloadPath = EnsureFile(MANAGED_PAYLOAD_NAME);

            EnsureManagedPayloadCopied( managedPayloadPath );
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

        private static string EnsureFile( string fileName)
        {
            var filePath = Path.Combine(Environment.CurrentDirectory, fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Unable to find file.", filePath);
            }
            return filePath;
        }

        private void EnsureManagedPayloadCopied( string sourcePath )
        {
            var process = Process.GetProcessesByName(ProcessName).FirstOrDefault() 
                ?? throw new InvalidOperationException($"Unable to find process: {ProcessName}");
            var processDirectory = Path.GetDirectoryName(process.MainModule.FileName);
            var destinationPath = Path.Combine(processDirectory, MANAGED_PAYLOAD_NAME);
            Console.WriteLine($"Copying {MANAGED_PAYLOAD_NAME} to {processDirectory}");
            File.Copy(sourcePath, destinationPath, true);
        }
    }
}
