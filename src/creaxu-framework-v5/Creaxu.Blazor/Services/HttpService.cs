using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks.Sources;
using Creaxu.Blazor.Extensions;
using Creaxu.Shared;
using Microsoft.Extensions.Configuration;

namespace Creaxu.Blazor.Services
{
    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE,
        UPLOAD
    }

    public interface IHttpService
    {
        Task<ApiResponse> GetAsync(string requestUri);
        Task<ApiResponse> PostAsync(string requestUri, object data);
        Task<ApiResponse> PutAsync(string requestUri, object data);
        Task<ApiResponse> UploadAsync(string requestUri, string fileName, Stream content);
        Task<ApiResponse> DeleteAsync(string requestUri);

        Task<ApiResponse<T>> GetAsync<T>(string requestUri);
        Task<ApiResponse<T>> PostAsync<T>(string requestUri, object data);
        Task<ApiResponse<T>> PutAsync<T>(string requestUri, object data);
        Task<ApiResponse<T>> UploadAsync<T>(string requestUri, string fileName, Stream content);
        Task<ApiResponse<T>> DeleteAsync<T>(string requestUri);
    }

    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly IJwtAuthenticationStateProvider _jwtAuthenticationState;

        private const string _basePath = "api/{0}/";
        
        private JsonSerializerOptions defaultJsonSerializerOptions =>
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        public HttpService(HttpClient httpClient, IJwtAuthenticationStateProvider jwtAuthenticationState)
        {
            _httpClient = httpClient;
            _jwtAuthenticationState = jwtAuthenticationState;
        }
        
        public async Task<ApiResponse> GetAsync(string requestUri)
        {
            return await ExecuteAsync(requestUri, HttpMethod.GET);
        }

        public async Task<ApiResponse> PostAsync(string requestUri, object data)
        {
            return await ExecuteAsync(requestUri, HttpMethod.POST, data);
        }

        public async Task<ApiResponse> PutAsync(string requestUri, object data)
        {
            return await ExecuteAsync(requestUri, HttpMethod.PUT, data);
        }

        public async Task<ApiResponse> UploadAsync(string requestUri, string fileName, Stream content)
        {
            var multiContent = new MultipartFormDataContent();
            multiContent.Add(new StreamContent(content), "file", fileName);

            return await ExecuteAsync(requestUri, HttpMethod.UPLOAD, multiContent);
        }

        public async Task<ApiResponse> DeleteAsync(string requestUri)
        {
            return await ExecuteAsync(requestUri, HttpMethod.DELETE);
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string requestUri)
        {
            return await ExecuteAsync<T>(requestUri, HttpMethod.GET);
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string requestUri, object data)
        {
            return await ExecuteAsync<T>(requestUri, HttpMethod.POST, data);
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string requestUri, object data)
        {
            return await ExecuteAsync<T>(requestUri, HttpMethod.PUT, data);
        }

        public async Task<ApiResponse<T>> UploadAsync<T>(string requestUri, string fileName, Stream content)
        {
            var multiContent = new MultipartFormDataContent();
            multiContent.Add(new StreamContent(content), "file", fileName);

            return await ExecuteAsync<T>(requestUri, HttpMethod.UPLOAD, multiContent);
        }

        public async Task<ApiResponse<T>> DeleteAsync<T>(string requestUri)
        {
            return await ExecuteAsync<T>(requestUri, HttpMethod.DELETE);
        }

        private async Task<ApiResponse> ExecuteAsync(string requestUri, HttpMethod httpMethod, object data = null)
        {
            try
            {
                var httpResponseMessage = await GetHttpResponseMessage(requestUri, httpMethod, data);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return await Deserialize<ApiResponse>(httpResponseMessage, defaultJsonSerializerOptions);
                }
                else
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized) // token expired
                    {
                        await _jwtAuthenticationState.LogoutAsync();
                    }

                    var response = new ApiResponse();

                    response.SetError(httpResponseMessage.ReasonPhrase, await httpResponseMessage.Content.ReadAsStringAsync());

                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetAllMessages());

                var response = new ApiResponse();

                response.SetError("Unhandled Error");

                return response;
            }
        }

        private async Task<ApiResponse<T>> ExecuteAsync<T>(string requestUri, HttpMethod httpMethod, object data = null)
        {
            try
            {
                var httpResponseMessage = await GetHttpResponseMessage(requestUri, httpMethod, data);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return await Deserialize<ApiResponse<T>>(httpResponseMessage, defaultJsonSerializerOptions);
                }
                else
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized) // token expired
                    {
                        await _jwtAuthenticationState.LogoutAsync();
                    }

                    var response = new ApiResponse<T>();

                    response.SetError(httpResponseMessage.ReasonPhrase, await httpResponseMessage.Content.ReadAsStringAsync());

                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetAllMessages());

                var response = new ApiResponse<T>();

                response.SetError("Unhandled Error");

                return response;
            }
        }

        private async Task<HttpResponseMessage> GetHttpResponseMessage(string requestUri, HttpMethod httpMethod, object data = null)
        {
            requestUri = string.Format(_basePath, CultureInfo.CurrentCulture.Name) + requestUri;
            
            string dataJson = null;
            if (data != null && httpMethod != HttpMethod.UPLOAD)
            {
                dataJson = JsonSerializer.Serialize(data);
            }

            HttpResponseMessage httpResponseMessage;

            switch (httpMethod)
            {
                case HttpMethod.GET:
                    httpResponseMessage = await _httpClient.GetAsync(requestUri);
                    break;
                case HttpMethod.POST:
                    httpResponseMessage = await _httpClient.PostAsync(requestUri, new StringContent(dataJson, Encoding.UTF8, "application/json"));
                    break;
                case HttpMethod.PUT:
                    httpResponseMessage = await _httpClient.PutAsync(requestUri, new StringContent(dataJson, Encoding.UTF8, "application/json"));
                    break;
                case HttpMethod.UPLOAD:
                    httpResponseMessage = await _httpClient.PostAsync(requestUri, (HttpContent)data);
                    break;
                case HttpMethod.DELETE:
                    httpResponseMessage = await _httpClient.DeleteAsync(requestUri);
                    break;
                default:
                    throw new NotImplementedException($"Utility code not implemented for this Http method {httpMethod}");
            }

            return httpResponseMessage;
        }

        private static async Task<T> Deserialize<T>(HttpResponseMessage httpResponse, JsonSerializerOptions options)
        {
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            
            return !string.IsNullOrEmpty(responseString) ? JsonSerializer.Deserialize<T>(responseString, options) : default;
        }
    }
}
