using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdefHelpDeskBase
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("internal", new OpenApiInfo { Title = "Internal API", Version = "v1" });
                c.SwaggerDoc("external",
                    new OpenApiInfo
                    {
                        Title = "External API",
                        Version = "v1",
                        Description = "ADefHelpDesk Web API"
                    });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                // Swagger 5.+ support
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] { }
                    }
                });

                // Set the comments path for the Swagger JSON and UI.                
                var xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ADefHelpDeskApp", "ADefHelpDeskApp.xml");
                c.IncludeXmlComments(xmlPath);
                c.OperationFilter<FileUploadOperation>(); //Register File Upload Operation Filter
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/external/swagger.json", "ADefHelpDesk External API V1");
            });

            return app;
        }

        public class FileUploadOperation : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                if (operation.Parameters != null)
                {
                    // If the method has a paramater "objFile"
                    // rewrite the parameters to create an upload control in swagger
                    if (operation.Parameters.Where(x => x.Name == "objFile").FirstOrDefault() != null)
                    {
                        // Create a collection that does not have objFile
                        // because it will be added with an upload control
                        List<OpenApiParameter> colIParameter = new List<OpenApiParameter>();
                        foreach (var item in operation.Parameters)
                        {
                            if (item.Name != "objFile")
                            {
                                colIParameter.Add(item);
                            }
                        }

                        operation.Parameters.Clear();
                        operation.Parameters = colIParameter;

                        operation.Parameters.Add(new OpenApiParameter
                        {
                            Name = "objFile",
                            In = ParameterLocation.Query,
                            Description = "Upload File",
                            Required = false,
                            Style = ParameterStyle.Form,
                            //Type = "file"
                        });
                        //operation.Consumes.Add("multipart/form-data");
                    }
                }
            }
        }
    }
}