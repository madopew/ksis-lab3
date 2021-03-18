using System.Collections.Generic;
using FileServer.Models;

namespace FileServer.Services.Interfaces
{
    public interface IFileService
    {
        IEnumerable<DirectoryEntry> GetDirectoryEntries(string path);
        DirectoryEntry CreateDirectory(string parentPath, string name);
        FileContent GetFile(string path);
        DirectoryEntry UploadFile(string parentPath, FileUpload file);
        void DeleteEntry(string path);
    }
}