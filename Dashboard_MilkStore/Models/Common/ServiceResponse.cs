namespace Dashboard_MilkStore.Models.Common
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public List<string>? Errors { get; set; }
        public bool IsSuccessStatusCode { get; set; }
        public bool HasErrors { get; set; }
    }
} 