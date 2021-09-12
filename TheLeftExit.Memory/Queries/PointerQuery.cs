using System;
using System.Runtime.CompilerServices;

using TheLeftExit.Memory.Sources;

namespace TheLeftExit.Memory.Queries {
    public class PointerQuery {
        public PointerQueryCondition condition;
        public UInt32 range;
        public SByte step;

        public PointerQuery(PointerQueryCondition queryCondition, UInt32 maxOffset, SByte scanStep) {
            if (queryCondition == null || scanStep == 0)
                throw new ArgumentException();
            condition = queryCondition;
            range = maxOffset;
            step = scanStep;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public PointerQueryResult? Run(MemorySource source, UInt64 baseAddress) {
            Byte stepAbs = (Byte)Math.Abs(step);
            bool scanForward = step > 0;
            for (UInt32 offset = 0; offset <= range; offset += stepAbs) {
                UInt64 targetAddress = scanForward ? baseAddress + offset : baseAddress - offset;
                PointerQueryConditionResult result = condition(source, targetAddress);
                if (result == PointerQueryConditionResult.Return)
                    return new PointerQueryResult() {
                        Target = targetAddress,
                        Offset = offset
                    };
                else if (result == PointerQueryConditionResult.Break)
                    break;
            }
            return null;
        }

        public PointerQueryResult? Result { get; private set; }

        public PointerQueryResult? GetResult(MemorySource source, UInt64 baseAddress, bool forceRun = false, bool updateResult = true) {
            if (forceRun || !Result.HasValue) {
                PointerQueryResult? result = Run(source, baseAddress);
                if (updateResult || !Result.HasValue)
                    Result = result;
            }
            return Result;
        }
    }

    public delegate PointerQueryConditionResult PointerQueryCondition(MemorySource memorySource, UInt64 address);

    public enum PointerQueryConditionResult {
        Continue,
        Return,
        Break
    }

    public struct PointerQueryResult {
        public UInt64 Target { get; init; }
        public Int64 Offset { get; init; }
    }
}
