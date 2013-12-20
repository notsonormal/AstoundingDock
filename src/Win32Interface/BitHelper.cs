using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstoundingApplications.Win32Interface
{
    public static class BitHelper
    {
        public static bool IsSet(int bitfield, int flag)
        {
            return (bitfield & flag) != 0;
        }

        public static bool IsSet(uint bitfield, uint flag)
        {
            return (bitfield & flag) != 0;
        }
    }
}
