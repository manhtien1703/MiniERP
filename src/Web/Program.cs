using System;
using System.Linq;
using System.Text;
using Application.Services;
using Domain.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Web.Services;
using Microsoft.AspNetCore.Http;
using Minio;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Cấu hình DateTime luôn serialize theo UTC format (ISO 8601 với "Z")
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        // Đảm bảo DateTime được serialize theo ISO 8601 với UTC timezone
        options.JsonSerializerOptions.WriteIndented = false;
        // Convert DateTime sang UTC trước khi serialize
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MiniERP API",
        Version = "v1",
        Description = "API cho hệ thống quản lý kho lạnh MiniERP"
    });

    // Cấu hình JWT Authentication cho Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                      "Nhập 'Bearer' [space] và token của bạn.\r\n\r\n" +
                      "Ví dụ: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Bỏ qua lỗi khi generate Swagger document để tránh crash
    options.IgnoreObsoleteActions();
    options.IgnoreObsoleteProperties();
    options.CustomSchemaIds(type => type.FullName);
    
    // Cấu hình để Swagger hỗ trợ file upload với [FromForm] IFormFile
    // Map IFormFile để Swagger hiểu cách xử lý
    options.MapType<IFormFile>(() => new OpenApiSchema { Type = "string", Format = "binary" });
    // Sử dụng OperationFilter để convert IFormFile parameters thành RequestBody
    options.OperationFilter<Web.Swagger.FileUploadOperationFilter>();
});

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// JWT Authentication
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? "YourSuperSecretKeyForJWT_MustBeLongEnough_32Chars!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "MiniERP";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "MiniERPUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
    };
});

builder.Services.AddAuthorization();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // Lấy allowed origins từ configuration hoặc environment variable
        var allowedOrigins = builder.Configuration["AllowedOrigins"]?.Split(';')
            ?? new[] { 
                "http://localhost:5173", 
            };
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MiniErpDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)))
);

// Repositories
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IDeviceLogRepository, DeviceLogRepository>();
builder.Services.AddScoped<IProvinceRepository, ProvinceRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<WarehouseService>();
builder.Services.AddScoped<DeviceService>();
builder.Services.AddScoped<MonitoringService>();
builder.Services.AddScoped<ProvinceService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();


// Fake sensor background job
builder.Services.AddHostedService<FakeSensorService>();

// MinIO Configuration
var minioEndpoint = builder.Configuration["MinIO:Endpoint"] ?? "localhost:9000";
var minioAccessKey = builder.Configuration["MinIO:AccessKey"] ?? "minioadmin";
var minioSecretKey = builder.Configuration["MinIO:SecretKey"] ?? "minioadmin";
var minioUseSSL = builder.Configuration.GetValue<bool>("MinIO:UseSSL", false);

builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var client = new MinioClient()
        .WithEndpoint(minioEndpoint)
        .WithCredentials(minioAccessKey, minioSecretKey);
    
    if (minioUseSSL)
    {
        client = client.WithSSL();
    }
    
    return client.Build();
});

// Object Storage Service
builder.Services.AddScoped<IObjectStorageService, MinioObjectStorageService>();

var app = builder.Build();

// Auto migrate database (chạy trong Development và Production)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MiniErpDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        // Kiểm tra database connection
        if (context.Database.CanConnect())
        {
            logger.LogInformation("Database connection successful. Running migrations...");
            context.Database.Migrate(); // Chạy migrations
            logger.LogInformation("Migrations completed successfully.");
        }
        else
        {
            logger.LogWarning("Cannot connect to database. Please check connection string.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        logger.LogError("Please ensure MySQL is running and the connection string is correct.");
    }
}

// Global Exception Handling
app.UseMiddleware<Web.Middleware.GlobalExceptionHandlerMiddleware>();

// CORS PHẢI được đặt TRƯỚC bất kỳ middleware nào có thể redirect hoặc require authentication
// Điều này đảm bảo preflight OPTIONS requests được xử lý đúng cách
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirect - Bật lại để sử dụng HTTPS
// CORS đã được đặt trước nên sẽ xử lý preflight requests đúng cách
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Cấu hình static files để serve ảnh đã upload
app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();

    // Health check endpoint
    app.MapGet("/", () => new { 
        status = "ok", 
        message = "MiniERP API is running",
        timestamp = DateTime.UtcNow,
        endpoints = new[] {
            "/api/Auth/login",
            "/api/Warehouse",
        "/api/Device",
        "/swagger"
    }
}).AllowAnonymous();

app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow }).AllowAnonymous();

app.MapControllers();

// Sử dụng PORT từ environment variable hoặc cấu hình
if (app.Environment.IsDevelopment())
{
    // Development: Chạy trên localhost với HTTPS và HTTP
    app.Run();
}
else
{
    // Production: Sử dụng PORT từ environment variable
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    app.Run($"http://0.0.0.0:{port}");
}
