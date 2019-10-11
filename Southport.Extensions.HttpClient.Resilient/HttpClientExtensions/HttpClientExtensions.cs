// ***********************************************************************
// Assembly         : Southport.ResilientHttp
// Author           : Robert H Anstett
// Created          : 2019-10-11
//
// Last Modified By : Robert H Anstett
// Last Modified On : 2019-10-11
// ***********************************************************************
// <copyright file="HttpClientExtensions.cs" company="Southport Solutions, LLC">
//     
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Southport.Extensions.HttpClient.Resilient.Converters;

// ReSharper disable MemberCanBePrivate.Global

namespace Southport.Extensions.HttpClient.Resilient.HttpClientExtensions
{
    /// <summary>
    /// Class HttpClientExtensions.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HttpClientExtensions
    {
        /// <summary>
        /// The random
        /// </summary>
        private static readonly Random Random = new Random();
        /// <summary>
        /// Gets the long sleep milliseconds.
        /// </summary>
        /// <value>The long sleep milliseconds.</value>
        private static int LongSleepMilliseconds => Random.Next(750, 1500);

        /// <summary>
        /// Gets the short sleep milliseconds.
        /// </summary>
        /// <value>The short sleep milliseconds.</value>
        private static int ShortSleepMilliseconds => Random.Next(250, 750);

        public static readonly List<HttpStatusCode> RetryStatusCodes = new List<HttpStatusCode> {HttpStatusCode.RequestTimeout};

        #region Send

        /// <summary>
        /// Send resilient as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="method">The HTTP Method (Verb)</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes">Status code to force a retry.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        /// <exception cref="ResilientHttpRequestException">There is no HTTP response.</exception>
        public static async Task<HttpResponseMessage> SendResilientAsync(this System.Net.Http.HttpClient httpClient, string url, HttpMethod method, CancellationToken cancellationToken, int maxRetries = 2, List<HttpStatusCode> retryStatusCodes = null)
        {
            return await SendResilientAsync(httpClient, ()=>new HttpRequestMessage(method, url), cancellationToken, maxRetries, retryStatusCodes);
        }

        /// <summary>
        /// Send resilient as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="httpContentFunc">The HTTP content function.</param>
        /// <param name="method">The HTTP Method (Verb)</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes"></param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        /// <exception>There is no HTTP response.</exception>
        public static async Task<HttpResponseMessage> SendResilientAsync(this System.Net.Http.HttpClient httpClient, string url, Func<HttpContent> httpContentFunc, HttpMethod method, CancellationToken cancellationToken, int maxRetries = 2, List<HttpStatusCode> retryStatusCodes = null)
        {
            var httpRequestMessageFunc = new Func<HttpRequestMessage>(delegate
            {
                var message = new HttpRequestMessage(method, url) {Content = httpContentFunc()};
                return message;
            });
                
            return await SendResilientAsync(httpClient, httpRequestMessageFunc, cancellationToken, maxRetries, retryStatusCodes);
        }

        /// <summary>
        /// Send resilient as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="httpRequestMessageFunc">The HTTP request message function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes">The HTTP status code to force a retry.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        /// <exception cref="NullReferenceException">The HttpRequestMessageFunction cannot be null.</exception>
        /// <exception cref="ResilientHttpRequestException">There is no HTTP response.</exception>
        public static async Task<HttpResponseMessage> SendResilientAsync(this System.Net.Http.HttpClient httpClient, Func<HttpRequestMessage> httpRequestMessageFunc, CancellationToken cancellationToken, int maxRetries = 2, List<HttpStatusCode> retryStatusCodes = null)
        {
            var retries = 0;
            retryStatusCodes = retryStatusCodes ?? RetryStatusCodes;
            HttpResponseMessage response = null;
            var requestSendTimes = new List<DateTime>();
            while (retries < maxRetries)
            {
                retries++;
                if (httpRequestMessageFunc == null)
                {
                    throw new NullReferenceException("The HttpRequestMessageFunction cannot be null.");
                }

                var request = httpRequestMessageFunc();
                try
                {
                    requestSendTimes.Add(DateTime.UtcNow);
                    response = await httpClient.SendAsync(request, cancellationToken);
                }
                catch (HttpRequestException e)
                {
                    if (e.InnerException != null)
                    {
                        if (e.InnerException is WebException webException && webException.Status == WebExceptionStatus.ConnectFailure && retries < maxRetries)
                        {
                            Thread.Sleep(retries * LongSleepMilliseconds);
                        }
                    }

                    throw new ResilientHttpRequestException($"There was an exception at URL: {request.RequestUri.OriginalString}, Verb: {request.Method}.", e);
                }
                catch (TaskCanceledException e)
                {
                    if (e.CancellationToken.IsCancellationRequested)
                    {
                        throw new ResilientHttpRequestException($"The HTTP request was canceled for URL: {request.RequestUri.OriginalString}, Verb: {request.Method}.", e);
                    }

                    throw new ResilientHttpRequestException($"The HTTP request timed out for URL: {request.RequestUri.OriginalString}, Verb: {request.Method}.", e);
                }

                if (response.IsSuccessStatusCode 
                    || response.StatusCode == HttpStatusCode.NotFound 
                    || retryStatusCodes.Contains(response.StatusCode) == false
                    || retries >= maxRetries)
                {
                    return response;
                }

                Thread.Sleep(retries * ShortSleepMilliseconds);
            }

            if (response == null)
            {
                throw new ResilientHttpRequestException("There is no HTTP response.");
            }

            await ThrowResponseException(response, maxRetries, requestSendTimes);
            return null;
        }
        #endregion

        /// <summary>
        /// Throws the response exception.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="requestSendTimes">The request send times.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ResilientHttpRequestException">
        /// HTTP response was not successful, retried {maxRetries} {Environment.NewLine}{Environment.NewLine} Response Status Code:{response.StatusCode}{Environment.NewLine}Version:{response.Version}{Environment.NewLine} Response Message:{Environment.NewLine}{Environment.NewLine}{messageString}
        /// or
        /// HTTP response was not successful, retried {maxRetries} {Environment.NewLine}{Environment.NewLine} Response Message:{Environment.NewLine}{Environment.NewLine}UNABLE TO GET CONTENT FAILED WITH ERROR:{Environment.NewLine}{Environment.NewLine}{e.Message}{Environment.NewLine}Stacktrace:{Environment.NewLine}{e.StackTrace}
        /// </exception>
        internal static async Task ThrowResponseException(HttpResponseMessage response, int maxRetries, List<DateTime> requestSendTimes = null)
        {
            try
            {
                var messageString = await response.Content.ReadAsStringAsync();
                throw new ResilientHttpRequestException(response, $"HTTP response was not successful, retried {maxRetries} {Environment.NewLine}{Environment.NewLine} Response Status Code:{response.StatusCode}{Environment.NewLine}Version:{response.Version}{Environment.NewLine} Response Message:{Environment.NewLine}{Environment.NewLine}{messageString}", requestSendTimes);
            }
            catch (Exception e)
            {
                throw new ResilientHttpRequestException(response, $"HTTP response was not successful, retried {maxRetries} {Environment.NewLine}{Environment.NewLine} Response Message:{Environment.NewLine}{Environment.NewLine}UNABLE TO GET CONTENT FAILED WITH ERROR:{Environment.NewLine}{Environment.NewLine}{e.Message}{Environment.NewLine}Stacktrace:{Environment.NewLine}{e.StackTrace}", requestSendTimes);
            }
        }

        internal static async Task<TResponseType> ProcessResponse<TResponseType>(HttpResponseMessage response, int maxRetries)
        {
            if (!response.IsSuccessStatusCode)
            {

                await ThrowResponseException(response, maxRetries);
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.GetValues("Content-Type").First();
            return DeserializeResponse<TResponseType>(responseString, contentType);
        }

        /// <summary>
        /// Serialize the request content.
        /// </summary>
        /// <param name="contentType">The type of the content object to be added to the HttpContent.</param>
        /// <param name="contentObject">The content to be added to the HttpContent</param>
        /// <param name="encoding">The coding of the values in the string content, when no value is set the default is UTF8.</param>
        /// <param name="converter">Custom converter that implements the ISouthportConvert interface.</param>
        /// <returns>TResponse.</returns>
        /// <exception cref="Exception">Unable to deserialize exception. {Environment.NewLine}{Environment.NewLine}Content: {content}</exception>
        internal static Func<HttpContent> CreateStringContentFunction<TContentType>(TContentType contentObject, string contentType = HttpMediaType.Json, Encoding encoding = null, ISouthportConvert converter = null)
        {
            encoding = encoding ?? Encoding.UTF8;

            if (converter != null)
            {
                return () => new StringContent(converter.SerializeObject(contentObject), encoding, contentType);
            }

            switch (contentType)
            {
                case HttpMediaType.Json:
                    return () => new StringContent(JsonConvert.SerializeObject(contentObject), encoding, contentType);
                case HttpMediaType.Xml:
                    return () => new StringContent(XmlConvert.SerializeObject(contentObject), encoding, contentType);
            }

            if (contentObject is string)
            {
                return () => new StringContent(contentObject as string, encoding, contentType);
            }

            throw new Exception("Unable to determine the contentObject's serialization and it is not a string.'");
        }

        /// <summary>
        /// Deserializes the response.
        /// </summary>
        /// <typeparam name="TResponse">The type of the t response.</typeparam>
        /// <param name="contentString">The content string to deserialize.</param>
        /// <param name="contentType">The content type.</param>
        /// <param name="converter">The custom converter to deserialize the string.</param>
        /// <returns>TResponse.</returns>
        /// <exception cref="Exception">Unable to deserialize exception. {Environment.NewLine}{Environment.NewLine}Content: {content}</exception>
        internal static TResponse DeserializeResponse<TResponse>(string contentString, string contentType = HttpMediaType.Json, ISouthportConvert converter = null)
        {
            if (converter != null)
            {
                return converter.DeserializeObject<TResponse>(contentString);
            }

            switch (contentType)
            {
                case HttpMediaType.Json:
                    return JsonConvert.DeserializeObject<TResponse>(contentString);
                case HttpMediaType.Xml:
                    return XmlConvert.DeserializeObject<TResponse>(contentString);
            }

            throw new Exception("Unable to deserialize contentString.");
        }
    }
}
