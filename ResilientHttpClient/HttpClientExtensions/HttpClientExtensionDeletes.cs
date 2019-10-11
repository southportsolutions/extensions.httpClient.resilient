// ***********************************************************************
// Assembly         : Southport.ResilientHttp
// Author           : Robert H Anstett
// Created          : 2019-10-11
//
// Last Modified By : Robert H Anstett
// Last Modified On : 2019-10-11
// ***********************************************************************
// <copyright file="HttpClientExtensionDeletes.cs" company="Southport Solutions, LLC">
//     
// </copyright>
// <summary></summary>
// ***********************************************************************



// ReSharper disable MemberCanBePrivate.Global

namespace Southport.ResilientHttp
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Class HttpClientExtensions.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HttpClientExtensionDeletes
    {
        #region Delete

                /// <summary>
        /// delete resilient as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes">The retry status codes.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        public static async Task<HttpResponseMessage> DeleteResilientAsync(this HttpClient httpClient, string url, int maxRetries = 2, List<HttpStatusCode> retryStatusCodes = null)
        {
            return await DeleteResilientAsync(httpClient, url, CancellationToken.None, maxRetries, retryStatusCodes);
        }

        /// <summary>
        /// delete resilient as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResponseType">The type to deserialize the JSON response.</typeparam>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes">The retry status codes.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public static async Task<TResponseType> DeleteResilientAsync<TResponseType>(this HttpClient httpClient, string url, int maxRetries = 2, List<HttpStatusCode> retryStatusCodes = null) where TResponseType : class
        {
            return await DeleteResilientAsync<TResponseType>(httpClient, url, CancellationToken.None, maxRetries: maxRetries, retryStatusCodes: retryStatusCodes);
        }

        /// <summary>
        /// delete resilient as an asynchronous operation.
        /// </summary>
        /// <typeparam name="TResponseType">The type to deserialize the JSON response.</typeparam>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="converter">The customer deserialization converter</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes">The retry status codes.</param>
        /// <returns>Task&lt;TResponseType&gt;.</returns>
        public static async Task<TResponseType> DeleteResilientAsync<TResponseType>(this HttpClient httpClient, string url, CancellationToken cancellationToken, ISouthportConvert converter = null, int maxRetries = 2, List<HttpStatusCode> retryStatusCodes = null) where TResponseType : class
        {
            var response = await DeleteResilientAsync(httpClient, url, cancellationToken, maxRetries, retryStatusCodes);
            return await HttpClientExtensions.ProcessResponse<TResponseType>(response, maxRetries);
        }

        /// <summary>
        /// delete resilient as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="url">The URL of the host to send the request to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="maxRetries">The maximum retries.</param>
        /// <param name="retryStatusCodes">The retry status codes.</param>
        /// <returns>Task&lt;HttpResponseMessage&gt;.</returns>
        /// <exception cref="ResilientHttpRequestException">There is no HTTP response.</exception>
        public static async Task<HttpResponseMessage> DeleteResilientAsync(this HttpClient httpClient, string url, CancellationToken cancellationToken, int maxRetries = 2, List<HttpStatusCode> retryStatusCodes = null)
        {
            return await HttpClientExtensions.SendResilientAsync(httpClient, url, HttpMethod.Delete, cancellationToken, maxRetries, retryStatusCodes);
        }

        #endregion
    }
}
