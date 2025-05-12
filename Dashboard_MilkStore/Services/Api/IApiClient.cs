using Dashboard_MilkStore.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Services.Api
{
    public interface IApiClient
    {
        Task<T> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams = null, string token = null);
        Task<T> PostAsync<T>(string endpoint, object data, string token = null);
        Task<T> PutAsync<T>(string endpoint, object data, string token = null);
        Task<T> PatchAsync<T>(string endpoint, object data, string token = null);
        Task<T> DeleteAsync<T>(string endpoint, string token = null);
    }
}
