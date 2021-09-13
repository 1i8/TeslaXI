using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLeftExit.Memory.Queries;
using TheLeftExit.Memory.RTTI;
using TheLeftExit.Memory.Sources;

namespace TheLeftExit.Growtopia.ObjectModel {
    internal static class ObjectModelHelper {
        public static PointerQueryCondition RTTIByRef(string name) => (MemorySource source, UInt64 addr) => {
            if (!source.TryRead(addr, out UInt64 target))
                return PointerQueryConditionResult.Break;
            if (source.GetRTTIClassNames64(target)?.Contains(name) ?? false)
                return PointerQueryConditionResult.Return;
            return PointerQueryConditionResult.Continue;
        };

        public static PointerQueryCondition RTTIByVal(string name) => (MemorySource source, UInt64 addr) => {
            if (!source.TryRead<Byte>(addr, out _))  // Dummy-read for an out-of-bounds check
                return PointerQueryConditionResult.Break;
            if (source.GetRTTIClassNames64(addr)?.Contains(name) ?? false)
                return PointerQueryConditionResult.Return;
            return PointerQueryConditionResult.Continue;
        };

        public static UInt64 GetResultByRef(this PointerQuery query, MemorySource source, UInt64 baseAddress) {
            PointerQueryResult? result = query.GetResult(source, baseAddress);
            if (!result.HasValue || !source.TryRead(result.Value.Target, out UInt64 targetAddress))
                throw new ObjectModelException();
            return targetAddress;
        }

        public static UInt64 GetResultByVal(this PointerQuery query, MemorySource source, UInt64 baseAddress) {
            PointerQueryResult? result = query.GetResult(source, baseAddress);
            if (!result.HasValue)
                throw new ObjectModelException();
            return result.Value.Target;
        }

        public static T ForceRead<T>(this MemorySource source, UInt64 address) where T : unmanaged {
            if (!source.TryRead(address, out T result))
                throw new ObjectModelException();
            return result;
        }
    }

    public class ObjectModelException : ApplicationException {
        internal ObjectModelException() : base() { }
        internal ObjectModelException(Type sourceClass, Type targetClass) : base($"Could not link {sourceClass.Name}->{targetClass.Name}.") { }
    }
}
