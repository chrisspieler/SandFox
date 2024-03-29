﻿using Sandbox;
using System.Text;

namespace SandFox.Commands;

public static class GameObjectCommands
{
    [ConCmd("gameobject_dump")]
    public static void DumpGameObjects()
    {
        if (Game.ActiveScene == null)
        {
            Log.Info("Active scene is null");
            return;
        }
        Log.Info($"Dumping game objects from scene: {Game.ActiveScene.Name}");
        foreach (var go in Game.ActiveScene.GetAllObjects(true))
        {
            Log.Info(GameObjectToString(go));
        }

        string GameObjectToString(Sandbox.GameObject go)
        {
            var sb = new StringBuilder();
            sb.Append($"{go.Id}: \"{go.Name}\", {go.Transform.World.ToString()}");
            foreach (var tag in go.Tags.TryGetAll())
            {
                sb.Append($", \"{tag}\"");
            }
            foreach (var component in go.Components.GetAll())
            {
                sb.Append($"\r\n\t{component.GetType().Name}, enabled: {component.Enabled}");
            }
            return sb.ToString();
        }
    }

    [ConCmd("gameobject_setenabled")]
    public static void SetGameObjectEnabled(string gameObject, bool enabled)
    {
        if (Game.ActiveScene == null)
        {
            Log.Info("Active scene is null");
            return;
        }
        Sandbox.GameObject go = null;
        if (Guid.TryParse(gameObject, out var guid))
        {
            go = Game.ActiveScene.Directory.FindByGuid(guid);
        }
        else
        {
            go = Game.ActiveScene.Directory.FindByName(gameObject).FirstOrDefault();
        }
        // No matching game object found.
        if (go is null)
            return;
        go.Enabled = enabled;
    }
}
