using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SwaggerFileUploadFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parameters = operation.Parameters;

        // 檢查是否包含 IFormFile 參數
        var hasFormFile = context.MethodInfo.GetParameters()
            .Any(p => p.ParameterType == typeof(IFormFile));

        if (hasFormFile)
        {
            // 設置為 multipart/form-data
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["title"] = new OpenApiSchema { Type = "string" },
                                ["startDate"] = new OpenApiSchema { Type = "string", Format = "date-time" },
                                ["endDate"] = new OpenApiSchema { Type = "string", Format = "date-time" },
                                ["adpriority"] = new OpenApiSchema { Type = "integer" },
                                ["status"] = new OpenApiSchema { Type = "string" },
                                ["url"] = new OpenApiSchema { Type = "string" },
                                ["img"] = new OpenApiSchema { Type = "string", Format = "binary" }
                            },
                            Required = new HashSet<string> { "title", "img" } // 根據需求調整必填字段
                        }
                    }
                }
            };

            // 從查詢參數移除文件參數
            operation.Parameters = operation.Parameters
                .Where(p => p.Name != "img")
                .ToList();
        }
    }
}
