using System;
using System.Numerics;
using System.Collections.Generic;

namespace Helpers
{
    public static class ProtocolHelper
    {
        public enum EthernetType : ulong
        {
            Fast = 100000000,       // 100 Mbit/s
            Gigabit = 1000000000,   // 1 Gbit/s
            _10G = 10000000000,     // 10 Gbit/s
            _40G = 40000000000,     // 40 Gbit/s
            _100G = 100000000000    // 100 Gbit/s
        }

        public static ulong GetBitRate(EthernetType ethernetType)
        {
            return (ulong)ethernetType;
        }

        public static double GetBitInterval(EthernetType ethetnetType)
        {
            return 1.0 / GetBitRate(ethetnetType);
        }

        public enum FrameRange
        {
            Min = 72,
            Max = 1526
        }

        public static double GetCapacity(EthernetType ethernetType, int byteFrameLength, bool useMiliSeconds = true)
        {
            if (!IsRightFrameLength(byteFrameLength))
                throw new ArgumentOutOfRangeException("byteFrameLength",
                    String.Format("Frame length must be between {0} and {1}", (int)FrameRange.Min, (int)FrameRange.Max));

            return 1.0 /
                (GetBitInterval(ethernetType) * (8 * byteFrameLength + 96) * (useMiliSeconds? Math.Pow(10, 3) : 1));
        }

        public static bool IsRightFrameLength(int byteFrameLength)
        {
            return (int)FrameRange.Min <= byteFrameLength && byteFrameLength <= (int)FrameRange.Max;
        }
    }
}