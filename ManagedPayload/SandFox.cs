using Sandbox;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace ManagedPayload;

public static class SandFox
{
    public static void Init()
    {
        try
        {
            ChangeMap("facepunch.flatgrass");
        }
        catch (Exception e)
        {
            Log.Error(e);
            return;
        }
        Log.Info("Successful SandFox init.");
    }

    private static void DumpGameObjects()
    {
        if (GameManager.ActiveScene == null)
        {
            Log.Info("Active scene is null");
        }
        Log.Info($"Dumping game objects from scene: {GameManager.ActiveScene.Name}");
        foreach( var go in GameManager.ActiveScene.GetAllObjects( true ))
        {
            Log.Info(GameObjectToString(go));
        }

        string GameObjectToString( GameObject go)
        {
            var sb = new StringBuilder();
            sb.Append(go.Name);
            sb.Append(", ");
            sb.Append(go.Transform.World.ToString());
            foreach( var tag in go.Tags.TryGetAll())
            {
                sb.Append($", \"{tag}\"");
            }
            foreach( var component in go.Components.GetAll())
            {
                sb.Append($"\t{component.GetType().Name}, enabled: {component.Enabled}");
            }
            return sb.ToString();
        }
    }

    private static void SetPlayerZPosition( float height )
    {
        if (GameManager.ActiveScene is null)
            return;
        var player = GameManager.ActiveScene
            .Directory
            .FindByName("player", false)
            .FirstOrDefault();
        if ( player is null )
        {
            Log.Info($"player not found");
            return;
        }
        player.Transform.Position = player.Transform.Position.WithZ( height );
    }

    private static void ChangeMap( string mapName)
    {
        if (GameManager.ActiveScene == null)
        {
            Log.Info("Active scene is null");
        }
        var map = GameManager.ActiveScene
            .GetAllComponents<MapInstance>()
            .FirstOrDefault();
        if ( map == null)
        {
            return;
        }
        map.Enabled = false;
        map.MapName = mapName;
        map.OnMapLoaded += () =>
        {
            SetPlayerZPosition(1000f);
            GameManager.ActiveScene.NavMesh.SetDirty();
        };
        map.Enabled = true;
    }

    private static void DumpAllTypes()
    {
        List<Type> types = new List<Type>();
        // Dumps all types that can be found in the current AppDomain
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.FullName))
        {
            foreach (var type in assembly.GetTypes().OrderBy(t => t.Name))
            {
                types.Add(type);
            }
        }
        var typeNames = types.Select(t => $"{t.Assembly}: {t.FullName}");
        System.IO.File.WriteAllLines("C:\\temp\\sboxTypeDump.txt", typeNames);
    }
}
