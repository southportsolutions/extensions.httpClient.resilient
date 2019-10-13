// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTest1.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the UnitTest1 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Southport.Extensions.HttpClient.Resilient.Converters;
using Southport.Extensions.HttpClient.Resilient.HttpClientExtensions;
using Xunit;

namespace Southport.Extensions.HttpClient.Resilient.Test
{

    public class HttpClientExtensionsTests : IClassFixture<HttpMessageHandlerTestFixture>
    {
        private System.Net.Http.HttpClient HttpClient { get; set; }
        private HttpMessageHandlerTestFixture MessageHandler { get; set; }

        private const string Url = "https://southport.solutions/test";

        public HttpClientExtensionsTests()
        {
            Reset();
            //HttpClient = new System.Net.Http.HttpClient(MessageHandler);
        }

        private void Reset()
        {
            MessageHandler = new HttpMessageHandlerTestFixture();
            HttpClient = new System.Net.Http.HttpClient(MessageHandler);
        }

        #region SendAsync

        [Fact]
        public async Task SendAsync_MaxRetries_0()
        {
            Reset();
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => HttpClient.SendResilientAsync("https://southport.solutions/test", HttpMethod.Get, CancellationToken.None, 0));
        }

        [Fact]
        public async Task SendAsync_httpRequestMessageFunc_null()
        {
            Reset();
            await Assert.ThrowsAsync<NullReferenceException>(() => HttpClient.SendResilientAsync(null, CancellationToken.None));
        }

        [Fact]
        public async Task SendAsync_ResilientHttpRequestException_Unknown()
        {
            Reset();
            MessageHandler.Exception = new HttpRequestException("test");
            var exception = await Assert.ThrowsAsync<ResilientHttpRequestException>(() => HttpClient.SendResilientAsync(Url, HttpMethod.Get, CancellationToken.None));
            Assert.Single(exception.RequestSendTimes);
            Assert.Equal(Url, exception.Url.OriginalString);
            Assert.StartsWith("There was an exception at URL:", exception.Message);
        }

        [Fact]
        public async Task SendAsync_ResilientHttpRequestException_ConnectionFailure()
        {
            Reset();
            MessageHandler.Exception = new HttpRequestException("test", new WebException("Test web exception", WebExceptionStatus.ConnectFailure));
            var exception = await Assert.ThrowsAsync<ResilientHttpRequestException>(() => HttpClient.SendResilientAsync(Url, HttpMethod.Get, CancellationToken.None));
            Assert.Equal(2, exception.RequestSendTimes.Count);
            Assert.Equal(Url, exception.Url.OriginalString);
            Assert.StartsWith("There was an exception at URL:", exception.Message);
        }

        [Fact]
        public async Task SendAsync_TaskCanceledException_Canceled()
        {
            Reset();
            var cancellationToken = new CancellationToken(true);
            MessageHandler.Exception = new TaskCanceledException("test", new Exception("test exception"), cancellationToken);
            var exception = await Assert.ThrowsAsync<ResilientHttpRequestException>(() => HttpClient.SendResilientAsync(Url, HttpMethod.Get, CancellationToken.None));
            Assert.Single(exception.RequestSendTimes);
            Assert.Equal(Url, exception.Url.OriginalString);
            Assert.StartsWith("The HTTP request was canceled for URL:", exception.Message);
        }

        [Fact]
        public async Task SendAsync_TaskCanceledException_TimedOut()
        {
            Reset();
            MessageHandler.Exception = new TaskCanceledException("test", new Exception("test exception"));
            var exception = await Assert.ThrowsAsync<ResilientHttpRequestException>(() => HttpClient.SendResilientAsync(Url, HttpMethod.Get, CancellationToken.None));
            Assert.Single(exception.RequestSendTimes);
            Assert.Equal(Url, exception.Url.OriginalString);
            Assert.StartsWith("The HTTP request timed out for URL:", exception.Message);
        }

        [Fact]
        public async Task SendAsync_ResponseNotSuccessStatusCode_StatusCodeInRetryList_RequestTimedOut()
        {
            Reset();
            MessageHandler.SetResponseStatusCode(HttpStatusCode.RequestTimeout);
            var response = await HttpClient.SendResilientAsync(Url, HttpMethod.Get, CancellationToken.None);
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.RequestTimeout, response.StatusCode);
        }

        [Fact]
        public async Task SendAsync_Response_NotSuccessStatusCode_NotFound()
        {
            Reset();
            MessageHandler.SetResponseStatusCode(HttpStatusCode.NotFound);
            var response = await HttpClient.SendResilientAsync(Url, HttpMethod.Get, CancellationToken.None);
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SendAsync_Response_SuccessStatusCode_Ok()
        {
            Reset();
            MessageHandler.SetResponseStatusCode(HttpStatusCode.OK);
            var response = await HttpClient.SendResilientAsync(Url, HttpMethod.Get, CancellationToken.None);
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        #endregion

        #region CreateStringContentFunction

        [Fact]
        public async Task CreateStringContentFunction_String()
        {
            const string @string = "This is a test string.";
            var contentFunc = HttpClientExtensions.HttpClientExtensions.CreateStringContentFunction(@string, HttpMediaType.Plain);
            var content = contentFunc();
            var contentString = await content.ReadAsStringAsync();
            Assert.Equal(contentString, @string);
            Assert.Equal(HttpMediaType.Plain, content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task CreateStringContentFunction_Json()
        {
            var @object = new List<string> {"test string1", "test string2"};
            var contentFunc = HttpClientExtensions.HttpClientExtensions.CreateStringContentFunction(@object, HttpMediaType.Json);
            var content = contentFunc();
            var contentString = await content.ReadAsStringAsync();
            var @string = JsonConvert.SerializeObject(@object);
            Assert.Equal(contentString, @string);
            Assert.Equal(HttpMediaType.Json, content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task CreateStringContentFunction_Xml()
        {
            var @object = new List<string> {"test string1", "test string2"};
            var contentFunc = HttpClientExtensions.HttpClientExtensions.CreateStringContentFunction(@object, HttpMediaType.Xml);
            var content = contentFunc();
            var contentString = await content.ReadAsStringAsync();
            var @string = XmlConvert.SerializeObject(@object);
            Assert.Equal(contentString, @string);
            Assert.Equal(HttpMediaType.Xml, content.Headers.ContentType.MediaType);
        }

        [Fact]
        public async Task CreateStringContentFunction_TestConverter()
        {
            var @object = new List<string> {"test string1", "test string2"};
            var contentType = "text/testContent";
            var converter = new TestConverter();
            var contentFunc = HttpClientExtensions.HttpClientExtensions.CreateStringContentFunction(@object, contentType, converter: converter);
            var content = contentFunc();
            var contentString = await content.ReadAsStringAsync();
            var @string = converter.SerializeObject(@object);
            Assert.Equal(contentString, @string);
            Assert.Equal(contentType, content.Headers.ContentType.MediaType);
        }

        [Fact]
        public void CreateStringContentFunction_NullContentType()
        {
            var @object = new List<string> {"test string1", "test string2"};
            var exception = Assert.Throws<Exception>(() => HttpClientExtensions.HttpClientExtensions.CreateStringContentFunction(@object, null));
            Assert.Equal("Unable to determine the contentObject's serialization and it is not a string.", exception.Message);
        }

        #endregion

        #region DeserializeResponse

        [Fact]
        public void DeserializeResponse_String()
        {
            const string @string = "This is a test string.";
            var content = HttpClientExtensions.HttpClientExtensions.DeserializeResponse<string>(@string, HttpMediaType.Plain);
            Assert.Equal(content, @string);
        }

        [Fact]
        public void DeserializeResponse_Json()
        {
            var @object = new List<string> {"test string1", "test string2"};
            var @string = JsonConvert.SerializeObject(@object);
            var content = HttpClientExtensions.HttpClientExtensions.DeserializeResponse<List<string>>(@string);
            Assert.Equal(@object, content);
        }

        [Fact]
        public void DeserializeResponse_Xml()
        {
            var @object = new List<string> {"test string1", "test string2"};
            var @string = XmlConvert.SerializeObject(@object);
            var content = HttpClientExtensions.HttpClientExtensions.DeserializeResponse<List<string>>(@string, HttpMediaType.Xml);
            Assert.Equal(@object, content);
        }

        [Fact]
        public void DeserializeResponse_TestConverter()
        {
            var @object = new List<string> {"test string1", "test string2"};
            var testConverter = new TestConverter();
            var @string = testConverter.SerializeObject(@object);
            var content = HttpClientExtensions.HttpClientExtensions.DeserializeResponse<List<string>>(@string, null, testConverter);
            Assert.Equal(@object, content);
        }

        [Fact]
        public void DeserializeResponse_NullContentType()
        {
            var @object = new List<string> {"test string1", "test string2"};
            var @string = JsonConvert.SerializeObject(@object);
            var exception = Assert.Throws<Exception>(() => HttpClientExtensions.HttpClientExtensions.DeserializeResponse<List<string>>(@string, null));
            Assert.Equal("Unable to deserialize contentString.", exception.Message);
        }

        #endregion
    }
}
