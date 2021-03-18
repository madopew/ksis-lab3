namespace FileApiClient.Models
{
    public sealed class FileUpload
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string FileBase64 { get; set; }
    }
}