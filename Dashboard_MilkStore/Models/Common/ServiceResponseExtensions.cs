using System.Net;

namespace Dashboard_MilkStore.Models.Common
{
    public static class ServiceResponseExtensions
    {
        public static ServiceResponse<T> SuccessResponse<T>(this ServiceResponse<T> _, T data, string message = "")
        {
            return new ServiceResponse<T>
            {
                Data = data,
                Success = true,
                Message = message,
                StatusCode = 200,
                IsSuccessStatusCode = true
            };
        }

        public static ServiceResponse<T> SuccessResponse<T>(this ServiceResponse<T> _, string message = "")
        {
            return new ServiceResponse<T>
            {
                Success = true,
                Message = message,
                StatusCode = 200,
                IsSuccessStatusCode = true
            };
        }

        public static ServiceResponse<T> FailResponse<T>(this ServiceResponse<T> _, string message, int statusCode = 400)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                IsSuccessStatusCode = false
            };
        }

        public static ServiceResponse<T> ErrorResponse<T>(this ServiceResponse<T> _, string errorMessage, int statusCode = 500, List<string>? errors = null)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = errorMessage,
                StatusCode = statusCode,
                Errors = errors,
                IsSuccessStatusCode = false,
                HasErrors = errors != null && errors.Any()
            };
        }

        public static ServiceResponse<T> NotFoundResponse<T>(this ServiceResponse<T> _, string message = "Không tìm thấy thông tin")
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = message,
                StatusCode = 404,
                IsSuccessStatusCode = false
            };
        }
    }
}
