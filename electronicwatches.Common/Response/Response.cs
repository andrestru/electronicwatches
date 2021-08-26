using System;
using System.Collections.Generic;
using System.Text;

namespace electronicwatches.Common.Response
{
    public class Response
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public object Result { get; set; }
    }
}
