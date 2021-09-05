using electronicwatches.functions.Entities;
using electronicwatches.functions.Function;
using electronicwatches.functions.Test.Helpers;
using electronicwatches.Test.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Xunit;

namespace electronicwatches.functions.Test.Tests
{
    public class timeApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateTime_Should_Return_200()
        {
            // Arrenge
            MockCloudTableTodos mockTime = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            TimeEntity timeRequest = TestFactory.GetTodoRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeRequest);

            // Act
            IActionResult response = await TimeApi.CreateTime(request, mockTime, logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }


        [Fact]
        public async void UpdateTime_Should_Return_200()
        {
            // Arrenge
            MockCloudTableTodos mockTime = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            TimeEntity timeRequest = TestFactory.GetTodoRequest();
            Guid todoId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId, timeRequest);

            // Act
            IActionResult response = await TimeApi.updateTime(request, mockTime, todoId.ToString(), logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

         [Fact]
         public async void GetTime_Should_Return_200()
         {
            MockCloudTableTodos mockTime = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports/?comp=TIME"));
            TimeEntity requestentity = TestFactory.GettimeEntity();
            HttpRequest request = TestFactory.CreateHttpRequest(requestentity);

          
            IActionResult response = await TimeApi.Getall(request, mockTime, logger);

             var result = (OkObjectResult)response;
             Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

         }


        [Fact]
        public async void Delete_Should_Return_200()
        {
            MockCloudTableTodos mockTime = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            TimeEntity timeRequest = TestFactory.GettimeEntity();

            HttpRequest request = TestFactory.CreateHttpRequestforDelete(timeRequest.RowKey, timeRequest);
            
            IActionResult response = await TimeApi.DeletebyId(request, timeRequest, mockTime,  timeRequest.Id.ToString(),logger);

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void GetbyId_Should_Return_200()
        {
           TimeEntity timeRequest = TestFactory.GetTodoRequest();
            Guid timeId = Guid.NewGuid();
            HttpRequest request = TestFactory.CreateHttpRequest(timeId, timeRequest);

            IActionResult response = await TimeApi.GetbyId(request, timeRequest, timeId.ToString(), logger);

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }


        [Fact]
        public async void GetbyDate_Should_Return_200()
        {
            MockTableDate mockTime = new MockTableDate(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            DateEntity dateRequest = TestFactory.GetDateRequest();

            HttpRequest request = TestFactory.CreateHttpRequest(dateRequest);

            IActionResult response = await TimeApi.GetbyDate(request, mockTime, dateRequest.DateWorked, logger);

            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }


    }
}
