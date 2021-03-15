using System;
using System.Collections.Generic;
using System.IO;
using FileServer.Models;
using FileServer.Services.Interfaces;

namespace FileServer.Services.Implementations
{
    public class LocalFileService : IFileService
    {
        private const string RootPath = @"C:\Users\Madi\source\repos\ksis_lab3\FileServer\Data";

        public IEnumerable<DirectoryEntry> GetDirectoryEntries(string path)
        {
            var directoryPath = Path.Combine(RootPath, path);
            directoryPath = Path.GetFullPath(directoryPath);
            
            if (!Directory.Exists(directoryPath))
            {
                throw new ArgumentException("Directory does not exist");
            }
            
            var directory = new DirectoryInfo(directoryPath);
            var entries = directory.GetFileSystemInfos();

            var resultList = new List<DirectoryEntry>(entries.Length)
            {
                GetParentDirectory(directoryPath),
            };

            foreach (var entry in entries)
            {
                var relativePath = Path.GetRelativePath(RootPath, entry.FullName);
                resultList.Add(new DirectoryEntry
                {
                    Type = GetEntryType(entry),
                    Name = entry.Name,
                    Path = Normalize(relativePath),
                });
            }
            
            return resultList;
        }

        public DirectoryEntry CreateDirectory(string parentPath, string name)
        {
            var parentAbsolutePath = Path.Combine(RootPath, parentPath);
            parentAbsolutePath = Path.GetFullPath(parentAbsolutePath);
            
            if (!Directory.Exists(parentAbsolutePath))
            {
                throw new ArgumentException("Directory does not exist");
            }
            
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name is empty");
            }

            var directoryPath = Path.Combine(parentAbsolutePath, name);
            if (Directory.Exists(directoryPath))
            {
                throw new ArgumentException("Directory already exists");
            }

            var createdDirectory = Directory.CreateDirectory(directoryPath);
            
            directoryPath = Path.GetRelativePath(RootPath, directoryPath);
            return new DirectoryEntry
            {
                Type = GetEntryType(createdDirectory),
                Name = createdDirectory.Name,
                Path = Normalize(directoryPath),
            };
        }

        private static DirectoryEntry GetParentDirectory(string childAbsolutePath)
        {
            var parentPath = string.Join(Path.DirectorySeparatorChar, 
                childAbsolutePath.Split(Path.DirectorySeparatorChar)[..^1]);
            parentPath = Path.GetRelativePath(RootPath, parentPath);

            if (parentPath.Equals(".") || parentPath.Equals(".."))
            {
                parentPath = string.Empty;
            }
            else
            {
                parentPath = Normalize(parentPath);
            }
            
            return new DirectoryEntry
            {
                Type = DirectoryEntryType.Directory,
                Name = "..",
                Path = parentPath,
            };
        }

        private static string Normalize(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }

        private static DirectoryEntryType GetEntryType(FileSystemInfo entry)
        {
            return (entry.Attributes & FileAttributes.Directory) != 0
                ? DirectoryEntryType.Directory
                : DirectoryEntryType.File;
        }
    }
}