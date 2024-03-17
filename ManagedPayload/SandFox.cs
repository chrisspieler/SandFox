using HarmonyLib;
using System.Reflection;

namespace SandFox;

public static class SandFoxSystem
{
    public static Assembly CurrentGameAssembly { get; private set; }
    public static Harmony HarmonyInstance { get; private set; }
    public static void Init( Assembly gameAssembly )
    {
        CurrentGameAssembly = gameAssembly;

        try
        {
            AddConsoleCommands();
            InstantiateHarmony();
        }
        catch (Exception e)
        {
            Log.Error(e);
            return;
        }
    }

    private static void AddConsoleCommands()
    {
        Log.Info($"Adding console commands from {nameof(ManagedPayload)}");
        Commands.ConsoleCommands.AddConsoleCommands(Assembly.GetExecutingAssembly());
        Log.Info($"Successful {nameof(SandFoxSystem)} init.");
    }

    private static void InstantiateHarmony()
    {
        if ( CurrentGameAssembly is null )
            throw new InvalidOperationException("A game assembly must be loaded before instantiating Harmony.");

        var gameAssemblyName = CurrentGameAssembly.GetName().Name.Replace("package.", "");
        HarmonyInstance = new Harmony($"com.{gameAssemblyName}");
        HarmonyInstance.PatchCategory(Assembly.GetExecutingAssembly(), gameAssemblyName);
        Log.Info($"Harmony instantiated: {HarmonyInstance.Id}");
    }
}
