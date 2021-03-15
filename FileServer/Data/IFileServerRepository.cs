using System.Collections.Generic;
using FileServer.Models;

namespace FileServer.Data
{
    public interface IFileServerRepository
    {
        IEnumerable<DirectoryEntry> GetRootContents();
        IEnumerable<DirectoryEntry> GetDirectoryContents(int id);
    }
}