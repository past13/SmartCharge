using Microsoft.EntityFrameworkCore;
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
        modelBuilder.Entity<GroupEntity>(entity =>
        {
            entity.HasKey(g => g.Id);

            entity.Property(g => g.RowVersion)
                .IsRowVersion();
        });
        
        modelBuilder.Entity<ChargeStationEntity>(entity =>
        {
            entity.HasKey(g => g.Id);
           
            entity.Property(g => g.RowVersion)
                .IsRowVersion();
            
            entity.HasOne(cs => cs.GroupEntity)
                .WithMany(g => g.ChargeStations)
                .HasForeignKey(cs => cs.GroupId);
        });
        
        modelBuilder.Entity<ConnectorEntity>(entity =>
        {
            entity.HasKey(g => g.Id);
            
            entity.Property(g => g.RowVersion)
                .IsRowVersion();
            
            entity.HasOne(cs => cs.ChargeStation)
                .WithMany(g => g.Connectors)
                .HasForeignKey(cs => cs.ChargeStationId);
        });
    }
}