using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Web.Swagger;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Kiểm tra nếu action có parameter là IFormFile
        var formFileParams = context.ApiDescription.ParameterDescriptions
            .Where(p => p.ModelMetadata?.ModelType == typeof(IFormFile) || 
                       p.ModelMetadata?.ModelType == typeof(IFormFile[]))
            .ToList();

        if (!formFileParams.Any())
            return;

        // Xóa tất cả parameters hiện tại (vì chúng sẽ được thay thế bởi RequestBody)
        operation.Parameters?.Clear();

        // Tạo RequestBody cho multipart/form-data
        var properties = new Dictionary<string, OpenApiSchema>();
        var required = new HashSet<string>();

        foreach (var param in formFileParams)
        {
            var paramName = param.Name ?? "file";
            properties[paramName] = new OpenApiSchema
            {
                Type = "string",
                Format = "binary",
                Description = $"Upload file: {paramName}"
            };
            
            // Kiểm tra xem parameter có required không
            if (param.ModelMetadata?.IsRequired == true)
            {
                required.Add(paramName);
            }
        }

        operation.RequestBody = new OpenApiRequestBody
        {
            Required = required.Any(),
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties = properties,
                        Required = required.Any() ? required : null
                    }
                }
            }
        };
    }
}

