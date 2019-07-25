using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patlite_test
{
    class BasicCode
    {
        public static string[] WRITE_HEADER = new string[] { "58", "58", "53", "00", "00", "06" };

        public static string ACK = "06";

        public static string NAK = "15";

        public static string[] READ = new string[] { "58", "58", "47", "00", "00", "00" };

        public static string[] CLEAR = new string[] { "58", "58", "43", "00", "00", "00" };
    }
}
