using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileServer.Models;
using FileServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileServer.Controllers
{
    [ApiController]
    [Route("api/fs")]
    public class DirectoryEntriesController : ControllerBase
    {
        private readonly IFileService fileService;
        public DirectoryEntriesController(IFileService fileService)
        {
            this.fileService = fileService;
        }

        [HttpGet("list")]
        public ActionResult<IEnumerable<DirectoryEntry>> GetRootContents()
        {
            return GetPathContents(string.Empty);
        }
        
        [HttpGet("list/{*path}")]
        public ActionResult<IEnumerable<DirectoryEntry>> GetPathContents(string path)
        {
            IEnumerable<DirectoryEntry> content;
            try
            {
                content = fileService.GetDirectoryEntries(path);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            
            return Ok(content);
        }

        [HttpPost("create")]
        public ActionResult<DirectoryEntry> CreateDirectoryRoot(string name)
        {
            return CreateDirectory(string.Empty, name);
        }

        [HttpPost("create/{*path}")]
        public ActionResult<DirectoryEntry> CreateDirectory(string path, string name)
        {
            DirectoryEntry entry;
            try
            {
                entry = fileService.CreateDirectory(path, name);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(entry);
        }

        [HttpGet("download/{*path}")]
        public ActionResult DownloadFile(string path)
        {
            FileContent result;
            try
            {
                result = fileService.GetFile(path);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return File(result.FileStream, result.ContentType);
        }

        [HttpDelete("delete")]
        public ActionResult DeleteRoot()
        {
            return DeleteEntry(string.Empty);
        }

        [HttpDelete("delete/{*path}")]
        public ActionResult DeleteEntry(string path)
        {
            try
            {
                fileService.DeleteEntry(path);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost("upload")]
        public ActionResult<DirectoryEntry> UploadFile(FileUpload fileUpload)
        {
            return UploadFile(string.Empty, fileUpload);
        }

        [HttpPost("upload/{*path}")]
        public ActionResult<DirectoryEntry> UploadFile(string path, FileUpload fileUpload)
        {
            DirectoryEntry result;
            try
            {
                result = fileService.UploadFile(path, fileUpload);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(result);
        }
    }
}