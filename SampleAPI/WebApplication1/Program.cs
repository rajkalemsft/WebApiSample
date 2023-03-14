using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Westwind.AspNetCore.Markdown;

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

builder.Services.AddMarkdown(config =>
{
    // optional Tag BlackList
    config.HtmlTagBlackList = "script|iframe|object|embed|form"; // default

    // Simplest: Use all default settings
    var folderConfig = config.AddMarkdownProcessingFolder("/docs/", "~/Pages/__MarkdownPageTemplate.cshtml");

    // Customized Configuration: Set FolderConfiguration options
    //folderConfig = config.AddMarkdownProcessingFolder("/posts/", "~/Pages/__MarkdownPageTemplate.cshtml");

    // Optionally strip script/iframe/form/object/embed tags ++
    folderConfig.SanitizeHtml = false;  //  default

    // Optional configuration settings
    folderConfig.ProcessExtensionlessUrls = true;  // default
    folderConfig.ProcessMdFiles = true; // default

    // Optional pre-processing - with filled model
    folderConfig.PreProcess = (model, controller) =>
    {
        // controller.ViewBag.Model = new MyCustomModel();
    };

    // folderConfig.BasePath = "https://github.com/RickStrahl/Westwind.AspNetCore.Markdow/raw/master";

    // Create your own IMarkdownParserFactory and IMarkdownParser implementation
    // to replace the default Markdown Processing
    //config.MarkdownParserFactory = new CustomMarkdownParserFactory();                 

    // optional custom MarkdigPipeline (using MarkDig; for extension methods)
    config.ConfigureMarkdigPipeline = builder =>
    {
        builder.UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Default)
            .UsePipeTables()
            .UseGridTables()
            .UseAutoIdentifiers(AutoIdentifierOptions.GitHub) // Headers get id="name" 
            .UseAutoLinks() // URLs are parsed into anchors
            .UseAbbreviations()
            .UseYamlFrontMatter()
            .UseEmojiAndSmiley(true)
            .UseListExtras()
            .UseFigures()
            .UseTaskLists()
            .UseCustomContainers()
            //.DisableHtml()   // renders HTML tags as text including script
            .UseGenericAttributes();
    };
});

// We need to use MVC so we can use a Razor Configuration Template
// for the Markdown Processing Middleware
builder.Services.AddMvc()
    // have to let MVC know we have a controller otherwise it won't be found
    .AddApplicationPart(typeof(MarkdownPageProcessorMiddleware).Assembly);

var app = builder.Build();

app.UseDefaultFiles(new DefaultFilesOptions()
{
    DefaultFileNames = new List<string> { "index.md", "index.html" }
});

app.UseMarkdown();

app.UseRouting();

app.UseStaticFiles();

app.MapRazorPages();
app.MapDefaultControllerRoute();

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




app.Run();
