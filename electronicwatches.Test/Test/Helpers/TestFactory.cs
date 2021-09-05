using electronicwatches.functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace electronicwatches.functions.Test.Helpers
{
    public class TestFactory
    {
        public static TimeEntity GettimeEntity()
        {
            return new TimeEntity
            {
                ETag = "*",
                PartitionKey = "TIME",
                RowKey = Guid.NewGuid().ToString(),
                Id = 2,
                BusinessHour = DateTime.UtcNow,
                Type = 0,
                Consolidated = false
            };
        }

        public static DateEntity GetDateEntity()
        {
            return new DateEntity
            {
                ETag = "*",
                PartitionKey = "DATE",
                RowKey = Guid.NewGuid().ToString(),
                DateWorked = DateTime.UtcNow,
                Id = 2,
                Minute = 50
            };
        }

        public static DefaultHttpRequest CreateHttpRequestforDelete(string Row, TimeEntity TimeRequest)
        {
            string request = JsonConvert.SerializeObject(TimeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{Row}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid todoId, TimeEntity TimeRequest)
        {
            string request = JsonConvert.SerializeObject(TimeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{todoId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(DateEntity dateRequest)
        {
            string request = JsonConvert.SerializeObject(dateRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{dateRequest.DateWorked}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid timeId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{timeId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(TimeEntity timeRequest)
        {
            string request = JsonConvert.SerializeObject(timeRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }


        public static TimeEntity GetTodoRequest()
        {
            return new TimeEntity
            {
                Id = 2,
                BusinessHour = DateTime.UtcNow,
                Consolidated = false,
                Type = 0
            };
        }


        public static DateEntity GetDateRequest()
        {
            return new DateEntity
            {
                DateWorked = DateTime.UtcNow,
                Id = 2,
                Minute = 50
            };
        }

        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }

        public static List<TimeEntity> GetAll()
        {
            List<TimeEntity> listTime = new List<TimeEntity>();
            TimeEntity entity = new TimeEntity
            {
                ETag = "*",
                PartitionKey = "TIME",
                RowKey = Guid.NewGuid().ToString(),
                Id = 2,
                BusinessHour = DateTime.UtcNow,
                Type = 0,
                Consolidated = false
            };
            listTime.Add(entity);
            return listTime;
        }

        public static List<DateEntity> GetAllByDate()
        {
            List<DateEntity> listTime = new List<DateEntity>();
            DateEntity entity = new DateEntity
            {
                ETag = "*",
                PartitionKey = "DATE",
                RowKey = Guid.NewGuid().ToString(),
                DateWorked = DateTime.UtcNow,
                Id = 2,
                Minute = 50
            };
            listTime.Add(entity);
            return listTime;
        }

    }
}
