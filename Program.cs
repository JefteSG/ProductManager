using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using ProductManager.Api.Middleware;
using ProductManager.Application.Interfaces;
using ProductManager.Application.Services;
using ProductManager.Application.Validators;
using ProductManager.Infrastructure.Data;
using ProductManager.Infrastructure.Repositories;

// ── Logging ──────────────────────────────────────────────────────────────────
// A configuração do Serilog é feita antes de construir o host para garantir que os logs de inicialização sejam capturados.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// ── Database ──────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Dependency Injection ──────────────────────────────────────────────────────
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// ── Validation ────────────────────────────────────────────────────────────────
// Registra todos os validadores descobertos na assembly automaticamente.
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();

// ── API ───────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Product Manager API",
        Version = "v1",
        Description = "RESTful API para gerenciar produtos com validação de regras de negócio."
    });
    // Incluir comentários XML dos resumos dos controladores na interface do Swagger.
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// ── CORS ──────────────────────────────────────────────────────────────────────
// Permitir o servidor de desenvolvimento do React durante o desenvolvimento. Ajuste para produção.
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// ── Auto-migrate ──────────────────────────────────────────────────────────────
// Aplica migrações pendentes na inicialização para que o aplicativo funcione imediatamente
// sem a necessidade de um passo manual `dotnet ef database update`.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("FrontendDev");

// validação de entrada é feita nos controladores usando os validadores do FluentValidation registrados anteriormente.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Manager API v1"));
}

// Redirecionamento HTTPS deve ser tratado por um reverse proxy (nginx) na frente do contêiner.
// No entanto, durante o desenvolvimento local, é útil ter o redirecionamento HTTPS habilitado para testar a segurança da API. Em produção, o nginx cuidará disso, então podemos desabilitar o redirecionamento HTTPS para evitar confusão.
if (!app.Environment.IsProduction())
    app.UseHttpsRedirection();

app.MapControllers();

Log.Information("Product Manager API starting up");
app.Run();
