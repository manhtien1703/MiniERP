using System;
using System.Text;
using Application.Services;
using Domain.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
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
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:5175", "http://localhost:5177")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MiniErpDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
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

var app = builder.Build();

// Auto migrate database (chỉ chạy trong Development)
if (app.Environment.IsDevelopment())
{
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
                logger.LogWarning("Cannot connect to database. Please create the database first.");
                logger.LogWarning("Run: CREATE DATABASE MiniERP CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;");
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
            logger.LogError("Please ensure MySQL is running and the database 'MiniERP' exists.");
        }
    }
}

// Global Exception Handling
app.UseMiddleware<Web.Middleware.GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Cấu hình static files để serve ảnh đã upload
app.UseStaticFiles();

// Sử dụng CORS - phải đặt trước Authentication
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
