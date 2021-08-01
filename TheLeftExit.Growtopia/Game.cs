using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using TheLeftExit.Memory;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.RTTI;

namespace TheLeftExit.Growtopia
{
    // might not even use this
    public enum GameClass
    {
        App,
        GameLogicHandler,
        NetAvatar,
        World
    }

    // All values listed must be initializable within Update
    public enum GameValue
    {
        PlayerX,
        PlayerY,
        PlayerDir,
        WorldData,
        Inventory,
        DroppedItems
    }

    public sealed class Game
    {
        private IntPtr Handle;
        private Int64 BaseAddress;
        private Int32 mainModuleSize;

        private Dictionary<GameValue, Int64> addresses;
        public Int64 this[GameValue item] { get => addresses[item]; }

        public Game(Process gt)
        {
            Handle = gt.Handle; // Questionable - might want to explicitly open a handle, but this one has full access anyway...
            BaseAddress = (Int64)gt.MainModule.BaseAddress;
            mainModuleSize = gt.MainModule.ModuleMemorySize;
        }

        // TODO:
        // 1. Retrieve process handle manually
        // 2. Encapsulate addresses and write funcs to return actual values from memory
        // 3. Since handles seem to get closed, reopen one whenever it's closed on read
        // 4. Add a bunch of { get; } properties to expose addresses, offsets
        // 5. Expose private isSomething functions (could be used by other devs, as well as to scan more broadly)
        // Unordered short term goal: get ahold of VirtualQueryEx and use it to determine query ranges
        // Long term goal: read entire structures (like NetAvatar instead of pX,pY,pDir) in a single read

        public void UpdateAddresses(Int32 defaultAppOffset = 0)
        {
            addresses = new();

            PointerQuery AppQuery = new PointerQuery() { Condition = RTTI("App"), Range = mainModuleSize, Default = defaultAppOffset };
            PointerQuery GameLogicComponentQuery = new PointerQuery() { Condition = RTTI("GameLogicComponent"), Range = 0x1000 };
            PointerQuery NetAvatarQuery = new PointerQuery() { Condition = RTTI("NetAvatar"), Range = 0x1000 };
            PointerQuery WorldQuery = new PointerQuery() { Condition = RTTI("World"), Range = 0x1000 };
            PointerQuery InventoryQuery = new PointerQuery() { Condition = isInventory, Range = 0x1000, Kind = ScanType.ScanByValue };
            PointerQuery DroppedItemsQuery = new PointerQuery() { Condition = isDoubleLinkedList, Range = 0x100 };

            PointerQueryResult App = AppQuery.Run(Handle, BaseAddress);
            PointerQueryResult GameLogicComponent = GameLogicComponentQuery.Run(Handle, App.Target);
            PointerQueryResult NetAvatar = NetAvatarQuery.Run(Handle, GameLogicComponent.Target);
            PointerQueryResult World = WorldQuery.Run(Handle, GameLogicComponent.Target);
            
            PointerQueryResult Inventory = InventoryQuery.Run(Handle, GameLogicComponent.Target);
            PointerQueryResult DroppedItems = DroppedItemsQuery.Run(Handle, World.Target);

            addresses.Add(GameValue.PlayerX, NetAvatar.Target + HardcodedOffsets.PlayerX);
            addresses.Add(GameValue.PlayerY, NetAvatar.Target + HardcodedOffsets.PlayerY);
            addresses.Add(GameValue.PlayerDir, NetAvatar.Target + HardcodedOffsets.PlayerDirection);
            addresses.Add(GameValue.WorldData, World.Target + HardcodedOffsets.WorldData);
            addresses.Add(GameValue.Inventory, Handle.ReadInt64(Inventory.Target));
            addresses.Add(GameValue.DroppedItems, Handle.ReadInt64(DroppedItems.Target));

            if (addresses.Any(x => x.Value == 0))
                throw new MemoryReadingException("Unable to initialize. ");
        }

        #region Private fields used in querying
        private bool isInventory(IntPtr h, Int64 a)
        {
            Int64 header = h.ReadInt64(a);
            Int64 item1 = h.ReadInt64(header);
            Int64 item2 = h.ReadInt64(item1);
            Int32 itemInfo1 = h.ReadInt32(item1 + 0x10);
            Int32 itemInfo2 = h.ReadInt32(item2 + 0x10);
            return itemInfo1 == 0x00010012 && itemInfo2 == 0x00010020;
        }

        private bool isDoubleLinkedList(IntPtr h, Int64 a)
        {
            Int64 header = h.ReadInt64(a);
            Int64 item1 = h.ReadInt64(header);
            Int64 loopback = h.ReadInt64(item1 + 0x08);
            return loopback != 0 && header == loopback;
        }

        private PointerQueryCondition RTTI(String name) =>
            (IntPtr h, Int64 a) => h.GetRTTIClassName64(a) == name;

        // End goal: get rid of this class.
        private static class HardcodedOffsets
        {
            public const Int32 PlayerX = 0x08;
            public const Int32 PlayerY = 0x0C;
            public const Int32 PlayerDirection = 0x61;
            public const Int32 WorldData = 0x28;
        }
        #endregion
    }
}
