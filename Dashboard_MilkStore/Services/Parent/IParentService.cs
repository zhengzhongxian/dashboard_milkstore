using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Parent;

namespace Dashboard_MilkStore.Services.Parent
{
    public interface IParentService
    {
        Task<ServiceResponse<List<Models.Parent.Parent>>> GetParentsAsync(string? token = null);
        Task<ServiceResponse<Models.Parent.Parent>> GetParentByIdAsync(string id, string? token = null);
    }
}
