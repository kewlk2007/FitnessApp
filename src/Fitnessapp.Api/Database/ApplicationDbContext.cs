using Fitnessapp.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Fitnessapp.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }

        public DbSet<RunActivity> RunActivities { get; set; }

    }
}
