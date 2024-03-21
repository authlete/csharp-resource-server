using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Authlete.Api;
using Authlete.Conf;
using Microsoft.AspNetCore.Hosting;

// Initialize a WebApplication builder with the provided command-line arguments
var builder = WebApplication.CreateBuilder(args);

// Configure the WebHost to listen on all network interfaces on port 5001
builder.WebHost.UseUrls("http://*:5001");

// Adds controllers to the services collection.
// This is required to use MVC controller features in .NET 8.0, replacing the older AddMvc.
builder.Services.AddControllers();

// Adds a singleton service of type IAuthleteApi. This is a custom setup for integrating Authlete, 
// a service for OAuth 2.0 and OpenID Connect 1.0 implementation.
// It uses a specific configuration loader 'AuthletePropertiesConfiguration'
// assumed to load necessary settings for Authlete.
builder.Services.AddSingleton<IAuthleteApi>(provider =>
{
    // Create a configuration instance
    var conf = new AuthletePropertiesConfiguration();
    // Return a new instance of AuthleteApi with the loaded configuration
    return new AuthleteApi(conf);
});

// Build the WebApplication instance using the configured builder
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Use the Developer Exception Page middleware during development
    // for detailed exception stack traces
    app.UseDeveloperExceptionPage();
}

// Middleware to serve default files.
// It will search for default files like index.html in the wwwroot folder.
app.UseDefaultFiles();

// Middleware to serve static files. Allows serving files from wwwroot folder.
app.UseStaticFiles();

// Setup endpoint routing, necessary for ASP.NET Core 3.0 onwards for routing HTTP requests
app.UseRouting();

// Configures endpoints for MVC controller actions.
// It maps incoming requests to controller actions.
app.UseEndpoints(endpoints =>
{
    // Setup routing to map requests to controller actions
    endpoints.MapControllers();
});

// Run the application
app.Run();

