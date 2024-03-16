using Sandbox;

namespace SandFox.Commands;

public static class CameraCommands
{
    [ConCmd("camera_fov")]
    public static void SetCameraFov(float fov)
    {
        if (Game.ActiveScene == null)
        {
            Log.Info("Active scene is null");
            return;
        }
        if (Game.ActiveScene.Camera is null)
        {
            Log.Info("No camera found in active scene");
            return;
        }
        Game.ActiveScene.Camera.FieldOfView = fov;
    }
}
