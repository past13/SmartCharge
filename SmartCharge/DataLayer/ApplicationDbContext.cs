using Microsoft.EntityFrameworkCore;
using SmartCharge.Domain;
using SmartCharge.Domain.Entities;

namespace SmartCharge.DataLayer;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<GroupEntity> Groups { get; set; }
    public DbSet<ChargeStationEntity> ChargeStations { get; set; }
    public DbSet<ConnectorEntity> Connector { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupEntity>()
            .HasKey(g => g.Id);

        modelBuilder.Entity<GroupEntity>()
            .Property(g => g.RowVersion)
            .IsRowVersion();
        
        modelBuilder.Entity<ChargeStationEntity>()
            .HasKey(cs => cs.Id);

        modelBuilder.Entity<ChargeStationEntity>()
            .Property(g => g.RowVersion)
            .IsRowVersion();
        
        modelBuilder.Entity<ChargeStationEntity>()
            .HasOne(cs => cs.GroupEntity)
            .WithMany(g => g.ChargeStations)
            .HasForeignKey(cs => cs.GroupId);
        
        modelBuilder.Entity<ConnectorEntity>()
            .HasKey(g => g.Id);

        modelBuilder.Entity<ConnectorEntity>()
            .Property(g => g.RowVersion)
            .IsRowVersion();
        
        modelBuilder.Entity<ConnectorEntity>()
            .HasOne(cs => cs.ChargeStation)
            .WithMany(g => g.Connectors)
            .HasForeignKey(cs => cs.ChargeStationId);
    }
}