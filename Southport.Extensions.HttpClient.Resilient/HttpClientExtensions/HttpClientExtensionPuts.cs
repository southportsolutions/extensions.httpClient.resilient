// ***********************************************************************
// Assembly         : Southport.ResilientHttp
// Author           : Robert H Anstett
// Created          : 2019-10-11
//
// Last Modified By : Robert H Anstett
// Last Modified On : 2019-10-11
// ***********************************************************************
// <copyright file="HttpClientExtensionPuts.cs" company="Southport Solutions, LLC">
//     
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable MemberCanBePrivate.Global

namespace Southport.Extensions.HttpClient.Resilient.HttpClientExtensions
{
    /// <summary>
    /// Class HttpClientExtensions.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HttpClientExtensionPuts
    {

        #region PutResilient

        /// <summary>
        /// put resilient as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResponseType">The type to deserialize the JSON response.</typeparam>
        /// <typeparam name="TContentType">The content type of the object to be JSON serialized and sent.</typeparam>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="contentObject">The content object to be sent.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        /// <exception cref="ResilientHttpRequestException">There is no HTTP response or the response was not successful.</exception>
        public static async Task<TResponseType> PutResilientAsync<TResponseType, TContentType>(this System.Net.Http.HttpClient httpClient, string url, TContentType contentObject, string httpContentType = HttpMediaType.Json, Encoding encoding = null, int? maxRetries = null, List<HttpStatusCode> retryStatusCodes = null) where TResponseType : class where TContentType : class
        {
            return await PutResilientAsync<TResponseType, TContentType>(httpClient, url, contentObject, CancellationToken.None, httpContentType, encoding, maxRetries, retryStatusCodes);
        }

        /// <summary>
        /// put resilient as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TContentType">The content type of the object to be JSON serialized and sent.</typeparam>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="contentObject">The content object to be sent.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        /// <exception cref="ResilientHttpRequestException">There is no HTTP response.</exception>
        public static async Task<HttpResponseMessage> PutResilientAsync<TContentType>(this System.Net.Http.HttpClient httpClient, string url, TContentType contentObject, string httpContentType = HttpMediaType.Json, Encoding encoding = null, int? maxRetries = null, List<HttpStatusCode> retryStatusCodes = null) where TContentType : class
        {
            return await PutResilientAsync(httpClient, url, contentObject, CancellationToken.None, httpContentType, encoding, maxRetries, retryStatusCodes);
        }

        /// <summary>
        /// put resilient as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        /// <exception cref="ResilientHttpRequestException">There is no HTTP response.</exception>
        public static async Task<HttpResponseMessage> PutResilientAsync(this System.Net.Http.HttpClient httpClient, string url, int? maxRetries = null, List<HttpStatusCode> retryStatusCodes = null)
        {
            return await PutResilientAsync(httpClient, url, null, CancellationToken.None, maxRetries, retryStatusCodes);
        }

        /// <summary>
        /// put resilient as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="httpContentFunc">The HTTP content function.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        /// <exception cref="ResilientHttpRequestException">There is no HTTP response.</exception>
        public static async Task<HttpResponseMessage> PutResilientAsync(this System.Net.Http.HttpClient httpClient, string url, Func<HttpContent> httpContentFunc, int? maxRetries = null, List<HttpStatusCode> retryStatusCodes = null)
        {
            return await PutResilientAsync(httpClient, url, httpContentFunc, CancellationToken.None, maxRetries, retryStatusCodes);
        }

        /// <summary>
        /// put resilient as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResponseType">The type to deserialize the JSON response.</typeparam>
        /// <typeparam name="TContentType">The content type of the object to be JSON serialized and sent.</typeparam>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="contentObject">The content object to be sent.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="httpContentType"></param>
        /// <param name="encoding"></param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes"></param>
        /// <returns>Task&lt;T&gt;.</returns>
        /// <exception cref="ResilientHttpRequestException">There is no HTTP response or the response was not sccessful.</exception>
        public static async Task<TResponseType> PutResilientAsync<TResponseType, TContentType>(this System.Net.Http.HttpClient httpClient, string url, TContentType contentObject, CancellationToken cancellationToken, string httpContentType = HttpMediaType.Json, Encoding encoding = null, int? maxRetries = null, List<HttpStatusCode> retryStatusCodes = null) where TResponseType : class where TContentType : class
        {
            var response = await PutResilientAsync(httpClient, url, contentObject, cancellationToken, httpContentType, encoding, maxRetries, retryStatusCodes);
            return await HttpClientExtensions.ProcessResponse<TResponseType>(response, maxRetries);
        }

        /// <summary>
        /// put resilient as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TContentType">The content type of the object to be JSON serialized and sent.</typeparam>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="contentObject">The content object to be sent.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="httpContentType"></param>
        /// <param name="encoding"></param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes"></param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        /// <exception cref="ResilientHttpRequestException">There is no HTTP response.</exception>
        public static async Task<HttpResponseMessage> PutResilientAsync<TContentType>(this System.Net.Http.HttpClient httpClient, string url, TContentType contentObject, CancellationToken cancellationToken, string httpContentType = HttpMediaType.Json, Encoding encoding = null, int? maxRetries = null, List<HttpStatusCode> retryStatusCodes = null) where TContentType : class
        {
            var stringContentFunction = HttpClientExtensions.CreateStringContentFunction(contentObject, httpContentType, encoding);
            return await PutResilientAsync(httpClient, url, stringContentFunction, cancellationToken, maxRetries, retryStatusCodes);
        }

        /// <summary>
        /// put resilient as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes"></param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        /// <exception cref="ResilientHttpRequestException"></exception>
        public static async Task<HttpResponseMessage> PutResilientAsync(this System.Net.Http.HttpClient httpClient, string url, CancellationToken cancellationToken, int? maxRetries = null, List<HttpStatusCode> retryStatusCodes = null)
        {
            return await PutResilientAsync(httpClient, url, null, cancellationToken, maxRetries, retryStatusCodes);
        }

        /// <summary>
        /// put resilient as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="httpContentFunc">The HTTP content function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes"></param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        /// <exception cref="ResilientHttpRequestException">There is no HTTP response.</exception>
        public static async Task<HttpResponseMessage> PutResilientAsync(this System.Net.Http.HttpClient httpClient, string url, Func<HttpContent> httpContentFunc, CancellationToken cancellationToken, int? maxRetries = null, List<HttpStatusCode> retryStatusCodes = null)
        {
            return await HttpClientExtensions.SendResilientAsync(httpClient, url, httpContentFunc, HttpMethod.Put, cancellationToken, maxRetries, retryStatusCodes);
        }

        #endregion

    }
}
