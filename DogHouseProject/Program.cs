using Api.Filters;
using Api.Middlewares;
using Application.Interfaces;
using Application.Services;
using Application.Validators;
using Domain.Interfaces;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DogContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IDogService, DogService>();
builder.Services.AddScoped<IDogRepository, DogRepository>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
});


builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<DogCreateDtoValidator>());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RateLimitingMiddleware>(10); // Limit 10 requests per second
app.UseMiddleware<JsonSyntaxErrorMiddleware>();

// Global error handler with UseExceptionHandler to catch JSON syntax errors
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error is JsonException)
        {
            // If this is a JSON syntax error, return an invalid format message.
            var errorResponse = new
            {
                Errors = new[]
                {
                    new
                    {
                        Message = "Invalid JSON format. Please check your JSON syntax."
                    }
                }
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(jsonResponse);
        }
        else
        {
            // General handler for other errors
            var generalErrorResponse = new
            {
                Errors = new[]
                {
                    new
                    {
                        Message = "An unexpected error occurred."
                    }
                }
            };

            var jsonResponse = JsonSerializer.Serialize(generalErrorResponse);
            await context.Response.WriteAsync(jsonResponse);
        }
    });
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
