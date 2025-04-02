using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace STBEverywhere_back_APIClient.Services
{
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var formParameters = context.ApiDescription.ActionDescriptor.Parameters
                .Where(param => param.BindingInfo?.BindingSource?.Id == "Form")
                .ToList();

            if (formParameters.Count > 0)
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = formParameters.ToDictionary(
                                param => param.Name,
                                param => new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                })
                        }
                    }
                }
                };
            }
        }
    }
}
