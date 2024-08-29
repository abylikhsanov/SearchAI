using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
namespace SearchAI.controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadDocument(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is empty");
        }

        return Ok();
    }
}