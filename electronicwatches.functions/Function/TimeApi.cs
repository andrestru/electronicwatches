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

            Console.Write("**************************"+time.BusinessHour);
            log.LogInformation("1." + time.BusinessHour);

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
            string message = "New time stored in table"+ timeEntity.BusinessHour;
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
