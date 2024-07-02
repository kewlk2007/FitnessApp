using Carter;
using Fitnessapp.Api.Contracts;
using Fitnessapp.Api.Database;
using Fitnessapp.Api.Repositories;
using Fitnessapp.Api.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fitnessapp.Api.Features.RunActivities
{
    public static class GetRunActivities
    {
        public class Query : IRequest<Result<List<RunActivityResponse>>>
        {
            public int UserProfileId { get; set; }
        }
        internal sealed class Handler : IRequestHandler<Query, Result<List<RunActivityResponse>>>
        {
            private readonly IRunActivityRepository _runActivityRepository;
            public Handler(IRunActivityRepository runActivityRepository)
            {
                _runActivityRepository = runActivityRepository;
            }

            public async Task<Result<List<RunActivityResponse>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var runActivityResponse = await _runActivityRepository.GetByUserProfileId(request.UserProfileId,cancellationToken);
                
                if(runActivityResponse is null) 
                {
                    return Result.Failure<List<RunActivityResponse>>(new Error(
                        "GetRunActivity.Null",
                        "Run activities with the specified UserProfile ID was not found or is empty"));
                }

                return runActivityResponse;
            }
           
        }
    }

    public class GetRunActivitiesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/runactivities/{userProfileId}", async (int userProfileId, ISender sender) =>
            {
                var query = new GetRunActivities.Query { UserProfileId = userProfileId };

                var result = await sender.Send(query);

                if (result.IsFailure)
                {
                    return Results.NotFound(result.Error);
                }

                return Results.Ok(result.Value);
            });
        }
    }
}
