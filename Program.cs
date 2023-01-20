global using FastEndpoints;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder();
builder.Services.AddFastEndpoints();

builder.Services.AddDirectoryBrowser();

var app = builder.Build();
app.UseAuthorization();
app.UseFastEndpoints();

app.UseDefaultFiles();
app.UseStaticFiles();

// TODO create settings file.
var imageFolderToHost = "P:/Pictures/BestOfTheBest/Prints";

app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(imageFolderToHost),
    RequestPath = "/Prints",
    EnableDirectoryBrowsing = true
});

app.Run();