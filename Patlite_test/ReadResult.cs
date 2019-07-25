using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patlite_test
{
    class ReadResult: CommandResult
    {
        string Red { get; set; }

        string Amber { get; set; }

        string Green { get; set; }

        string Buzzer { get; set; }

        public ReadResult(string red, string amber, string green, string buzzer)
        {
            Red = red ?? throw new ArgumentNullException(nameof(red));
            Amber = amber ?? throw new ArgumentNullException(nameof(amber));
            Green = green ?? throw new ArgumentNullException(nameof(green));
            Buzzer = buzzer ?? throw new ArgumentNullException(nameof(buzzer));
        }

        public override string ToString()
        {
            return $"RED: {Red.ToString()} AMBER: {Amber.ToString()} GREEN: {Green.ToString()} BUZZER: {Buzzer.ToString()}";
        }
    }
}
