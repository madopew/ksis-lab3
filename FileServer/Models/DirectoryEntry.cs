namespace FileServer.Models
{
    public sealed class DirectoryEntry
    {
        public DirectoryEntryType Type { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}