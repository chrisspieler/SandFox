using HarmonyLib;
using Sandbox;

namespace SandFox.Patches.Sauna;

[HarmonyPatchCategory("fish.sauna")]
[HarmonyPatch(typeName: "Sauna.Player", methodName: "OnPreRender")]
public class SaunaFovPatch
{
    [ConVar("sauna_camera_fov")]
    public static float CameraFov { get; set; } = 100f;

    public static void Postfix() 
    {
        if ( Game.ActiveScene is null )
            return;

        var targetFov = Input.Down("View") ? 30f : CameraFov;
        var camera = Game.ActiveScene.Camera;
        camera.FieldOfView = camera.FieldOfView.LerpTo( targetFov, Time.Delta * 10f );
    }
}
