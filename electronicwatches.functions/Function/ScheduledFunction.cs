using System;
using System.Threading.Tasks;
using electronicwatches.functions.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace electronicwatches.functions.Function
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            [Table("dates", Connection = "AzureWebJobsStorage")] CloudTable datesTable,
            ILogger log)
        {

            log.LogInformation($"calculating Dates function executed at: {DateTime.Now}");



            string filter = TableQuery.GenerateFilterConditionForBool("Consolidated", QueryComparisons.Equal, false);

            TableQuery<TimeEntity> tableQuery = new TableQuery<TimeEntity>().Where(filter);
            TableQuerySegment<TimeEntity> times = await timeTable.ExecuteQuerySegmentedAsync(tableQuery, null);

            int Consolidated = 0;
            int hour = 0;
            int minute = 0;
            int total = 0;

            foreach (TimeEntity login in times)
            {
                if (login.Type == 0)
                {
                    foreach (TimeEntity completed in times)
                    {
                        if (completed.Id == login.Id && completed.Type == 1)
                        {                 
                                log.LogInformation($"esta es la fecha : {completed.BusinessHour.Hour}  ////// {login.BusinessHour.Hour}  trabajadas por él");
                                hour = completed.BusinessHour.Hour - login.BusinessHour.Hour;
                                minute = completed.BusinessHour.Minute - login.BusinessHour.Minute;

                                total = total + (hour*60) + minute;
                                 log.LogInformation($"////////{minute}///{hour} ////////");
                                completed.Consolidated = true;
                                await timeTable.ExecuteAsync(TableOperation.Replace(completed));
                                Consolidated++;
                                log.LogInformation($"total dentro en minutos: {total}  trabajadas perro items at: {DateTime.Now}");
                                 TableOperation findid = TableOperation.Retrieve<DateEntity>("DATE", completed.Id.ToString());
                                TableResult findresult = await datesTable.ExecuteAsync(findid);

                            if(findresult.Result == null)
                            {
                                DateEntity timeEntity = new DateEntity
                                {
                                    Id = completed.Id,
                                    ETag = "*",
                                    DateWorked = DateTime.UtcNow,
                                    PartitionKey = "DATE",
                                    RowKey = Guid.NewGuid().ToString(),
                                    Hours = total,
                                };
                                TableOperation addOperation = TableOperation.Insert(timeEntity);
                                await datesTable.ExecuteAsync(addOperation);
                            }
                            else
                            {

                            }
                               
                        }
                    }
                }
                total = 0;
            }


            log.LogInformation($"Estas son las horas en su totalidad: {total}  trabajadas perro items at: {DateTime.Now}");
            log.LogInformation($"Updated: {Consolidated} items at: {DateTime.Now}");
        }
    }
}
