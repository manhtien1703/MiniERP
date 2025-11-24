using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<UploadController> _logger;

    public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpPost("device-image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDeviceImage([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "Không có file được upload" });
        }

        // Kiểm tra định dạng file
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(new { error = "Định dạng file không được hỗ trợ. Chỉ chấp nhận: jpg, jpeg, png, gif, webp" });
        }

        // Kiểm tra kích thước file (max 5MB)
        if (file.Length > 5 * 1024 * 1024)
        {
            return BadRequest(new { error = "File quá lớn. Kích thước tối đa: 5MB" });
        }

        try
        {
            // Đảm bảo WebRootPath tồn tại
            if (string.IsNullOrEmpty(_environment.WebRootPath))
            {
                _environment.WebRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
            }

            // Tạo thư mục wwwroot nếu chưa có
            if (!Directory.Exists(_environment.WebRootPath))
            {
                Directory.CreateDirectory(_environment.WebRootPath);
            }

            // Tạo thư mục uploads nếu chưa có
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "devices");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Tạo tên file duy nhất
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            _logger.LogInformation("Uploading file: {FileName} to {FilePath}", file.FileName, filePath);

            // Lưu file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về URL
            var fileUrl = $"/uploads/devices/{fileName}";
            _logger.LogInformation("File uploaded successfully: {FileUrl}", fileUrl);
            return Ok(new { url = fileUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {Message}", ex.Message);
            return StatusCode(500, new { error = $"Lỗi khi upload file: {ex.Message}" });
        }
    }

    [HttpDelete("device-image/{fileName}")]
    public IActionResult DeleteDeviceImage(string fileName)
    {
        try
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "devices");
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                return Ok(new { message = "Xóa file thành công" });
            }

            return NotFound(new { error = "File không tồn tại" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file");
            return StatusCode(500, new { error = "Lỗi khi xóa file" });
        }
    }
}
