using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patlite_test
{
    public static class LightMode
    {
        //lighting modes for signal tower
        public static string NO_FLASH = "00";

        public static string STEADY = "01";

        public static string FLASHING_PATTERN_1 = "02";

        public static string FLASHING_PATTERN_2 = "03";

        public static string LIGHT_REMAIN = "09";

        public enum STATUS
        {
            NO_FLASH = 0,

            STEADY = 1,

            PATTERN_1 = 2,

            PATTERN_2 = 3
        }
    }
}
