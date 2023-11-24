namespace BlazingTrails.Api.Endpoints;

public static class CreateTrailsEndpoint
{
    public static async Task<Results<Created, ValidationProblem>> Invoke(BlazingTrailsContext context, TrailDTO trail, TrailValidator validator, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(trail, cancellationToken);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        // To test server-side validation uncomment the following line
        //return TypedResults.ValidationProblem(new Dictionary<string, string[]> { { "Name", new[] { "Name already exists" } } });

        var entity = new Trail
        {
            Name = trail.Name,
            Description = trail.Description,
            Location = trail.Location,
            TimeInMinutes = trail.TimeInMinutes,
            Length = trail.Length,
            Route = trail.Route.Select(x => new RouteInstruction { Stage = x.Stage, Description = x.Description, }).ToList(),
        };

        context.Trails.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Created($"/api/v1/trails/{entity.Id}");
    }
}