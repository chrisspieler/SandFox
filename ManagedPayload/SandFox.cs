using System.Reflection;

namespace SandFox;

public static class SandFoxSystem
{
    public static void Init()
    {
        try
        {
            Log.Info($"Adding console commands from {nameof(ManagedPayload)}");
            Commands.ConsoleCommands.AddConsoleCommands(Assembly.GetExecutingAssembly());
            Log.Info($"Successful {nameof(SandFoxSystem)} init.");
        }
        catch (Exception e)
        {
            Log.Error(e);
            return;
        }
    }
}
