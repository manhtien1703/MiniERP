using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Application.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IObjectStorageService _objectStorageService;
    private readonly ILogger<UploadController> _logger;
    private readonly string _bucketName;

    public UploadController(
        IObjectStorageService objectStorageService,
        ILogger<UploadController> logger,
        IConfiguration configuration)
    {
        _objectStorageService = objectStorageService;
        _logger = logger;
        _bucketName = configuration["MinIO:BucketName"] ?? "device-images";
    }

    [HttpPost("device-image")]
    [Consumes("multipart/form-data")]
    [ApiExplorerSettings(IgnoreApi = true)] // Tạm thời exclude khỏi Swagger để tránh lỗi 500
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
            // Tạo tên file duy nhất
            var fileName = $"{Guid.NewGuid()}{extension}";
            var objectName = $"devices/{fileName}";

            _logger.LogInformation("Uploading file to MinIO: {FileName} -> {BucketName}/{ObjectName}", file.FileName, _bucketName, objectName);

            // Lấy content type từ file
            var contentType = file.ContentType ?? "application/octet-stream";
            
            // Xác định content type dựa trên extension nếu chưa có
            if (contentType == "application/octet-stream")
            {
                contentType = extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };
            }

            // Upload file lên MinIO
            using (var stream = file.OpenReadStream())
            {
                var fileUrl = await _objectStorageService.UploadFileAsync(
                    _bucketName,
                    objectName,
                    stream,
                    contentType,
                    new Dictionary<string, string>
                    {
                        { "original-filename", file.FileName },
                        { "upload-date", DateTime.UtcNow.ToString("O") }
                    });

                _logger.LogInformation("File uploaded successfully to MinIO: {FileUrl}", fileUrl);
                return Ok(new { url = fileUrl });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to MinIO: {Message}", ex.Message);
            return StatusCode(500, new { error = $"Lỗi khi upload file: {ex.Message}" });
        }
    }

    [HttpDelete("device-image/{fileName}")]
    public async Task<IActionResult> DeleteDeviceImage(string fileName)
    {
        try
        {
            var objectName = $"devices/{fileName}";
            
            // Kiểm tra file có tồn tại không
            var exists = await _objectStorageService.FileExistsAsync(_bucketName, objectName);
            if (!exists)
            {
                return NotFound(new { error = "File không tồn tại" });
            }

            // Xóa file từ MinIO
            await _objectStorageService.DeleteFileAsync(_bucketName, objectName);
            
            _logger.LogInformation("File deleted successfully from MinIO: {BucketName}/{ObjectName}", _bucketName, objectName);
            return Ok(new { message = "Xóa file thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from MinIO: {Message}", ex.Message);
            return StatusCode(500, new { error = $"Lỗi khi xóa file: {ex.Message}" });
        }
    }
}
