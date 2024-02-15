using Sandbox;

namespace SandFox.Commands;

public static class TypeCommands
{
    [ConCmd("type_dump")]
    public static void DumpAllTypes()
    {
        List<Type> types = new();
        // Dumps all types that can be found in the current AppDomain
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName))
        {
            foreach (var type in assembly.GetTypes().OrderBy(t => t.Name))
            {
                types.Add(type);
            }
        }
        var typeNames = types.Select(t => $"{t.Assembly}: {t.FullName}");
        File.WriteAllLines("C:\\temp\\sboxTypeDump.txt", typeNames);
    }
}
