using Sandbox;

namespace SandFox.Commands;

public static class MapCommands
{
    [ConCmd("map_change")]
    public static void ChangeMap(string mapName)
    {
        if (GameManager.ActiveScene == null)
        {
            Log.Info("Active scene is null");
        }
        var map = GameManager.ActiveScene
            .GetAllComponents<MapInstance>()
            .FirstOrDefault();
        if (map == null)
        {
            return;
        }
        map.Enabled = false;
        map.MapName = mapName;
        map.OnMapLoaded += () =>
        {
            PlayerCommands.SetPlayerPosition( new Vector3( 0, 0, 300f ) );
            GameManager.ActiveScene.NavMesh.SetDirty();
        };
        map.Enabled = true;
    }
}
