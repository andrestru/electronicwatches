using electronicwatches.functions.Entities;
using electronicwatches.functions.Test.Helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace electronicwatches.Test.Test.Helpers
{
    public class MockTableDate : CloudTable
    {
        public MockTableDate(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockTableDate(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockTableDate(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }

        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = 200,
                Result = TestFactory.GetDateEntity()
            });
        }

        public override async Task<TableQuerySegment<DateEntity>> ExecuteQuerySegmentedAsync<DateEntity>(TableQuery<DateEntity> querys, TableContinuationToken token)
        {
            ConstructorInfo info = typeof(TableQuerySegment<DateEntity>)
                 .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                 .FirstOrDefault(a => a.GetParameters().Count() == 1);

            return await Task.FromResult(info.Invoke(new object[] { TestFactory.GetAllByDate() }) as TableQuerySegment<DateEntity>);
        }
    }
}
