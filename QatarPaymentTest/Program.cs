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
builder.Services.AddControllers();
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

// Run the application
app.Run();

