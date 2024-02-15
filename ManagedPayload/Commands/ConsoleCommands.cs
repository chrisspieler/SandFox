using Sandbox;
using System.Collections;
using System.Reflection;

namespace SandFox.Commands;

public static class ConsoleCommands
{
    private static Type GetTypeFromAssembly(string assemblyName, string typeName)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            if (assembly.GetName().Name == assemblyName)
            {
                return assembly.GetType(typeName);
            }
        }
        return null;
    }

    /// <summary>
    /// Invokes <c>ConsoleSystem.Collection.AddAssembly</c> using the provided assembly as an argument.
    /// </summary>
    internal static void AddConsoleCommands(Assembly assembly)
    {
        var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
        var conVarSystem = GetTypeFromAssembly("Sandbox.Engine", "Sandbox.ConVarSystem");
        if (conVarSystem is null)
        {
            Log.Info("ConVarSystem not found");
            return;
        }
        var addAssemblyMethod = conVarSystem.GetMethod("AddAssembly", flags, new Type[] { typeof(Assembly), typeof(string), typeof(bool) });
        if (addAssemblyMethod is null)
        {
            Log.Info("ConVarSystem.AddAssembly method not found");
            return;
        }
        addAssemblyMethod.Invoke(null, new object[] { assembly, "game", false });
    }

    [ConCmd("dump_console_commands")]
    public static void DumpAllConsoleCommands()
    {
        var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
        var conVarSystem = GetTypeFromAssembly("Sandbox.Engine", "Sandbox.ConVarSystem");
        if (conVarSystem is null)
        {
            Log.Info("ConVarSystem not found");
            return;
        }
        var members = conVarSystem.GetField("Members", flags)?.GetValue(null);
        if (members is null)
        {
            Log.Info("ConVarSystem.Members is null or missing.");
            return;
        }
        var dict = members as IDictionary;
        List<string> lines = new();
        foreach (var command in dict.Values)
        {
            var name = command.GetType().GetProperty("Name", flags)?.GetValue(command);
            var assembly = command.GetType().GetField("assembly", flags)?.GetValue(command) as Assembly;
            var isProtected = command.GetType().GetProperty("IsProtected", flags)?.GetValue(command);
            var help = command.GetType().GetProperty("Help", flags)?.GetValue(command);
            var line = $"{assembly?.GetName()?.Name} {name} (protected: {isProtected}): {help}";
            lines.Add( line );
            Log.Info(line);
        }
        File.WriteAllLines("C:\\temp\\SandFoxConsoleCommandDump.txt", lines);
    }
}
