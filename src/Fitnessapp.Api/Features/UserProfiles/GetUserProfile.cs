using Carter;
using Fitnessapp.Api.Contracts;
using Fitnessapp.Api.Database;
using Fitnessapp.Api.Repositories;
using Fitnessapp.Api.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Fitnessapp.Api.Features.UserProfiles
{
    public static class GetUserProfile
    {
        public class Query : IRequest<Result<UserProfileResponse>>
        {
            public int Id { get; set; }
        }
        internal sealed class Handler : IRequestHandler<Query, Result<UserProfileResponse>>
        {
            private readonly IUserProfileRepository _userProfileRepository;
            public Handler(IUserProfileRepository userProfileRepository)
            {
                _userProfileRepository = userProfileRepository;
            }

            public async Task<Result<UserProfileResponse>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userProfileResponse = await _userProfileRepository.GetById(request.Id, default);
                
                if(userProfileResponse is null) 
                {
                    Log.Error($"The userprofile with the specified ID of {request.Id} was not found", request);
                    return Result.Failure<UserProfileResponse>(new Error(
                        "GetUserProfile.Null",
                        "The userprofile with the specified ID was not found"));
                }

                Log.Information($"GetUserProfile:", userProfileResponse);
                return userProfileResponse;
            }
           
        }
    }

    public class GetUserProfileEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/userprofiles/{id}", async (int id, ISender sender) =>
            {
                var query = new GetUserProfile.Query { Id = id };

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
