using System.IO;

namespace FileApiClient.Models
{
    public sealed class FileContent
    {
        public Stream FileStream { get; set; }
        public string ContentType { get; set; }
    }
}