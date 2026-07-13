using AgentRepoViewer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(cors =>
    {
        cors.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Register RepoAnalyzerService with repo root path
var repoRoot = Path.Combine(Directory.GetCurrentDirectory(), "..");
builder.Services.AddSingleton(new RepoAnalyzerService(repoRoot));

var app = builder.Build();

app.UseRouting();
app.UseCors();

app.MapControllers();
app.MapGet("/", () => Results.File(Path.Combine(app.Environment.ContentRootPath, "wwwroot", "index.html"), "text/html"));

app.Run();

