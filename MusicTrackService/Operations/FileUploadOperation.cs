using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MusicTrackService.Operations;

public class FileUploadOperation : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                // Check if operation or context is null
                if (operation == null || context == null)
                {
                    return; // Exit if operation or context is null
                }
    
                // Check if the action matches the upload MP3 operation
                if (operation.OperationId?.ToLower().Contains("uploadmp3") == true)
                {
                    operation.Parameters ??= new List<OpenApiParameter>(); // Initialize if null
                    operation.Parameters.Clear();
    
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = "musicTrackId",
                        In = ParameterLocation.Path,
                        Required = true,
                        Schema = new OpenApiSchema
                        {
                            Type = "integer"
                        }
                    });
    
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["multipart/form-data"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Properties =
                                    {
                                        ["mp3File"] = new OpenApiSchema
                                        {
                                            Type = "string",
                                            Format = "binary"
                                        }
                                    },
                                    Required = new HashSet<string> { "mp3File" }
                                }
                            }
                        }
                    };
                }
            }
}