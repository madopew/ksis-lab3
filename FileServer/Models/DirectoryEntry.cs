namespace FileServer.Models
{
    public sealed class DirectoryEntry
    {
        public int Id { get; set; }
        public DirectoryEntryType Type { get; set; }
        public string Name { get; set; }
    }
}