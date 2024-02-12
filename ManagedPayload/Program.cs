using System.Reflection;
using System.Runtime.Loader;

namespace ManagedPayload;

public static class Program
{
    public static int PayloadMain( string argument )
    {
        try
        {
            var alc = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
            string packageAssembly = GetPackageAssemblyName( alc );
            Log.Info($"ALC name: {alc.Name}, package: {packageAssembly}");
            if (!IsGameAssemblyLoadContext(alc))
            {
                Log.Info("Loading payload in to game assembly load context.");
                alc = GetGameAssemblyLoadContext();
                if ( alc is null)
                {
                    Log.Error("Failed to find game assembly load context.");
                    return -1;
                }
                using (alc.EnterContextualReflection())
                {
                    var assemblyLocation = Assembly.GetExecutingAssembly().Location;
                    Log.Info($"Loading assembly from path: {assemblyLocation}");
                    var asm = alc.LoadFromAssemblyPath(assemblyLocation);
                    Log.Info($"Payload assembly: {asm}");
                    var program = asm.GetType("ManagedPayload.Program");
                    Log.Info($"Payload program: {program?.Name}");
                    var main = program.GetMethod("PayloadMain");
                    Log.Info($"Payload main: {main?.Name}");
                    main?.Invoke(null, new object[] { argument });
                }
                return 0;
            }
        }
        catch (Exception ex )
        {
            Log.Error(ex);
            return -1;
        }
        Log.Info($"In game assembly load context");
        SandFox.Init();
        return 0;
        //var outOfContextScene = typeof(GameManager).GetProperty("ActiveScene").GetValue(null) as Scene;
        //Log.Info($"Out of context is null: {outOfContextScene is null}");
        //using (alc.EnterContextualReflection())
        //{
        //    alc.LoadFromAssemblyName(Assembly.GetCallingAssembly().GetName());
        //    var inContextScene = typeof(GameManager).GetProperty("ActiveScene").GetValue(null) as Scene;
        //    Log.Info($"In context is null: {inContextScene is null}");
        //    SandFox.Init();
        //}
        //return 0;
    }

    private static bool IsIsolatedAssemblyContext(AssemblyLoadContext alc)
    {
        return alc.GetType().Name == "IsolatedAssemblyContext";
    }

    private static bool IsGameAssemblyLoadContext(AssemblyLoadContext alc)
    {
        if (alc is null || !IsIsolatedAssemblyContext(alc))
            return false;

        var packageName = GetPackageAssemblyName(alc);
        if (packageName is null)
            return false;
        return !(packageName.StartsWith("package.base") || packageName.StartsWith("package.local.menu"));
    }

    private static string GetPackageAssemblyName( AssemblyLoadContext alc)
    {
        if (!IsIsolatedAssemblyContext(alc))
            return null;
        var asm = alc.GetType().GetField("Assembly").GetValue(alc) as Assembly;
        return asm?.GetName().Name;
    }

    private static AssemblyLoadContext GetGameAssemblyLoadContext()
    {
        var alcs = AssemblyLoadContext.All
            .Where(IsIsolatedAssemblyContext);
        foreach (var alc in alcs)
        {
            if ( IsGameAssemblyLoadContext(alc) )
            {
                return alc;
            }
        }
        Log.Info("Found no game assembly load context.");
        return null;
    }
}