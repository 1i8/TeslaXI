using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

using TheLeftExit.Growtopia.Native;
using static TheLeftExit.Growtopia.ObjectModel.QueryConditions;


namespace TheLeftExit.Growtopia.ObjectModel
{
    public class Growtopia : GameClass // It's not exactly one, but the inherited class has some nice features
    {
        private static bool queried = false;
        public Growtopia(Int32 processId, Int32 appOffset = -1)
        {
            if (appOffset != -1)
                AppQuery.Default = appOffset;

            Address = (Int64)Process.GetProcessById(processId).MainModule.BaseAddress;
            Handle = Memory.OpenProcess(processId);

            if (!queried)
            {
                FindMemberOffsets();
                queried = true;
            }
        }

        [QueriedNestedClass]
        public App App => Offset<App>(AppOffset, ref AppInitialized);
        private static PointerQuery AppQuery = new PointerQuery { Condition = RTTI("App"), Range = 0x1200000 };
        private static Int32 AppOffset;
        private static bool AppInitialized;
    }

    public class App : GameClass
    {
        [QueriedNestedClass] // App->GameLogicComponent
        public GameLogicComponent GameLogicComponent => Offset<GameLogicComponent>(GameLogicComponentOffset, ref GameLogicComponentInitialized);
        private static PointerQuery GameLogicComponentQuery = new PointerQuery { Condition = RTTI("GameLogicComponent"), Range = 0x1000 };
        private static Int32 GameLogicComponentOffset;
        private static bool GameLogicComponentInitialized;
    }

    public class GameLogicComponent : GameClass
    {
        [QueriedNestedClass] // GameLogicComponent->World
        public World World  => Offset<World>(WorldOffset, ref WorldInitialized);
        private static PointerQuery WorldQuery = new PointerQuery { Condition = RTTI("World"), Range = 0x1000 };
        private static Int32 WorldOffset;
        private static bool WorldInitialized;

        [QueriedNestedClass] // GameLogicComponent->WorldRenderer
        public WorldRenderer WorldRenderer => Offset<WorldRenderer>(WorldRendererOffset, ref WorldRendererInitialized);
        private static PointerQuery WorldRendererQuery = new PointerQuery { Condition = RTTI("WorldRenderer"), Range = 0x200 };
        private static Int32 WorldRendererOffset;
        private static bool WorldRendererInitialized;

        [QueriedNestedClass] // GameLogicComponent->NetAvatar
        public NetAvatar NetAvatar => Offset<NetAvatar>(NetAvatarOffset, ref NetAvatarInitialized);
        private static PointerQuery NetAvatarQuery = new PointerQuery { Condition = RTTI("NetAvatar"), Range = 0x1000 };
        private static Int32 NetAvatarOffset;
        private static bool NetAvatarInitialized;

        [QueriedNestedClass] // GameLogicComponent->[Inventory]
        public GameList<ItemSlot> Inventory => OffsetList<ItemSlot>(InventoryOffset, ref InventoryInitialized, ItemSlot.Constructor);
        private static PointerQuery InventoryQuery = new PointerQuery { Condition = IsInventory, Range = 0x300, Kind = ScanType.ScanByValue };
        private static Int32 InventoryOffset;
        private static bool InventoryInitialized;
    }

    public class World : GameClass
    {
        [QueriedNestedClass] // World->WorldTileMap
        public WorldTileMap WorldTileMap => Offset<WorldTileMap>(WorldTileMapOffset, ref WorldTileMapInitialized, true);
        private static PointerQuery WorldTileMapQuery = new PointerQuery { Condition = RTTI("WorldTileMap"), Range = 0x200, Kind = ScanType.ScanByValue };
        private static Int32 WorldTileMapOffset;
        private static bool WorldTileMapInitialized;

        [QueriedNestedClass] // World->WorldObjectMap
        public WorldObjectMap WorldObjectMap => Offset<WorldObjectMap>(WorldObjectMapOffset, ref WorldObjectMapInitialized, true);
        private static PointerQuery WorldObjectMapQuery = new PointerQuery { Condition = RTTI("WorldObjectMap"), Range = 0x200, Kind = ScanType.ScanByValue };
        private static Int32 WorldObjectMapOffset;
        private static bool WorldObjectMapInitialized;
    }

    public class WorldTileMap : GameClass
    {
        public Int32 Width => Handle.ReadInt32(Address + 0x08);
        public Int32 Height => Handle.ReadInt32(Address + 0x0C);
        public WorldTile this[Int32 X, Int32 Y] 
        {
            get
            {
                Int64 baseAddress = Handle.ReadInt64(Address + 0x18);
                if (X < 0 || X >= Width || Y < 0 || Y >= Height)
                    throw new ArgumentOutOfRangeException();
                return new WorldTile(Handle, baseAddress + BlockOffset * (Y * Width + X));
            }
        }
        // TODO: block data struct, block data offsets, tileextra, whatever
        private const Int32 BlockOffset = 0x90;
    }

    public class WorldObjectMap : GameClass
    {
        [QueriedNestedClass] // WorldObjectMap->[DroppedItems]
        public GameList<WorldObject> DroppedItems => OffsetList<WorldObject>(DroppedItemsOffset, ref DroppedItemsInitialized, WorldObject.Constructor);
        private static PointerQuery DroppedItemsQuery = new PointerQuery { Condition = IsDoubleLinkedList, Range = 0x100, Kind = ScanType.ScanByValue };
        private static Int32 DroppedItemsOffset;
        private static bool DroppedItemsInitialized;
    }

    public class WorldRenderer : GameClass
    {
        [QueriedNestedClass] // WorldRenderer->WorldCamera
        public WorldCamera WorldCamera => Offset<WorldCamera>(WorldCameraOffset, ref WorldCameraInitialized, true);
        private static PointerQuery WorldCameraQuery = new PointerQuery { Condition = RTTI("WorldCamera"), Range = 0x200, Kind = ScanType.ScanByValue };
        private static Int32 WorldCameraOffset;
        private static bool WorldCameraInitialized;
    }

    public class WorldCamera : GameClass
    {
        public PointF CameraPosition => new PointF(Handle.ReadSingle(Address + 0x10), Handle.ReadSingle(Address + 0x14));
        public Single ZoomFactor => Handle.ReadSingle(Address + 0x2C);
        public SizeF ScreenSize => new SizeF(Handle.ReadSingle(Address + 0x38), Handle.ReadSingle(Address + 0x3C));
    }

    public class NetAvatar : GameClass
    {
        public Point Position => new Point((Int32)Handle.ReadSingle(Address + 0x08), (Int32)Handle.ReadSingle(Address + 0x0C));
        public bool FacingLeft => Handle.ReadBoolean(Address + 0x61);
    }
}