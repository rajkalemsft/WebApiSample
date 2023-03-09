using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(document =>
 {
     document.AddSecurity("bearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme
     {
         Type = OpenApiSecuritySchemeType.OAuth2,
         Description = "Azure AAD Authentication",
         Flow = OpenApiOAuth2Flow.Implicit,
         Flows = new OpenApiOAuthFlows()
         {
             Implicit = new OpenApiOAuthFlow()
             {
                 Scopes = new Dictionary<string, string>
                        {
                            { $"api://142d74a3-61da-496b-a789-ec791d670663/access_as_user", "Access Application" },
                        },
                 AuthorizationUrl = $"https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/oauth2/v2.0/authorize",
                 TokenUrl = $"https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/oauth2/v2.0/token",
             },
         },
     });

     // To add bearer token in request to APIs with Authorize attribute
     document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));
 });

var app = builder.Build();

app.UseStaticFiles();


// Add Swagger UI
app.UseOpenApi();
app.UseSwaggerUi3(settings =>
{
    settings.OAuth2Client = new OAuth2ClientSettings
    {
        // Use the same client id as your application.
        // Alternatively you can register another application in the portal and use that as client id
        // Doing that you will have to create a client secret to access that application and get into space of secret management
        // This makes it easier to access the application and grab a token on behalf of user
        ClientId = "142d74a3-61da-496b-a789-ec791d670663",
        AppName = "trim-middleware-api",
    };
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
