using System;
using System.Collections.Generic;
using System.IO;
using FileServer.Models;
using FileServer.Services.Interfaces;
using MimeTypes;

namespace FileServer.Services.Implementations
{
    public class LocalFileService : IFileService
    {
        private const string RootPath = @"C:\Users\Madi\source\repos\ksis_lab3\FileServer\Data";

        public IEnumerable<DirectoryEntry> GetDirectoryEntries(string path)
        {
            var directoryPath = Path.Join(RootPath, path);
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
                var size = (entry is FileInfo info) ? info.Length : -1;
                resultList.Add(new DirectoryEntry
                {
                    Type = GetEntryType(entry),
                    Name = entry.Name,
                    Path = Normalize(relativePath),
                    Size = size,
                    LastModified = entry.LastWriteTime,                    
                });
            }
            
            return resultList;
        }

        public DirectoryEntry CreateDirectory(string parentPath, string name)
        {
            var parentAbsolutePath = Path.Join(RootPath, parentPath);
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
                Size = -1,
                LastModified = createdDirectory.LastWriteTime,
            };
        }

        public FileContent GetFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Not a file");
            }

            var absolutePath = Path.Join(RootPath, path);
            absolutePath = Path.GetFullPath(absolutePath);

            if (!File.Exists(absolutePath))
            {
                throw new ArgumentException("File does not exist!");
            }

            var fileInfo = new FileInfo(absolutePath);
            var fileStream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var type = MimeTypeMap.GetMimeType(fileInfo.Extension);
            return new FileContent
            {
                FileStream = fileStream,
                ContentType = type,
            };
        }

        public DirectoryEntry UploadFile(string parentPath, FileUpload file)
        {
            var parentAbsolutePath = Path.Join(RootPath, parentPath);
            parentAbsolutePath = Path.GetFullPath(parentAbsolutePath);
            
            if (!Directory.Exists(parentAbsolutePath))
            {
                throw new ArgumentException("Directory does not exist!");
            }
            
            if (string.IsNullOrWhiteSpace(file.Name))
            {
                throw new ArgumentException("Name is empty!");
            }
            
            if (string.IsNullOrWhiteSpace(file.ContentType))
            {
                throw new ArgumentException("Content type is empty!");
            }
            
            if (string.IsNullOrWhiteSpace(file.FileBase64))
            {
                throw new ArgumentException("Data is empty!");
            }

            string fileName;
            try
            {
                fileName = file.Name + $"{MimeTypeMap.GetExtension(file.ContentType)}";
            }
            catch (Exception e)
            {
                throw new ArgumentException("Content type is incorrect!");
            }
            var fileAbsolutePath = Path.Combine(parentAbsolutePath, fileName);

            if (File.Exists(fileAbsolutePath))
            {
                throw new ArgumentException("File already exists!");
            }

            using var createdFile = new FileStream(fileAbsolutePath, FileMode.Create);
            createdFile.Write(Convert.FromBase64String(file.FileBase64));

            return new DirectoryEntry
            {
                Type = DirectoryEntryType.File,
                Name = fileName,
                Path = Normalize(Path.GetRelativePath(RootPath, fileAbsolutePath)),
                Size = createdFile.Length,
                LastModified = DateTime.Now,
            };
        }

        public void DeleteEntry(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Directory.Delete(RootPath, true);
                Directory.CreateDirectory(RootPath);
            }
            else
            {
                var absolutePath = Path.Join(RootPath, path);
                absolutePath = Path.GetFullPath(absolutePath);

                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                }
                else if (Directory.Exists(absolutePath))
                {
                    Directory.Delete(absolutePath, true);
                }
                else
                {
                    throw new ArgumentException("There is no such directory nor file");
                }
            }
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
                Size = -1,
                LastModified = DateTime.MinValue,
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