namespace BlazingTrails.Server.Persistence.Entities;

public class RouteInstruction
{
    public Guid Id { get; set; }
    public Guid TrailId { get; set; }
    public int Stage { get; set; }
    public required string Description { get; set; }
    public Trail Trail { get; set; } = default!;
}

public class RouteInstructionConfig : IEntityTypeConfiguration<RouteInstruction>
{
    public void Configure(EntityTypeBuilder<RouteInstruction> builder)
    {
        builder.Property(x => x.Description).HasMaxLength(250);
    }
}