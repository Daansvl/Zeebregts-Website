using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MandagenRegistratie.classes
{
    public class Periode
    {
        public bool IsProjectleider { get; set; }
        public int Duration { get; set; }
        public bool ReadOnly { get; set; }
        public bool IsLeadingPeriod { get; set; }
        public bool IsEditablePeriod { get; set; }

        public Periode()
        {
        }
    }
}
