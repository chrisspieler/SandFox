using Sandbox;
using SandFox.Commands;

namespace SandFox.Components;

/// <summary>
/// While enabled, continously overrides scene camera's field of view.
/// </summary>
public sealed class ForceCameraFov : Component
{
    [Property] public float FieldOfView { get; set; } = 90f;

    protected override void OnPreRender()
    {
        CameraCommands.SetCameraFov( FieldOfView );
    }

    [ConCmd("force_camera_fov")]
    public static void Force( float fieldOfView )
    {
        if (Game.ActiveScene is null)
            return;

        if ( fieldOfView <= 0f )
        {
            DestroyExisting();
            return;
        }    

        var forceFovComponent = GetOrCreate();
        forceFovComponent.FieldOfView = fieldOfView;
    }

    /// <summary>
    /// Destroys all <see cref="GameObject"/> in the scene that have a <see cref="ForceCameraFov"/> component.
    /// </summary>
    public static void DestroyExisting()
    {
        if (Game.ActiveScene is null)
            return;

        var allExisting = Game.ActiveScene.GetAllComponents<ForceCameraFov>();
        foreach( var forceGo in allExisting )
        {
            forceGo.GameObject.Destroy();
        }
    }

    /// <summary>
    /// Returns an existing or newly created <see cref="ForceCameraFov"/>, or <c>null</c> if
    /// no scene is currently active.
    /// </summary>
    public static ForceCameraFov GetOrCreate()
    {
        if (Game.ActiveScene is null)
            return null;

        var forceComponent = Game.ActiveScene.GetAllComponents<ForceCameraFov>().FirstOrDefault();
        if (forceComponent is null)
        {
            var forceGo = new GameObject();
            forceGo.Name = "Force Camera Fov";
            forceComponent = forceGo.Components.Create<ForceCameraFov>();
        }
        return forceComponent;
    }
}
