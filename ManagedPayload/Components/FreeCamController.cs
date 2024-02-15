using Sandbox;

namespace SandFox.Components;

public sealed class FreeCamController : Component
{
    [ConVar("freecam_look_sensitivity")]
    public static float FreeCamLookSensitivity { get; set; } = 0.2f;
    [ConVar("freecam_move_speed")]
    public static float MoveSpeed { get; set; } = 200f;
    public List<GameObject> ReenableList { get; set; } = new();
    public Angles EyeAngles { get; set; }

    protected override void OnUpdate()
    {
        var camera = Scene.Camera;
        if ( camera is null)
            return;

        var moveDir = Input.AnalogMove * camera.Transform.Rotation;
        camera.Transform.Position += moveDir * MoveSpeed * Time.Delta;
        EyeAngles += Input.AnalogLook * FreeCamLookSensitivity * Preferences.Sensitivity;
        camera.Transform.Rotation = EyeAngles;
    }

    public void End()
    {
        foreach( var go in ReenableList )
        {
            go.Enabled = true;
        }
        GameObject.Destroy();
    }

    [ConCmd("freecam_start")]
    public static FreeCamController Begin()
    {
        var existingController = GameManager.ActiveScene.GetAllComponents<FreeCamController>();
        if (existingController.Any())
        {
            Log.Info("FreeCam already exists");
            return null;
        }
        var toggled = ToggleEnabledByTag( false, "player" );
        var go = new GameObject( true, "FreeCam" );
        var freeCam = go.Components.Create<FreeCamController>();
        freeCam.ReenableList = toggled;
        return freeCam;
    }

    [ConCmd("freecam_end")]
    public static void EndCurrentFreeCam()
    {
        var currentFreeCam = GameManager.ActiveScene
            .GetAllComponents<FreeCamController>()
            .FirstOrDefault();
        if ( !currentFreeCam.IsValid() )
        {
            Log.Info("No active FreeCam.");
            return;
        }
        currentFreeCam.End();
    }

    private static List<GameObject> ToggleEnabledByTag( bool enabled, string tag )
    {
        var toggled = new List<GameObject>();

        if ( string.IsNullOrWhiteSpace(tag) )
            return toggled;

        var tagged = GameManager.ActiveScene
            .GetAllObjects(false)
            .Where( go => go.Tags.Has(tag) );
        foreach( var go in tagged )
        {
            go.Enabled = enabled;
            toggled.Add(go);
        }
        return toggled;
    }
}
