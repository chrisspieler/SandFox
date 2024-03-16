using Sandbox;

namespace SandFox.Commands;

public static class SceneCommands
{
    [ConCmd("scene_list")]
    public static void ListScenes()
    {
        var scenes = ResourceLibrary.GetAll<SceneFile>();
        foreach (var scene in scenes)
        {
            Log.Info(scene.ResourceName);
        }
    }

    [ConCmd("scene_change")]
    public static void ChangeScene(string sceneName)
    {
        var sceneFile = ResourceLibrary.GetAll<SceneFile>()
            .FirstOrDefault(r => r.ResourceName == sceneName);
        if (sceneFile is null)
        {
            Log.Info($"Unable to find scene: {sceneName}");
            return;
        }
        if (Game.ActiveScene is null)
        {
            Log.Info("Active scene is null");
            return;
        }
        Game.ActiveScene.Load(sceneFile);
    }
}
