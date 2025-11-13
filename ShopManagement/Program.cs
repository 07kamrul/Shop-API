using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShopManagement.API.Data.Context;
using ShopManagement.Interfaces;
using ShopManagement.Services;
using ShopManagement.UnitOfWork;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shop Management API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IReportService, ReportService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Configure Kestrel - Support both HTTP and HTTPS
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // HTTP on port 7097
    serverOptions.ListenAnyIP(7097);

    // HTTPS on port 7098 (optional for development)
    serverOptions.ListenAnyIP(7098, listenOptions =>
    {
        listenOptions.UseHttps(); // Uses development certificate
    });

    // Localhost HTTP
    serverOptions.Listen(System.Net.IPAddress.Loopback, 7097);
});

// Disable HTTPS redirection in Development
// builder.Services.AddHttpsRedirection(options => { options.HttpsPort = null; });

var app = builder.Build();

// Global exception handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unhandled exception: {ex}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An internal server error occurred.");
    }
});

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop Management API v1");
    });
}

// IMPORTANT: Do NOT use HTTPS redirection for development
// app.UseHttpsRedirection(); // COMMENTED OUT

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Database Migration
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        Console.WriteLine("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration failed: {ex.Message}");
    }
}

// Health check endpoints
app.MapGet("/", () => "Shop Management API is running!");
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    protocol = "HTTP"
}));

Console.WriteLine("========================================");
Console.WriteLine("Shop Management API Started");
Console.WriteLine("========================================");
Console.WriteLine($"HTTP URL: http://localhost:7097");
Console.WriteLine($"Network URL: http://{GetLocalIPAddress()}:7097");
Console.WriteLine($"Android Emulator: http://10.0.2.2:7097");
Console.WriteLine($"Swagger: http://localhost:7097/swagger");
Console.WriteLine("========================================");

app.Run();

// Helper method to get local IP
static string GetLocalIPAddress()
{
    try
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        return "localhost";
    }
    catch
    {
        return "localhost";
    }
}