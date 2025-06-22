using QatarPaymentTest.Data.DbContextApp;
using QatarPaymentTest.Extensions;
using QatarPaymentTest.Middleware;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
 .ReadFrom.Configuration(builder.Configuration)
 .CreateLogger();


// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.WriteIndented = true;

});
// Configure PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
 options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application-specific services
builder.Services.RegisterApplicationServices();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

var app = builder.Build();


// Enable CORS
app.UseCors("AllowAll");

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable authorization
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await services.SeedDataAsync();

        // Generate test data if environment is Development
        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine("Do you want to generate test data? (y/n)");
            var response = Console.ReadLine()?.ToLower();
            if (response == "y")
            {
                Console.WriteLine("Enter number of records to generate (default is 1,000,000):");
                var input = Console.ReadLine();
                var numberOfRecords = string.IsNullOrEmpty(input) ? 1000000 : int.Parse(input);
                await services.GenerateMillionRecordsAsync(numberOfRecords);
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Run the application
app.Run();

