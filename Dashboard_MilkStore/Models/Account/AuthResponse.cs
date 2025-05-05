using System.Text.Json.Serialization;

namespace Dashboard_MilkStore.Models.Account
{
    public class AuthResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }

        // Use JsonIgnore to prevent deserialization issues with the Errors property
        [JsonIgnore]
        public IEnumerable<string>? Errors { get; set; }

        public bool IsValid => Success && !string.IsNullOrEmpty(AccessToken);

        [JsonIgnore]
        public bool HasErrors => Errors != null && Errors.Any();
    }
}
