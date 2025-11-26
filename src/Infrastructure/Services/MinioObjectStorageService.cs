using Application.Services;
using Minio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class MinioObjectStorageService : IObjectStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MinioObjectStorageService> _logger;
    private readonly string _publicUrl;

    public MinioObjectStorageService(
        IMinioClient minioClient,
        IConfiguration configuration,
        ILogger<MinioObjectStorageService> logger)
    {
        _minioClient = minioClient;
        _configuration = configuration;
        _logger = logger;
        _publicUrl = configuration["MinIO:PublicUrl"] ?? configuration["MinIO:Endpoint"] ?? "http://localhost:9000";
    }

    public async Task<string> UploadFileAsync(string bucketName, string objectName, Stream stream, string contentType, Dictionary<string, string>? metadata = null)
    {
        try
        {
            // Đảm bảo bucket tồn tại
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(bucketName);
            bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs).ConfigureAwait(false);
            
            if (!found)
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(makeBucketArgs).ConfigureAwait(false);
                
                _logger.LogInformation("Created bucket: {BucketName}", bucketName);
                
                // Cấu hình bucket policy để public read (nếu cần)
                try
                {
                    var policy = GeneratePublicReadPolicy(bucketName);
                    var setPolicyArgs = new SetPolicyArgs()
                        .WithBucket(bucketName)
                        .WithPolicy(policy);
                    await _minioClient.SetPolicyAsync(setPolicyArgs).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not set bucket policy for {BucketName}. Continuing anyway.", bucketName);
                }
            }

            // Lưu stream length trước khi đọc
            var streamLength = stream.Length;
            
            // Upload file
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(stream)
                .WithObjectSize(streamLength)
                .WithContentType(contentType);

            // Metadata sẽ được MinIO tự động lưu nếu cần
            // Có thể thêm sau khi MinIO hỗ trợ metadata trong PutObjectArgs

            await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

            _logger.LogInformation("Uploaded file to MinIO: {BucketName}/{ObjectName}", bucketName, objectName);

            // Trả về public URL hoặc presigned URL
            var usePublicUrlString = _configuration["MinIO:UsePublicUrl"] ?? "true";
            var usePublicUrl = bool.TryParse(usePublicUrlString, out var result) ? result : true;
            
            if (usePublicUrl)
            {
                // Trả về public URL (nếu bucket có public read policy)
                var publicUrl = $"{_publicUrl.TrimEnd('/')}/{bucketName}/{objectName}";
                return publicUrl;
            }
            else
            {
                // Trả về presigned URL (tạm thời, có thời hạn)
                return await GetPresignedUrlAsync(bucketName, objectName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to MinIO: {BucketName}/{ObjectName}", bucketName, objectName);
            throw;
        }
    }

    public async Task DeleteFileAsync(string bucketName, string objectName)
    {
        try
        {
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);
            
            await _minioClient.RemoveObjectAsync(removeObjectArgs).ConfigureAwait(false);
            
            _logger.LogInformation("Deleted file from MinIO: {BucketName}/{ObjectName}", bucketName, objectName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from MinIO: {BucketName}/{ObjectName}", bucketName, objectName);
            throw;
        }
    }

    public async Task<string> GetPresignedUrlAsync(string bucketName, string objectName, int expiresInSeconds = 604800)
    {
        try
        {
            var presignedGetObjectArgs = new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithExpiry(expiresInSeconds);

            string url = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs).ConfigureAwait(false);
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL: {BucketName}/{ObjectName}", bucketName, objectName);
            throw;
        }
    }

    public async Task<bool> FileExistsAsync(string bucketName, string objectName)
    {
        try
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);

            await _minioClient.StatObjectAsync(statObjectArgs).ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private string GeneratePublicReadPolicy(string bucketName)
    {
        // Tạo policy để cho phép public read
        return $@"{{
            ""Version"": ""2012-10-17"",
            ""Statement"": [
                {{
                    ""Effect"": ""Allow"",
                    ""Principal"": {{ ""AWS"": [""*""] }},
                    ""Action"": [""s3:GetObject""],
                    ""Resource"": [""arn:aws:s3:::{bucketName}/*""]
                }}
            ]
        }}";
    }
}
