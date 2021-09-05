using electronicwatches.functions.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace electronicwatches.functions.Function
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            [Table("date", Connection = "AzureWebJobsStorage")] CloudTable datesTable,
            ILogger log)
        {

            log.LogInformation("Scheduled function started successfully");



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
                            hour = completed.BusinessHour.Hour - login.BusinessHour.Hour;
                            minute = completed.BusinessHour.Minute - login.BusinessHour.Minute;

                            total = total + (hour * 60) + minute;

                            completed.Consolidated = true;
                            login.Consolidated = true;
                            await timeTable.ExecuteAsync(TableOperation.Replace(completed));
                            await timeTable.ExecuteAsync(TableOperation.Replace(login));
                            Consolidated++;

                            TableOperation findid = TableOperation.Retrieve<DateEntity>("DATE", completed.Id.ToString());
                            TableResult findresult = await datesTable.ExecuteAsync(findid);
                            int y = DateTime.Now.Year;
                            int m = DateTime.Now.Month;
                            int d = DateTime.Now.Day;

                            if (findresult.Result == null)
                            {
                                DateEntity timeEntity = new DateEntity
                                {
                                    Id = completed.Id,
                                    ETag = "*",
                                    DateWorked = DateTime.Parse($"{y}-{m}-{d}Z"),
                                    PartitionKey = "DATE",
                                    RowKey = completed.Id.ToString(),
                                    Minute = total,
                                };
                                TableOperation addOperation = TableOperation.Insert(timeEntity);
                                await datesTable.ExecuteAsync(addOperation);
                                log.LogInformation("****Inserting new record in table***");
                            }
                            else
                            {
                                DateEntity dateEntity = (DateEntity)findresult.Result;
                                dateEntity.Minute += total;
                                TableOperation updateOperation = TableOperation.Replace(dateEntity);
                                await datesTable.ExecuteAsync(updateOperation);
                                log.LogInformation($"Updating { total} record in table");
                            }

                        }
                    }
                }
                total = 0;
            }
            log.LogInformation($"Updated: {Consolidated} items at: {DateTime.Now}");
        }
    }
}
