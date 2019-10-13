// ***********************************************************************
// Assembly         : Southport.ResilientHttp
// Author           : Robert H Anstett
// Created          : 2019-10-11
//
// Last Modified By : Robert H Anstett
// Last Modified On : 10-11-2019
// ***********************************************************************
// <copyright file="HttpClientExtensionGets.cs" company="Southport Solutions, LLC">
//     
// </copyright>
// <summary></summary>
// ***********************************************************************


using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable MemberCanBePrivate.Global

namespace Southport.Extensions.HttpClient.Resilient.HttpClientExtensions
{

    /// <summary>
    /// Class HttpClientExtensions.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HttpClientExtensionGets
    {
        #region GetResilient

        /// <summary>
        /// get resilient as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes">The retry status codes.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        public static async Task<HttpResponseMessage> GetResilientAsync(this System.Net.Http.HttpClient httpClient, string url, int? maxRetries = null, List<HttpStatusCode> retryStatusCodes = null)
        {
            return await GetResilientAsync(httpClient, url, CancellationToken.None, maxRetries, retryStatusCodes);
        }

        /// <summary>
        /// get resilient as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResponseType">The type to deserialize the JSON response.</typeparam>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes">The retry status codes.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public static async Task<TResponseType> GetResilientAsync<TResponseType>(this System.Net.Http.HttpClient httpClient, string url, int? maxRetries = null, List<HttpStatusCode> retryStatusCodes = null) where TResponseType : class
        {
            return await GetResilientAsync<TResponseType>(httpClient, url, CancellationToken.None, maxRetries, retryStatusCodes);
        }

        /// <summary>
        /// get resilient as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResponseType">The type to deserialize the JSON response.</typeparam>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes">The retry status codes.</param>
        /// <returns>Task&lt;TResponseType&gt;.</returns>
        public static async Task<TResponseType> GetResilientAsync<TResponseType>(this System.Net.Http.HttpClient httpClient, string url, CancellationToken cancellationToken, int? maxRetries = null, List<HttpStatusCode> retryStatusCodes = null) where TResponseType : class
        {
            var response = await GetResilientAsync(httpClient, url, cancellationToken, maxRetries, retryStatusCodes);
            return await HttpClientExtensions.ProcessResponse<TResponseType>(response, maxRetries);
        }

        /// <summary>
        /// get resilient as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes">The retry status codes.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        /// <exception cref="ResilientHttpRequestException">There is no HTTP response.</exception>
        public static async Task<HttpResponseMessage> GetResilientAsync(this System.Net.Http.HttpClient httpClient, string url, CancellationToken cancellationToken, int? maxRetries = null, List<HttpStatusCode> retryStatusCodes = null)
        {
            return await HttpClientExtensions.SendResilientAsync(httpClient, url, HttpMethod.Get, cancellationToken, maxRetries, retryStatusCodes);
        }

        #endregion
    }
}
