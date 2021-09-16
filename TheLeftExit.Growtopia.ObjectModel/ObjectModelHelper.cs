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
    public static class ObjectModelHelper {
        public static Int64? GetOffset(Type parentType, string propertyName) {
            PropertyInfo property = parentType.GetRuntimeProperty(propertyName);
            if (property == null)
                return null;
            PointerQueryAttribute attribute = property.GetCustomAttribute<PointerQueryAttribute>();
            if (attribute == null)
                return null;
            PointerQuery query = parentType.GetRuntimeFields().First(x => x.Name == attribute.PointerQueryFieldName).GetValue(null) as PointerQuery;
            return query.Offset;
        }
        internal static PointerQueryCondition RTTIByRef(string name) => (MemorySource source, UInt64 addr) => {
            if (!source.TryRead(addr, out UInt64 target))
                return PointerQueryConditionResult.Break;
            if (source.GetRTTIClassNames64(target)?.Contains(name) ?? false)
                return PointerQueryConditionResult.Return;
            return PointerQueryConditionResult.Continue;
        };

        internal static PointerQueryCondition RTTIByVal(string name) => (MemorySource source, UInt64 addr) => {
            if (!source.TryRead<Byte>(addr, out _))  // Dummy-read for an out-of-bounds check
                return PointerQueryConditionResult.Break;
            if (source.GetRTTIClassNames64(addr)?.Contains(name) ?? false)
                return PointerQueryConditionResult.Return;
            return PointerQueryConditionResult.Continue;
        };

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
    }

    public class ObjectModelException : ApplicationException {
        internal ObjectModelException() : base() { }
        internal ObjectModelException(Type sourceClass, Type targetClass) : base($"Could not link {sourceClass.Name}->{targetClass.Name}.") { }
    }

    internal class PointerQueryAttribute : Attribute {
        public string PointerQueryFieldName { get; }
        public PointerQueryAttribute(string fieldName) : base() { PointerQueryFieldName = fieldName; }
    }
}
