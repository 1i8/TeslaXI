using System;
using System.Threading;
using System.Threading.Tasks;
using TheLeftExit.Growtopia.ObjectModel;

using static TheLeftExit.Growtopia.GameWindow;

namespace TheLeftExit.TeslaX
{
    public partial class TeslaBot
    {
        public WorldTile GetTileAhead() => BlockAhead(game.App.GameLogicComponent.NetAvatar, game.App.GameLogicComponent.World.WorldTileMap).Tile;

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
                log($"Detected: {WorldTileToString(info.Tile)} (distance: {distance}).");
            }
        }

        public void Break(Action<String> log, Func<WorldTile, bool> condition, CancellationToken token)
        {
            NetAvatar netAvatar = game.App.GameLogicComponent.NetAvatar;
            WorldTileMap worldTileMap = game.App.GameLogicComponent.World.WorldTileMap;

            MovementManager movementManager = new(100, 150);
            PunchManager punchManager = new();

            while (!token.IsCancellationRequested)
            {
                var info = BlockAhead(netAvatar, worldTileMap);
                if (info.Tile.IsEmpty)
                {
                    log("Finished: no blocks in range");
                    break;
                }
                Int32 distance = PunchingDistance(netAvatar.Position.X, info.X, netAvatar.FacingLeft);
                if(distance > Range)
                {
                    log("Finished: no blocks in range");
                    break;
                }
                if (!condition(info.Tile))
                {
                    log("Finished: target does not match the condition.");
                    break;
                }

                bool? toMove = movementManager.Update(distance > TargetDistance);
                if (toMove.HasValue)
                    window.SendKey(netAvatar.FacingLeft ? LeftKey : RightKey, toMove.Value);
                bool? toPunch = punchManager.Update();
                if (toPunch.HasValue)
                    window.SendKey(PunchKey, toPunch.Value);
            }

            window.SendKey(LeftKey, false);
            window.SendKey(RightKey, false);
            window.SendKey(PunchKey, false);
        }
    }
}
