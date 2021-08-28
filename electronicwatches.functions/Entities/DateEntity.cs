using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace electronicwatches.functions.Entities
{
     public class DateEntity: TableEntity
    {
        public int Id { get; set; }

        public DateTime DateWorked { get; set; }

        public int Hours { get; set; }
    }
}
