using System;
using System.Collections.Generic;
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
        private readonly IFileService _fileService;
        public DirectoryEntriesController(IFileService fileService)
        {
            _fileService = fileService;
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
                content = _fileService.GetDirectoryEntries(path);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            
            return Ok(content);
        }

        [HttpPost("create")]
        public ActionResult CreateDirectoryRoot(string name)
        {
            return CreateDirectory(string.Empty, name);
        }

        [HttpPost("create/{*path}")]
        public ActionResult CreateDirectory(string path, string name)
        {
            DirectoryEntry entry;
            try
            {
                entry = _fileService.CreateDirectory(path, name);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(entry);
        }
    }
}