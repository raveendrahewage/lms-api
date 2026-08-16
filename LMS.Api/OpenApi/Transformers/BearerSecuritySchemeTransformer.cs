using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace LMS.Api.OpenApi.Transformers;

public class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationScheme = authenticationSchemeProvider.GetAllSchemesAsync();
        if (!authenticationScheme.Result.Any(s => s.Name == JwtBearerDefaults.AuthenticationScheme))
            return;

        var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>()
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "JWT Authorization LMS : Enter your token in the format 'Bearer {your_token}'"
            }
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = securitySchemes;

        foreach(var operation in document.Paths.Values.SelectMany(p => p.Operations ?? []))
        {
            operation.Value.Security ??= [];
            operation.Value.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        }
    }
}