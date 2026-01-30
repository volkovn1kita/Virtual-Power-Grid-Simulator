using System;
using Microsoft.EntityFrameworkCore;
using Virtual_Power_Grid_Simulator.Domain.Entities;

namespace Virtual_Power_Grid_Simulator.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<PowerPlant> PowerPlants { get; set; }
    public DbSet<PowerConsumer> PowerConsumers { get; set; }
    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PowerPlant>().HasKey(p => p.Id);
        modelBuilder.Entity<PowerConsumer>().HasKey(c => c.Id);
        
        base.OnModelCreating(modelBuilder);
    }
}
