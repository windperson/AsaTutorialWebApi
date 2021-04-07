using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AsaTutorialWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public FileController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload")]
        public IActionResult Post([FromForm(Name = "files")] List<IFormFile> files)
        {
            try
            {
                var uploadDirPath = Path.Combine(_environment.WebRootPath, "uploads");
                var dir = new DirectoryInfo(uploadDirPath);
                if (!dir.Exists)
                {
                    dir.Create();
                }

                files.ForEach(async file =>
                {
                    if (file.Length <= 0)
                    {
                        return;
                    }

                    var filePath = Path.Combine(uploadDirPath, file.FileName);
                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                });

                return Ok(new { Message = "file uploaded" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("download/{fileName}")]
        public IActionResult Get(string fileName)
        {
            try
            {
                var downloadPath = Path.Combine(_environment.WebRootPath, "uploads");
                var dir = new DirectoryInfo(downloadPath);
                if (!dir.Exists)
                {
                    dir.Create();
                }

                var provider = new PhysicalFileProvider(downloadPath);
                IFileInfo fileInfo = provider.GetFileInfo(fileName);
                var readStream = fileInfo.CreateReadStream();

                return File(readStream, "text/plain");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}