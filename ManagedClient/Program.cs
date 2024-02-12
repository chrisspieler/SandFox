namespace ManagedClient;

public class Program
{
    public static int Main(string[] args)
    {
        if ( !GetDllPath( out var dllPath ) )
        {
            Console.WriteLine("No DLL path found.");
            return -1;
        }
        try
        {
            MainLoop();
            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
        }
        catch ( Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
            return -1;
        }
        return 0;
    }

    private static bool GetDllPath( out string dllPath )
    {
        dllPath = string.Empty;
        var args = Environment.GetCommandLineArgs();
        if ( args.Any() )
        {
            dllPath = args[0];
            return true;
        }
        return false;
    }

    private static void MainLoop()
    {
        var injector = new NativePayloadInjector("sbox");
        injector.Inject();
    }
}