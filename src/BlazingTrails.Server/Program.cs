using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
builder.Services.AddDbContext<BlazingTrailsContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString(nameof(BlazingTrailsContext)));
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddValidatorsFromAssemblyContaining<Shared>();

builder.Services.Configure<FormOptions>(options =>
{
    // Set the limit to 5 MB
    options.MultipartBodyLengthLimit = 5 * 1024 * 1024;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseBlazorFrameworkFiles();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Images")),
    RequestPath = new PathString("/Images")
});

app.MapFallbackToFile("index.html");

app.MapGet("/api/v1/antiforgery/token", (IAntiforgery antiforgery, HttpContext context) =>
{
    var tokens = antiforgery.GetAndStoreTokens(context);
    return TypedResults.Content(tokens.RequestToken, "text/plain");
});
//.RequireAuthorization(); // In a real world scenario, you'll only give this token to authorized users

app.MapPost("/api/v1/trails", CreateTrailEndpoint.Invoke);
app.MapPost("/api/v1/trails/{trailId}/images", UploadTrailImageEndpoint.Invoke);

app.Run();
