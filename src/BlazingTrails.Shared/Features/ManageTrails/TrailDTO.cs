using System.Data;
using FluentValidation;

namespace BlazingTrails.Shared.Features.ManageTrails;

public class TrailDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Location { get; set; } = "";
    public int TimeInMinutes { get; set; }
    public int Length { get; set; }
    public ICollection<RouteInstruction> Route { get; set; } = [];
    public string? ImageUrl { get; set; }
    public int Climb { get; set; }
    public bool IsFavourite { get; set; }

    public class RouteInstruction
    {
        public int Stage { get; set; }
        public string Description { get; set; } = "";
    }
}

public class TrailValidator : AbstractValidator<TrailDTO>
{
    public TrailValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Please enter a name");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Please enter a description");
        RuleFor(x => x.Location).NotEmpty().WithMessage("Please enter a location");
        RuleFor(x => x.Length).GreaterThan(0).WithMessage("Please enter a length");
        RuleFor(x => x.Route).NotEmpty().WithMessage("Please add a route instruction");
        RuleForEach(x => x.Route).SetValidator(new RouteInstructionValidator());
    }
}

public class RouteInstructionValidator : AbstractValidator<TrailDTO.RouteInstruction>
{
    public RouteInstructionValidator()
    {
        RuleFor(x => x.Stage).NotEmpty().WithMessage("Please enter a stage");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Please enter a description");
    }
}