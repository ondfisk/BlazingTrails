using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazingTrails.Api.Persistence.Entities;

public class Trail
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? Image { get; set; }
    public required string Location { get; set; }
    public int TimeInMinutes { get; set; }
    public int Length { get; set; }
    public ICollection<RouteInstruction> Route { get; set; } = new List<RouteInstruction>();
}

public class TrailConfig : IEntityTypeConfiguration<Trail>
{
    public void Configure(EntityTypeBuilder<Trail> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(250);
        builder.Property(x => x.Image).HasMaxLength(100);
        builder.Property(x => x.Location).HasMaxLength(50);
    }
}