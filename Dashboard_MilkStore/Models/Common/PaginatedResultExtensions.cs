namespace Dashboard_MilkStore.Models.Common
{
    public static class PaginatedResultExtensions
    {
        public static PaginatedResult<T> SuccessResponse<T>(this PaginatedResult<T> _, List<T> items, PaginationMetadata metadata, string message = "")
        {
            return new PaginatedResult<T>
            {
                Items = items,
                Metadata = metadata,
                Success = true,
                Message = message,
                StatusCode = 200
            };
        }

        public static PaginatedResult<T> FailResponse<T>(this PaginatedResult<T> _, string message, int statusCode = 400)
        {
            return new PaginatedResult<T>
            {
                Success = false,
                Message = message,
                StatusCode = statusCode,
                Items = new List<T>(),
                Metadata = new PaginationMetadata()
            };
        }
    }
}
