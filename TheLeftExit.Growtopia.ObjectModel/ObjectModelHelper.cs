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
            if (!source.TryRead(addr, out UInt64 target)) return PointerQueryConditionResult.Break;
            if (source.GetRTTIClassNames64(target)?.Contains(name) ?? false) return PointerQueryConditionResult.Return;
            return PointerQueryConditionResult.Continue;
        };

        public static PointerQueryCondition RTTIByVal(string name) => (MemorySource source, UInt64 addr) => {
            if (!source.TryRead<Byte>(addr, out _)) return PointerQueryConditionResult.Break; // Dummy-read for an out-of-bounds check
            if (source.GetRTTIClassNames64(addr)?.Contains(name) ?? false) return PointerQueryConditionResult.Return;
            return PointerQueryConditionResult.Continue;
        };

        public static UInt64 GetNestedClassAddress(this PointerQuery query, MemorySource source, UInt64 baseAddress) {
            PointerQueryResult? result = query.GetResult(source, baseAddress);
            if (!result.HasValue || source.TryRead(result.Value.Target, out UInt64 targetAddress))
                throw new ObjectModelException(typeof(App), typeof(GameLogicComponent));
            return targetAddress;
        }
    }

    public class ObjectModelException : Exception {
        internal ObjectModelException(Type sourceClass, Type targetClass) : base($"Could not link {sourceClass.Name}->{targetClass.Name}.") { }
    }
}
