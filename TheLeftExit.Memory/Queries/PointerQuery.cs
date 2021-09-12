using System;
using System.Runtime.CompilerServices;

using TheLeftExit.Memory.Sources;

namespace TheLeftExit.Memory.Queries {
    public enum PointerQueryScanType {
        ScanByValue,
        ScanByRefReturnValue,
        ScamByRefReturnRef
    }

    public enum PointerQueryScanDirection {
        Forward,
        Backward
    }

    public delegate bool PointerQueryCondition(MemorySource memorySource, UInt64 address);

    public class PointerQuery {
        public PointerQueryCondition Condition { get; init; }
        public UInt32 Range { get; init; }
        public UInt32 Step { get; init; } = 4;
        public UInt32 Default { get; set; } = 0;
        public PointerQueryScanType Kind { get; init; } = PointerQueryScanType.ScanByRefReturnValue;
        public PointerQueryScanDirection Direction { get; init; } = PointerQueryScanDirection.Forward;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)] // Does this do anything? I certainly hope so.
        public PointerQueryResult Run(MemorySource memorySource, UInt64 baseAddress) {
            // Checking cached offset if provided.
            UInt64 defaultAddress = baseAddress + Default;
            if (Condition(memorySource, getScannedAddress(memorySource, defaultAddress)))
                return new PointerQueryResult() {
                    Target = getReturnAddress(memorySource, defaultAddress),
                    Offset = Default
                };

            // Scanning given range.
            for (UInt32 offset = 0; offset <= Range; offset += Step) {
                UInt64 targetAddress = Direction == PointerQueryScanDirection.Forward ? baseAddress + offset : baseAddress - offset;
                if (Condition(memorySource, getScannedAddress(memorySource, targetAddress)))
                    return new PointerQueryResult() {
                        Target = getReturnAddress(memorySource, targetAddress),
                        Offset = offset
                    };
            }

            // Uh oh.
            return PointerQueryResult.None;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private UInt64 getScannedAddress(MemorySource source, UInt64 origin) =>
            Kind == PointerQueryScanType.ScanByValue ? origin : source.Read<UInt64>(origin) ?? 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private UInt64 getReturnAddress(MemorySource source, UInt64 origin) =>
            Kind == PointerQueryScanType.ScamByRefReturnRef ? source.Read<UInt64>(origin) ?? 0 : origin;
    }

    public struct PointerQueryResult {
        public UInt64 Target;
        public Int64 Offset;
        public static readonly PointerQueryResult None = new();
    }
}
