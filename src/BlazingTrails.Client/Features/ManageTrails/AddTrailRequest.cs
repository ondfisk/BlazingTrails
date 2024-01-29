using System.Net.Http.Json;
using BlazingTrails.Shared.Features.ManageTrails;
using FluentValidation;
using MediatR;

namespace BlazingTrails.Client.Features.ManageTrails;

public record AddTrailRequest(TrailDTO Trail) : IRequest<AddTrailRequest.Response>
{
    public const string RouteTemplate = "/api/v1/trails";

    public record Response(Guid TrailId)
    {
        public bool Success => TrailId != Guid.Empty;
    }
}

public class AddTrailRequestValidator : AbstractValidator<AddTrailRequest>
{
    public AddTrailRequestValidator()
    {
        RuleFor(x => x.Trail).SetValidator(new TrailValidator());
    }
}

public class AddTrailRequestHandler(HttpClient httpClient) : IRequestHandler<AddTrailRequest, AddTrailRequest.Response>
{
    public async Task<AddTrailRequest.Response> Handle(AddTrailRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(AddTrailRequest.RouteTemplate, request.Trail, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var trailId = await response.Content.ReadFromJsonAsync<Guid>(cancellationToken);

            return new(trailId);
        }

        return new(Guid.Empty);
    }
}
