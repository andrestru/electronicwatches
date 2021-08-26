using System;
using System.Collections.Generic;
using System.Text;

namespace electronicwatches.Common.Model
{
    public class timeWorked
    {
        public int Id { get; set; }

        public DateTime BusinessHour { get; set; }

        public int Type { get; set; }

        public bool Consolidated { get; set; }
    }
}
