using System;
using System.Threading;
using System.Threading.Tasks;
using TheLeftExit.Growtopia.ObjectModel;

namespace TheLeftExit.TeslaX
{
    public partial class TeslaBot
    {
        public void Debug(Action<String> log, CancellationToken token)
        {
            NetAvatar netAvatar = game.App.GameLogicComponent.NetAvatar;
            WorldTileMap worldTileMap = game.App.GameLogicComponent.World.WorldTileMap;
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(10);
                var info = BlockAhead(netAvatar, worldTileMap);
                if (info.Tile.IsEmpty)
                {
                    log("Detected: nothing.");
                    continue;
                }
                Int32 distance = PunchingDistance(netAvatar.Position.X, info.X, netAvatar.FacingLeft);
                log($"Detected: {items[info.Tile.Foreground].Name} | {items[info.Tile.Background].Name} (distance: {distance}).");
            }
        }
    }
}
