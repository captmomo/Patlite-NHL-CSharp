using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patlite_test
{
    public static class BuzzerMode
    {
        //buzzer modes
        public static string STOP_BUZZER = "00";

        public static string BUZZ_PATTERN_1 = "01";

        public static string BUZZ_PATTERN_2 = "02";

        public static string BUZZ_PATTERN_3 = "03";

        public static string BUZZ_PATTERN_4 = "04";

        public static string BUZZ_REMAIN = "09";

        public enum STATUS
        {
            STOP = 0,

            PATTERN_1 = 1,

            PATTERN_2 = 2,

            PATTERN_3 = 3,

            PATTERN_4 = 4,

            REMAIN = 9
        }
    }
}
