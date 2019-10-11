// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitTest1.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the UnitTest1 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Southport.Extensions.HttpClient.Resilient.HttpClientExtensions;
using Xunit;

namespace Southport.ResilientHttp.Test
{

    public class UnitTest1
    {
        private HttpClient HttpClient { get; }
        private HttpMessageHandlerTestFixture MessageHandler { get; }
        public UnitTest1()
        {
            MessageHandler = new HttpMessageHandlerTestFixture();

            HttpClient = new HttpClient(MessageHandler);
        }

        [Fact]
        public async Task Test1()
        {
            var response = await HttpClient.SendResilientAsync("https://southport.solutions/test", HttpMethod.Get, CancellationToken.None);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
