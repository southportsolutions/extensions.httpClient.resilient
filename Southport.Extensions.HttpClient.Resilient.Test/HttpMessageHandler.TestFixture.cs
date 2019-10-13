using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Southport.Extensions.HttpClient.Resilient.Test
{
    public class HttpMessageHandlerTestFixture : HttpMessageHandler
    {
        public HttpContent Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public Exception Exception { get; set; }

        public HttpMessageHandlerTestFixture()
        {
            StatusCode = HttpStatusCode.OK;
        }

#pragma warning disable 1998
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
#pragma warning restore 1998
        {
            if (Exception != null)
            {
                throw Exception;
            }

            return new HttpResponseMessage(StatusCode) {Content = Content};
        }

        public void SetResponseContent(HttpContent content)
        {
            Content = content;
        }

        public void SetResponseContent(string content, string mediaType)
        {
            Content= new StringContent(content, Encoding.UTF8, mediaType);
        }

        public void SetResponseStatusCode(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }
}