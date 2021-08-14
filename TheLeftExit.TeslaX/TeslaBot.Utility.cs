using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using TheLeftExit.Growtopia.ObjectModel;

namespace TheLeftExit.TeslaX
{
    public partial class TeslaBot
    {
        private (Int32 X, WorldTile Tile) BlockAhead(NetAvatar netAvatar, WorldTileMap worldTileMap)
        {
            Point playerPos = netAvatar.Position;
            Int32 inc = netAvatar.FacingLeft ? -1 : 1;
            for (Int32 i = playerPos.X / 32 + inc; i >= 0 && i < worldTileMap.Width; i += inc)
            {
                WorldTile tile = worldTileMap[i, playerPos.Y / 32];
                if (!tile.IsEmpty)
                    return (i, tile);
            }
            return (0, new WorldTile());
        }

        private Int32 PunchingDistance(Int32 playerX, Int32 blockX, bool playerDir)
        {
            return playerDir ? playerX - (blockX + 1) * 32 : blockX * 32 - playerX - 1;
        }

        public String WorldTileToString(WorldTile tile)
        {
            StringBuilder sb = new();
            if (tile.Foreground != 0)
            {
                sb.Append($"{items[tile.Foreground].Name} [{tile.Foreground}]");
                if (tile.Background != 0)
                    sb.Append(" | ");
            }
            if (tile.Background != 0)
                sb.Append($"{items[tile.Background].Name} [{tile.Background}]");
            return sb.ToString();
        }
    }
}
