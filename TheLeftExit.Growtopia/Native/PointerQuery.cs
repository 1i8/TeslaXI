using System;

namespace TheLeftExit.Growtopia.Native
{
    public enum ScanType
    {
        /// <summary>
        /// Apply Condition to addresses directly.
        /// </summary>
        ScanByValue,
        /// <summary>
        /// Apply Condition to values pointed to by addresses. Return a matching value.
        /// </summary>
        ScanByRefReturnValue,
        /// <summary>
        /// Apply Condition to values pointed to by addresses. Return a pointer to matching value.
        /// </summary>
        ScamByRefReturnRef
    }

    public delegate bool PointerQueryCondition(IntPtr handle, Int64 address);

    /// <summary>
    /// Query that searches an address range for structures with a specified RTTI class name.
    /// </summary>
    public sealed class PointerQuery
    {
        /// <summary>
        /// Condition that the target should fulfill.
        /// </summary>
        public PointerQueryCondition Condition { get; init; }
        /// <summary>
        /// Maximum offset to search in.
        /// </summary>
        public Int32 Range { get; init; }
        /// <summary>
        /// Intervals to check addresses at.
        /// </summary>
        public Int32 Step { get; init; } = 4;
        /// <summary>
        /// First offset to check.
        /// </summary>
        public Int32 Default { get; set; } = 0;
        /// <summary>
        /// The way values are located and returned.<br/>Default: ScanByRefReturnValue.
        /// </summary>
        public ScanType Kind { get; init; } = ScanType.ScanByRefReturnValue;

        public PointerQueryResult Run(IntPtr handle, Int64 source)
        {
            Func<Int64, Int64> getScan = Kind == ScanType.ScanByValue ? x => x : x => handle.ReadInt64(x);
            Func<Int64, Int64> getReturn = Kind == ScanType.ScanByRefReturnValue ? x => handle.ReadInt64(x) : x => x;


            // Checking cached offset if provided.
            Int64 defaultAddress = source + Default;
            if (Condition(handle, getScan(defaultAddress)))
                return new PointerQueryResult()
                {
                    Target = getReturn(defaultAddress),
                    Offset = Default
                };

            // Scanning given range.
            for (Int64 i = source; i <= source + Range; i += Step)
            {
                if (Condition(handle, getScan(i)))
                    return new PointerQueryResult()
                    {
                        Target = getReturn(i),
                        Offset = (Int32)(i - source)
                    };
            }

            // Uh oh.
            return PointerQueryResult.None;
        }
    }

    public struct PointerQueryResult
    {
        public Int64 Target;
        public Int32 Offset;
        public static readonly PointerQueryResult None = new();
    }
}
