using Wordie.Server.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Parse command-line args for seeding options
var seedArg = args.Any(a => a.Equals("--seed", StringComparison.OrdinalIgnoreCase));
var forceArg = args.Any(a => a.Equals("--force", StringComparison.OrdinalIgnoreCase));
bool? resetOverride = null;
if (args.Any(a => a.Equals("--no-reset", StringComparison.OrdinalIgnoreCase))) resetOverride = false;
if (args.Any(a => a.Equals("--reset", StringComparison.OrdinalIgnoreCase))) resetOverride = true;

if (seedArg)
{
    // Run seeder according to CLI flags
    await app.InitialiseDatabaseAsync(forceArg, resetOverride);
}
else if (app.Environment.IsDevelopment())
{
    // Default behaviour for local development
    await app.InitialiseDatabaseAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

app.UseExceptionHandler(options => { });

app.UseRouting();

// Ensure authorization middleware is in the pipeline for controllers that require it
app.UseAuthorization();

// Redirect root to the swagger UI
app.MapGet("/", () => Results.Redirect("/api"));

// Map controller routes (switch from minimal API endpoint groups to controllers)
app.MapControllers();

app.Run();

public partial class Program { }
