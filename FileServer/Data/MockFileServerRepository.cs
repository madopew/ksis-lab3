using System.Collections.Generic;
using FileServer.Models;

namespace FileServer.Data
{
    public sealed class MockFileServerRepository : IFileServerRepository
    {
        public IEnumerable<DirectoryEntry> GetRootContents()
        {
            yield return new DirectoryEntry {Id = 0, Name = "test.txt", Type = DirectoryEntryType.File};
            yield return new DirectoryEntry {Id = 1, Name = "Api", Type = DirectoryEntryType.Directory};
            yield return new DirectoryEntry {Id = 2, Name = "site.html", Type = DirectoryEntryType.File};
        }

        public IEnumerable<DirectoryEntry> GetDirectoryContents(int id)
        {
            yield return new DirectoryEntry {Id = 3, Name = $"{id} file 1", Type = DirectoryEntryType.File};
            yield return new DirectoryEntry {Id = 4, Name = $"{id} folder 1", Type = DirectoryEntryType.Directory};
            yield return new DirectoryEntry {Id = 5, Name = $"{id} file 2", Type = DirectoryEntryType.File};
        }
    }
}