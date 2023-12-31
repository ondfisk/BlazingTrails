namespace BlazingTrails.Server.Endpoints;

public static class UploadTrailImageEndpoint
{
    public static async Task<Results<Created, NotFound<string>, ValidationProblem>> Invoke(BlazingTrailsContext context, Guid trailId, IFormFile file, FileValidator validator, CancellationToken cancellationToken)
    {
        var trail = await context.Trails.FindAsync(trailId, cancellationToken);

        if (trail is null)
        {
            return TypedResults.NotFound("Trail not found");
        }

        var validationResult = await validator.ValidateAsync(file, cancellationToken);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var fileName = $"{Guid.NewGuid()}.jpg";

        var saveLocation = Path.Combine(Directory.GetCurrentDirectory(), "images", fileName);

        var resizeOptions = new ResizeOptions
        {
            Mode = ResizeMode.Pad,
            Size = new Size(640, 426)
        };

        using var image = Image.Load(file.OpenReadStream());
        image.Mutate(x => x.Resize(resizeOptions));
        await image.SaveAsJpegAsync(saveLocation, cancellationToken: cancellationToken);

        trail.Image = fileName;
        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Created($"/images/{fileName}");
    }
}

public class FileValidator : AbstractValidator<IFormFile>
{
    public FileValidator()
    {
        RuleFor(x => x.Length).LessThan(1024 * 1024 * 5).WithMessage("File size must be less than 5MB");
        RuleFor(x => x.ContentType).Must(x => x == "image/jpeg" || x == "image/png").WithMessage("File must be a jpg or png");
    }
}