


//using ZUHS_APIs.Models;
//using ZUHS_APIs.Services;
//using ZUHS_APIs.Intefaces;
//using ZUHS_APIs.Interfaces;
//using Microsoft.Extensions.Logging;

//var builder = WebApplication.CreateBuilder(args);

//// Add logging first
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();
//builder.Logging.AddEventSourceLogger();

//// Enable detailed errors
//builder.WebHost.UseSetting("detailedErrors", "true");
//builder.WebHost.CaptureStartupErrors(true);

//try
//{
//    // Add services to the container.
//    builder.Services.AddControllers();
//    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//    builder.Services.AddScoped<DatabaseConnection>();

//    builder.Services.AddScoped<IDatabaseService>(provider =>
//    {
//        var configuration = provider.GetRequiredService<IConfiguration>();
//        var connectionString = configuration.GetConnectionString("DefaultConnection");
//        var logger = provider.GetRequiredService<ILogger<DatabaseService>>();

//        if (string.IsNullOrEmpty(connectionString))
//        {
//            throw new InvalidOperationException("DefaultConnection string is missing in appsettings.json");
//        }

//        // Create DatabaseConnection object with IConfiguration
//        var dbConnection = new DatabaseConnection(configuration);

//        return new DatabaseService(dbConnection, logger);
//    });

//    builder.Services.AddScoped<IPastelService, PastelService>();

//    builder.Services.AddEndpointsApiExplorer();
//    builder.Services.AddSwaggerGen();

//    var app = builder.Build();

//    // Configure the HTTP request pipeline.
//    if (app.Environment.IsDevelopment())
//    {
//        app.UseSwagger();
//        app.UseSwaggerUI();
//    }

//    app.UseHttpsRedirection();
//    app.UseAuthorization();
//    app.MapControllers();

//    app.Run();
//}
//catch (Exception ex)
//{
//    // Log the exception to a file
//    File.WriteAllText("startup-error.txt", $"{DateTime.Now}: {ex.ToString()}");
//    throw;
//}

/////////////////////////////////////////////////////////////////////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
using ZUHS_APIs.Models;
using ZUHS_APIs.Services;
using ZUHS_APIs.Intefaces;
using ZUHS_APIs.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add logging first
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

// Enable detailed errors
builder.WebHost.UseSetting("detailedErrors", "true");
builder.WebHost.CaptureStartupErrors(true);

try
{
    // Add services to the container.
    builder.Services.AddControllers();

    builder.Services.AddScoped<DatabaseConnection>();

    builder.Services.AddScoped<IDatabaseService>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var logger = provider.GetRequiredService<ILogger<DatabaseService>>();

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("DefaultConnection string is missing in appsettings.json");
        }

        // Create DatabaseConnection object with IConfiguration
        var dbConnection = new DatabaseConnection(configuration);

        return new DatabaseService(dbConnection, logger);
    });

    builder.Services.AddScoped<IPastelService, PastelService>();

    builder.Services.AddEndpointsApiExplorer();

    // JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // set true in production with HTTPS
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // ensures expiry is exact
                };
            });

    // Configure Swagger for all environments
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ZUHS APIs",
            Version = "v1",
            Description = "ZUHS API Documentation",
            Contact = new OpenApiContact
            {
                Name = "Your Name",
                Email = "your.email@example.com"
            }
        });

        // Optional: Include XML comments if you have them
        // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        // c.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    
    // Configure the HTTP request pipeline.

    // Enable Swagger in all environments
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZUHS APIs v1");
        c.RoutePrefix = "swagger"; // Set Swagger UI at the root of the application
        // c.RoutePrefix = string.Empty; // Uncomment if you want Swagger UI at the root
    });

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    // Log the exception to a file
    File.WriteAllText("startup-error.txt", $"{DateTime.Now}: {ex.ToString()}");
    throw;
}



