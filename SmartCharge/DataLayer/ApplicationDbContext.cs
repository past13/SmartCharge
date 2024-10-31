using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCharge.Domain.Entities;

namespace SmartCharge.DataLayer;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<GroupEntity> Groups { get; set; }
    public DbSet<ChargeStationEntity> ChargeStations { get; set; }
    public DbSet<ConnectorEntity> Connector { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupEntity>(entity =>
        {
            entity.HasKey(g => g.Id);
        
            entity.Property(e => e.RowVersion)
                .IsRowVersion();
            
            entity.HasMany(g => g.ChargeStations)
                .WithOne(cs => cs.GroupEntity)
                .HasForeignKey(cs => cs.GroupId);
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
            
            entity.Property(c => c.ConnectorNumber)
                .IsRequired()
                .HasConversion<int>();
            
            entity.HasIndex(c => new { c.ChargeStationId, c.ConnectorNumber })
                .IsUnique();

            entity.Property(g => g.RowVersion)
                .IsRowVersion();
            
            entity.HasOne(cs => cs.ChargeStation)
                .WithMany(g => g.Connectors)
                .HasForeignKey(cs => cs.ChargeStationId);
        });
    }
}