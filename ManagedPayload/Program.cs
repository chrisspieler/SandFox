using System.Reflection;
using System.Runtime.Loader;
using SandFox;

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
                    var program = asm.GetType("ManagedPayload.Program");
                    var main = program.GetMethod("PayloadMain");
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
        SandFoxSystem.Init();
        return 0;
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