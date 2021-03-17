using System.IO;

namespace FileServer.Models
{
    public sealed class FileContent
    {
        public Stream FileStream { get; set; }
        public string ContentType { get; set; }
    }
}