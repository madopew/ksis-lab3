using System.Collections.Generic;
using FileServer.Data;
using FileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileServer.Controllers
{
    [ApiController]
    [Route("api/dir")]
    public class DirectoryEntriesController : ControllerBase
    {
        private readonly IFileServerRepository _repository = new MockFileServerRepository();
        
        [HttpGet]
        public ActionResult<IEnumerable<DirectoryEntry>> GetRootContents()
        {
            var result = _repository.GetRootContents();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public ActionResult<IEnumerable<DirectoryEntry>> GetContents(int id)
        {
            if (id < 0 || id > 2)
            {
                return BadRequest();
            }

            var result = _repository.GetDirectoryContents(id);
            return Ok(result);
        }
    }
}