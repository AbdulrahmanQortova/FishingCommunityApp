using Microsoft.OpenApi;


namespace FishingCommunity.API.Extensions;

public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Fishing Community API",
                Version = "v1",
                Description = "Backend API for the Fishing Community Platform."
            });

            const string securitySchemeId = "Bearer";

            options.AddSecurityDefinition(securitySchemeId, new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Enter your access token below (no need to prefix with 'Bearer ')."
            });

            // v10+ pattern: AddSecurityRequirement now takes a delegate that receives
            // the in-progress OpenApiDocument, so the reference can be resolved against it.
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(securitySchemeId, document)] = new List<string>()
            });
        });

        return services;
    }
}