using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace electronicwatches.functions.Entities
{
    public class TimeEntity : TableEntity 
    {
        public int Id { get; set; }

        public DateTime BusinessHour { get; set; }

        public int Type { get; set; }

        public bool Consolidated { get; set; }
    }
}
