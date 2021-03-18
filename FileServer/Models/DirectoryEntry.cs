using System;

namespace FileServer.Models
{
    public sealed class DirectoryEntry
    {
        public DirectoryEntryType Type { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime LastModified { get; set; }
        public long Size { get; set; }
    }
}