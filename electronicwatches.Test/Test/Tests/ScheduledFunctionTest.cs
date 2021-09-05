using electronicwatches.functions.Function;
using electronicwatches.functions.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace electronicwatches.Test.Test.Tests
{
    public class ScheduledFunctionTest
    {

        [Fact]
        public void ScheduledFunction_Should_Log_Message()
        {
            MockCloudTableTodos mockRecordLogin = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableTodos mockRecordHour = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);

            _ = ScheduledFunction.Run(null, mockRecordLogin, mockRecordHour, logger);
            string message = logger.Logs[0];

            Assert.Contains("Scheduled function started successfully", message);
        }

    }
}
