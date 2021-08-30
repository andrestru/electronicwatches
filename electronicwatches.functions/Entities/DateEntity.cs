using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace electronicwatches.functions.Entities
{
    public class DateEntity : TableEntity
    {

        public int Id { get; set; }

        public DateTime DateWorked { get; set; }

        public int Minute { get; set; }
    }
}
