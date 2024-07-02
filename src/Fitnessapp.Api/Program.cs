using Carter;
using Fitnessapp.Api.Database;
using Fitnessapp.Api.Entities;
using Fitnessapp.Api.Features.UserProfiles;
using Fitnessapp.Api.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Reflection.Emit;

var builder = WebApplication.CreateBuilder(args);

string c = Directory.GetCurrentDirectory();
IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(c).AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json").Build();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    string connStr = config.GetValue<string>("ConnectionStrings:DefaultConnection");
    options.UseSqlServer(connStr);
});

var assembly = typeof(Program).Assembly;

builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

builder.Services.AddScoped<IRunActivityRepository, RunActivityRepository>();

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddCarter();

builder.Services.AddValidatorsFromAssembly(assembly);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/FitnessApp-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCarter();

app.UseHttpsRedirection();

ApplyMigration();
SeedData();

app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _db.Database.EnsureCreated();

        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
        var services = scope.ServiceProvider;

    };
}

void SeedData()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if(_db.UserProfiles.Where(f=>f.Id == 1).FirstOrDefault() == null)
        {
            _db.UserProfiles.Add(
                new UserProfile
                {
                    Name = "ervin miranda",
                    BirthDate = DateTime.Parse("1982-04-05"),
                    Height = 172,
                    Weight = 83
                });            
            _db.SaveChanges();
        }

        if(_db.RunActivities.Where(f=>f.UserProfileId == 1).FirstOrDefault() == null)
        {
            _db.RunActivities.Add(
                new RunActivity
                {
                    UserProfileId = 1,
                    Location = "Rcd Silang",
                    Started = new DateTime(2024, 7, 3, 14, 17, 5),
                    Ended = new DateTime(2024, 7, 3, 14, 56, 3),
                    Distance = 12
                });
            _db.SaveChanges();
        }
        
    }
    
}