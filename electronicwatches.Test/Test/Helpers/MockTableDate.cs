using electronicwatches.functions.Test.Helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
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
    }
}
