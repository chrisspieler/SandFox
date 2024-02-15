using Sandbox;

namespace SandFox.Commands
{
    public static class PlayerCommands
    {
        [ConCmd("player_setpos")]
        public static void SetPlayerPosition(Vector3 position)
        {
            if (GameManager.ActiveScene is null)
                return;
            var player = GameManager.ActiveScene
                .Directory
                .FindByName("player", false)
                .FirstOrDefault();
            if (player is null)
            {
                Log.Info($"player not found");
                return;
            }
            player.Transform.Position = position;
        }
    }
}
