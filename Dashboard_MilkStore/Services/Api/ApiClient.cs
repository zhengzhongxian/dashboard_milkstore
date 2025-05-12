using Dashboard_MilkStore.CoreHelpers;
using Dashboard_MilkStore.Models.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Dashboard_MilkStore.Services.Api
{
    public class ApiClient : IApiClient
    {
        private readonly CallAPI _callAPI;
        private readonly ILogger<ApiClient> _logger;
        private readonly string _baseUrl;

        public ApiClient(CallAPI callAPI, IConfiguration configuration, ILogger<ApiClient> logger)
        {
            _callAPI = callAPI;
            _logger = logger;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://milkstore-grbpfnduezbpgvgc.eastasia-01.azurewebsites.net";
        }

        public async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams = null, string token = null)
        {
            try
            {
                var url = BuildUrl(endpoint, queryParams);
                _logger.LogInformation($"GET request to {url}");
                return await _callAPI.GetAsync<T>(url, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GET request to {endpoint}");
                throw;
            }
        }

        public async Task<T> PostAsync<T>(string endpoint, object data, string token = null)
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint}";
                _logger.LogInformation($"POST request to {url}");
                return await _callAPI.PostAsync<T>(url, data, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in POST request to {endpoint}");
                throw;
            }
        }

        public async Task<T> PutAsync<T>(string endpoint, object data, string token = null)
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint}";
                _logger.LogInformation($"PUT request to {url}");
                return await _callAPI.PutAsync<T>(url, data, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in PUT request to {endpoint}");
                throw;
            }
        }

        public async Task<T> PatchAsync<T>(string endpoint, object data, string token = null)
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint}";
                _logger.LogInformation($"PATCH request to {url}");
                return await _callAPI.PatchAsync<T>(url, data, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in PATCH request to {endpoint}");
                throw;
            }
        }

        public async Task<T> DeleteAsync<T>(string endpoint, string token = null)
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint}";
                _logger.LogInformation($"DELETE request to {url}");
                return await _callAPI.DeleteAsync<T>(url, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in DELETE request to {endpoint}");
                throw;
            }
        }

        private string BuildUrl(string endpoint, Dictionary<string, string> queryParams)
        {
            var url = $"{_baseUrl}/{endpoint}";

            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", queryParams.Select(p => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value)}"));
                url = $"{url}?{queryString}";
            }

            return url;
        }
    }
}
