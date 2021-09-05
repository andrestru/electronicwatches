using electronicwatches.functions.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace electronicwatches.functions.Test.Helpers
{
    public class MockCloudTableTodos : CloudTable
    {
        public MockCloudTableTodos(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockCloudTableTodos(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableTodos(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }

        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = 200,
                Result = TestFactory.GettimeEntity()
            });
        }


        public override async Task<TableQuerySegment<TimeEntity>> ExecuteQuerySegmentedAsync<TimeEntity>(TableQuery<TimeEntity> query, TableContinuationToken token)
        {
            ConstructorInfo info = typeof(TableQuerySegment<TimeEntity>)
                 .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                 .FirstOrDefault(a => a.GetParameters().Count() == 1);

            return await Task.FromResult(info.Invoke(new object[] { TestFactory.GetAll() }) as TableQuerySegment<TimeEntity>);
        }
    }
}
