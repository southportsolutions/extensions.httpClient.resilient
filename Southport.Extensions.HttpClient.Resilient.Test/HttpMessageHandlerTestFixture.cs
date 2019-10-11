using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Southport.ResilientHttp.Test
{
    public class HttpMessageHandlerTestFixture : HttpMessageHandler
    {
        public HttpContent Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public HttpMessageHandlerTestFixture()
        {
            StatusCode = HttpStatusCode.OK;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return new HttpResponseMessage(statusCode: StatusCode) {Content = Content};
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