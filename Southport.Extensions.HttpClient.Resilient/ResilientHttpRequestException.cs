using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Southport.Extensions.HttpClient.Resilient
{
    public class ResilientHttpRequestException : HttpRequestException
    {

        public ResilientHttpRequestException(HttpResponseMessage response, string message) : this(message)
        {
            StatusCode = response.StatusCode;
            Url = response.RequestMessage.RequestUri;
        }

        public ResilientHttpRequestException(HttpResponseMessage response, string message, List<DateTime> tryTimes) : this(message, tryTimes)
        {
            StatusCode = response.StatusCode;
            Url = response.RequestMessage.RequestUri;
        }
        public ResilientHttpRequestException(HttpResponseMessage response, string message, Exception inner) : this(message, inner)
        {
            StatusCode = response.StatusCode;
            Url = response.RequestMessage.RequestUri;
        }
        public ResilientHttpRequestException(HttpResponseMessage response, string message, List<DateTime> tryTimes, Exception inner) : this(message, tryTimes, inner)
        {
            StatusCode = response.StatusCode;
            Url = response.RequestMessage.RequestUri;
        }

        public ResilientHttpRequestException(string message) : base(message) { }
        public ResilientHttpRequestException(string message, List<DateTime> requestSendTimes) : this(message)
        {
            RequestSendTimes = requestSendTimes;
        }
        public ResilientHttpRequestException(string message, Exception inner) : base(message, inner) { }
        public ResilientHttpRequestException(string message, List<DateTime> requestSendTimes, Exception inner) : this(message, inner)
        {
            RequestSendTimes = requestSendTimes;
        }

        public List<DateTime> RequestSendTimes { get; }
        public HttpStatusCode StatusCode { get; }
        public Uri Url { get; }

        public override string Message
        {
            get
            {
                var message = base.Message;

                var sendTimeString = "";
                if (Url != null)
                {
                    message = $"{message}{Environment.NewLine}URL:{Url.AbsoluteUri}";
                }

                message = $"{message}{Environment.NewLine}Status Code: {StatusCode}";

                if (RequestSendTimes != null)
                {
                    foreach (var requestSendTime in RequestSendTimes)
                    {
                        sendTimeString = $"{Environment.NewLine}{requestSendTime:O}";
                    }
                    message = $"{message}{Environment.NewLine}Request Send Times:{sendTimeString}";
                }

                return message;
            }
        }
    }
}