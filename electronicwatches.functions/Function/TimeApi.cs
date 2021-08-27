using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using electronicwatches.Common.Model;
using electronicwatches.Common.Response;
using electronicwatches.functions.Entities;

namespace electronicwatches.functions.Function
{
    public static class TimeApi
    {
        [FunctionName(nameof(CreateTime))]
        public static async Task<IActionResult> CreateTime(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            ILogger log)
        {
            log.LogInformation("Recieved a new Time.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            timeWorked time = JsonConvert.DeserializeObject<timeWorked>(requestBody);

            if (time?.Id == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    Success = false,
                    Message = "The request must have a Id"
                });
            }

            if (time?.BusinessHour == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    Success = false,
                    Message = "The request must have a Date"
                });
            }

            if (time?.Type<0 || time?.Type>1)
            {
                return new BadRequestObjectResult(new Response
                {
                    Success = false,
                    Message = "The request must be 0 or 1"
                });
            }


            TimeEntity timeEntity = new TimeEntity
            {
                Id = time.Id,
                ETag = "*",
                BusinessHour = time.BusinessHour,
                PartitionKey = "TIME",
                RowKey = Guid.NewGuid().ToString(),
                Type = time.Type,
                Consolidated = false
            };

            TableOperation addOperation = TableOperation.Insert(timeEntity);
            await timeTable.ExecuteAsync(addOperation);
            string message = "New time stored in table";
            log.LogInformation(message);
           
            return new OkObjectResult(new Response
            {
                Success = true,
                Message = message,
                Result = timeEntity
            });
        }

        [FunctionName(nameof(updateTime))]
        public static async Task<IActionResult> updateTime(
           [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "time/{id}")] HttpRequest req,
           [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
           string id,
           ILogger log)
        {

            log.LogInformation($"Update for: {id}, received.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            timeWorked time = JsonConvert.DeserializeObject<timeWorked>(requestBody);

            //validate id
            TableOperation findid = TableOperation.Retrieve<TimeEntity>("TIME", id);
            TableResult findresult = await timeTable.ExecuteAsync(findid);

            if (findresult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    Success = false,
                    Message = "Not found."
                });
            }

            //Update
            TimeEntity timeEntity = (TimeEntity)findresult.Result;
            timeEntity.Consolidated = time.Consolidated;
            if (time.Type==0 || time.Type==1)
            {
                timeEntity.Type = time.Type;
            }

            if (!string.IsNullOrEmpty(time.BusinessHour.ToString()))
            {
                timeEntity.BusinessHour = time.BusinessHour;
            }

            if (time.Id >= 0)
            {
                timeEntity.Id = time.Id;
            }

            TableOperation updateOperation = TableOperation.Replace(timeEntity);
            await timeTable.ExecuteAsync(updateOperation);
            string message = $"field: {id} updated in table";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                Success = true,
                Message = message,
                Result = timeEntity
            });

        }

        [FunctionName(nameof(Getall))]
        public static async Task<IActionResult> Getall(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "time")] HttpRequest req,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            ILogger log)
        {
            log.LogInformation("Get all recieved.");

            TableQuery<TimeEntity> table = new TableQuery<TimeEntity>();
            TableQuerySegment<TimeEntity> querys = await timeTable.ExecuteQuerySegmentedAsync(table, null);

            string message = "Retrieved all fields";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                Success = true,
                Message = message,
                Result = querys
            });
        }

        [FunctionName(nameof(GetbyId))]
        public static async Task<IActionResult> GetbyId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "time/{id}")] HttpRequest req,
            [Table("time", "TIME", "{id}", Connection = "AzureWebJobsStorage")] TimeEntity timeEntity,
            string id,
            ILogger log)
        {
            log.LogInformation($"get by id: {id}, recieved");

            if(timeEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    Success = false,
                    Message = "Not Found"
                });
            }

            string message = $"Field: {timeEntity.RowKey}, retrieved";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                Success = true,
                Message = message,
                Result = timeEntity
            });

        }

            [FunctionName(nameof(DeletebyId))]
        public static async Task<IActionResult> DeletebyId(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "time/{id}")] HttpRequest req,
            [Table("time", "TIME", "{id}", Connection = "AzureWebJobsStorage")] TimeEntity timeEntity,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable cloudTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Delete: {id}, recieved");

            if (timeEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    Success = false,
                    Message = "Not Found"
                });
            }

            await cloudTable.ExecuteAsync(TableOperation.Delete(timeEntity));
            string message = $"Field: {timeEntity.RowKey}, deleted";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                Success = true,
                Message = message,
                Result = timeEntity
            });
        }


    }
}
