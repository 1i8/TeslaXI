using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.RTTI;
using TheLeftExit.Memory.Sources;

namespace TheLeftExit.Growtopia.ObjectModel { 
    internal class ObjectModelException : ApplicationException {
        internal ObjectModelException() : base() { }
        internal ObjectModelException(Type sourceClass, Type targetClass) : base($"Could not link {sourceClass.Name}->{targetClass.Name}.") { }
    }

    internal static class ObjectModelHelper {
        internal static UInt64 GetResultByRef(this PointerQuery query, MemorySource source, UInt64 baseAddress) {
            PointerQueryResult? result = query.GetResult(source, baseAddress);
            if (!result.HasValue || !source.TryRead(result.Value.Target, out UInt64 targetAddress))
                throw new ObjectModelException();
            return targetAddress;
        }
        
        internal static UInt64 GetResultByVal(this PointerQuery query, MemorySource source, UInt64 baseAddress) {
            PointerQueryResult? result = query.GetResult(source, baseAddress);
            if (!result.HasValue)
                throw new ObjectModelException();
            return result.Value.Target;
        }

        internal static T ForceRead<T>(this MemorySource source, UInt64 address) where T : unmanaged {
            if (!source.TryRead(address, out T result))
                throw new ObjectModelException();
            return result;
        }

        internal static PointerQueryConditionResult IsInventory(MemorySource source, UInt64 address) {
            if (!source.TryRead(address, out UInt64 header))
                return PointerQueryConditionResult.Break;
            if (source.TryRead(header, out UInt64 item1) &&
                source.TryRead(item1, out UInt64 item2) &&
                source.TryRead(item1 + 0x10, out UInt32 itemInfo1) &&
                source.TryRead(item2 + 0x10, out UInt32 itemInfo2) &&
                (itemInfo1 == 0x00010012 && itemInfo2 == 0x00010020))
                return PointerQueryConditionResult.Return;
            return PointerQueryConditionResult.Continue;
        }

        internal static PointerQueryConditionResult IsDoubleLinkedList(MemorySource source, UInt64 address) {
            if (!source.TryRead(address, out UInt64 header))
                return PointerQueryConditionResult.Break;
            if (source.TryRead(header, out UInt64 item1) &&
                source.TryRead(item1 + 0x08, out UInt64 loopback) &&
                (header == loopback))
                return PointerQueryConditionResult.Return;
            return PointerQueryConditionResult.Continue;
        }
    }
}
