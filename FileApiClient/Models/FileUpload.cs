using Newtonsoft.Json;

namespace FileApiClient.Models
{
    public sealed class FileUpload
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("contenttype")]
        public string ContentType { get; set; }
        
        [JsonProperty("filebase64")]
        public string FileBase64 { get; set; }
    }
}