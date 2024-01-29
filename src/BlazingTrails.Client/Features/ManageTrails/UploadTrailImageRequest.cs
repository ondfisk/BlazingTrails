using FluentValidation;
using MediatR;

namespace BlazingTrails.Client.Features.ManageTrails;

public record UploadTrailImageRequest(Guid TrailId, IBrowserFile File) : IRequest<UploadTrailImageRequest.Response>
{
    public const string RouteTemplate = "/api/v1/trails/{trailId}/images";

    public record Response(string ImageName)
    {
        public bool Success => !string.IsNullOrWhiteSpace(ImageName);
    }
}

public class FileValidator : AbstractValidator<IBrowserFile>
{
    public FileValidator()
    {
        RuleFor(x => x.Size).LessThan(1024 * 1024 * 2).WithMessage("File size must be less than 2MB");
        RuleFor(x => x.ContentType).Must(x => x == "image/jpeg" || x == "image/png").WithMessage("File must be a jpg or a png");
    }
}

public class UploadTrailImageRequestValidator : AbstractValidator<UploadTrailImageRequest>
{
    public UploadTrailImageRequestValidator()
    {
        RuleFor(x => x.File).SetValidator(new FileValidator());
    }
}

public class UploadTrailImageHandler(HttpClient httpClient) : IRequestHandler<UploadTrailImageRequest, UploadTrailImageRequest.Response>
{
    public async Task<UploadTrailImageRequest.Response> Handle(UploadTrailImageRequest request, CancellationToken cancellationToken)
    {
        var trailImage = request.File;
        using var formData = new MultipartFormDataContent();
        formData.Headers.Add("X-XSRF-TOKEN", await httpClient.GetStringAsync("/api/v1/antiforgery/token", cancellationToken));
        var stream = trailImage.OpenReadStream(trailImage.Size, cancellationToken);
        var content = new StreamContent(stream);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(trailImage.ContentType);
        formData.Add(content, "file", trailImage.Name);

        var response = await httpClient.PostAsync(UploadTrailImageRequest.RouteTemplate.Replace("{trailId}", request.TrailId.ToString()), formData, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var imageName = await response.Content.ReadAsStringAsync(cancellationToken);

            return new(imageName);
        }

        return new(string.Empty);
    }
}