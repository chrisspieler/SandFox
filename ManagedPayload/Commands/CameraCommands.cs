using Sandbox;

namespace SandFox.Commands;

public static class CameraCommands
{
    [ConCmd("camera_fov")]
    public static void SetCameraFov(float fov)
    {
        if (GameManager.ActiveScene == null)
        {
            Log.Info("Active scene is null");
            return;
        }
        if (GameManager.ActiveScene.Camera is null)
        {
            Log.Info("No camera found in active scene");
            return;
        }
        GameManager.ActiveScene.Camera.FieldOfView = fov;
    }
}
