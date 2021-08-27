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
        public static async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            [Table("time", Connection = "AzureWebJobsStorage")] CloudTable timeTable,
            ILogger log)
        {
            log.LogInformation($"calculating and updating function executed at: {DateTime.Now}");

            string filter = TableQuery.GenerateFilterConditionForBool("Consolidated", QueryComparisons.Equal, false);

            TableQuery<TimeEntity> tableQuery = new TableQuery<TimeEntity>().Where(filter);
            TableQuerySegment<TimeEntity> times = await timeTable.ExecuteQuerySegmentedAsync(tableQuery, null);

            int Consolidated = 0;

            foreach (TimeEntity completed in times)
            {
                if (completed.Id == 1)
                {
                    completed.Consolidated = true;
                    await timeTable.ExecuteAsync(TableOperation.Replace(completed));
                    Consolidated++;
                }
            }

            log.LogInformation($"Updated: {Consolidated} items at: {DateTime.Now}");
        }
    }
}
