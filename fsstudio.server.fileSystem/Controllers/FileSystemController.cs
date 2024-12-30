// FileSystemController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using fsstudio.server.fileSystem.exec;

namespace fsstudio.server.fileSystem
{
    public class FileOperationModel
    {
        public string Path { get; set; } = "";
        public string Name { get; set; } = "";
    }

    [Route("api/[controller]")]
    [ApiController]
    public class FileSystemController : ControllerBase
    {
        private readonly SessionManager _sessionManager;

        public FileSystemController(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        [HttpGet("ListSubFoldersAndFiles")]
        public IActionResult ListSubFoldersAndFiles([FromQuery] string path)
        {
            try
            {
                var result = _sessionManager.ListSubFoldersAndFiles(path);
                return Ok(new { Directories = result.Directories, Files = result.Files });
            }
            catch (DirectoryNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("CreateFolder")]
        public IActionResult CreateFolder([FromBody] FileOperationModel model)
        {
            try
            {
                _sessionManager.CreateFolder(model.Path, model.Name);
                return Created($"{model.Path}/{model.Name}",
                    $"Folder '{model.Name}' created successfully.");
            }
            catch (IOException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("CreateFile")]
        public IActionResult CreateFile([FromBody] FileOperationModel model)
        {
            try
            {
                _sessionManager.CreateFile(model.Path, model.Name);
                return Created($"{model.Path}/{model.Name}",
                    $"File '{model.Name}.fsp' created successfully.");
            }
            catch (IOException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("DuplicateFile")]
        public IActionResult DuplicateFile([FromBody] FileOperationModel model)
        {
            try
            {
                _sessionManager.DuplicateFile(model.Path, model.Name);
                return Created($"{model.Path}/{model.Name}",
                    $"File duplicated as '{model.Name}.fsp' successfully.");
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (IOException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("DeleteItem")]
        public IActionResult DeleteItem(string path)
        {
            try
            {
                _sessionManager.DeleteItem(path);
                return Ok($"Item '{path}' deleted successfully.");
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("RenameItem")]
        public IActionResult RenameItem([FromBody] FileOperationModel model)
        {
            try
            {
                _sessionManager.RenameItem(model.Path, model.Name);
                return Ok($"Item '{model.Path}' renamed to '{model.Name}' successfully.");
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (IOException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}