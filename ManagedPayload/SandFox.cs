using Sandbox;
using System.Reflection;
using System.Text;

namespace SandFox;

public static class SandFoxSystem
{
    public static void Init()
    {
        try
        {
            Log.Info($"Adding console commands from {nameof(ManagedPayload)}");
            Commands.ConsoleCommands.AddConsoleCommands(Assembly.GetExecutingAssembly());
        }
        catch (Exception e)
        {
            Log.Error(e);
            return;
        }
        Log.Info($"Successful {nameof(SandFoxSystem)} init.");
    }
}
