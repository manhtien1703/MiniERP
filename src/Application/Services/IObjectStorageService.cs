using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Application.Services;

public interface IObjectStorageService
{
    /// <summary>
    /// Upload file lên object storage
    /// </summary>
    /// <param name="bucketName">Tên bucket</param>
    /// <param name="objectName">Tên object (file path trong bucket)</param>
    /// <param name="stream">Stream của file</param>
    /// <param name="contentType">Content type của file (ví dụ: image/jpeg)</param>
    /// <param name="metadata">Metadata của file (optional)</param>
    /// <returns>URL của file đã upload</returns>
    Task<string> UploadFileAsync(string bucketName, string objectName, Stream stream, string contentType, Dictionary<string, string>? metadata = null);

    /// <summary>
    /// Xóa file khỏi object storage
    /// </summary>
    /// <param name="bucketName">Tên bucket</param>
    /// <param name="objectName">Tên object (file path trong bucket)</param>
    Task DeleteFileAsync(string bucketName, string objectName);

    /// <summary>
    /// Tạo presigned URL để truy cập file (có thời hạn)
    /// </summary>
    /// <param name="bucketName">Tên bucket</param>
    /// <param name="objectName">Tên object</param>
    /// <param name="expiresInSeconds">Thời gian hết hạn (giây), mặc định 7 ngày</param>
    /// <returns>Presigned URL</returns>
    Task<string> GetPresignedUrlAsync(string bucketName, string objectName, int expiresInSeconds = 604800);

    /// <summary>
    /// Kiểm tra file có tồn tại không
    /// </summary>
    /// <param name="bucketName">Tên bucket</param>
    /// <param name="objectName">Tên object</param>
    /// <returns>True nếu file tồn tại</returns>
    Task<bool> FileExistsAsync(string bucketName, string objectName);
}

