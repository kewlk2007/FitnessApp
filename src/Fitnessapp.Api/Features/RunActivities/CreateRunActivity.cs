using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using MediatR;
using Fitnessapp.Api.Shared;
using FluentValidation;
using Fitnessapp.Api.Database;
using Azure.Core;
using Fitnessapp.Api.Entities;
using Carter;
using Fitnessapp.Api.Contracts;
using Mapster;
using Serilog;
using Fitnessapp.Api.Repositories;

namespace Fitnessapp.Api.Features.RunActivities
{
    public static class CreateRunActivity
    {
        public class Command : IRequest<Result<int>>
        {
            public int Id { get; set; }
            public int UserProfileId { get; set; }
            public string Location { get; set; } = string.Empty;
            public DateTime? Started { get; set; } = null;
            public DateTime? Ended { get; set; } = null;
            public int Distance { get; set; } = 0;
        }

        public class Validator: AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(c=>c.Location).NotEmpty();
                RuleFor(c=>c.UserProfileId).NotEmpty().NotNull();
                RuleFor(c=>c.Started).NotEmpty().NotNull();
                RuleFor(c=>c.Ended).NotEmpty().NotNull();
                RuleFor(c=>c.Distance).NotEmpty().NotNull().GreaterThan(0);
            }
        }

        internal sealed class Handler: IRequestHandler<Command, Result<int>>
        {
            private readonly IRunActivityRepository _runActivityRepository;
            private readonly IValidator<Command> _validator;

            public Handler(IRunActivityRepository runActivityRepository, IValidator<Command> validator)
            {
                _runActivityRepository = runActivityRepository;
                _validator = validator;
            }

            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    Log.Error($"CreateRunActivityError:CreateRunActivity.Validation", validationResult.ToString());
                    return Result.Failure<int>(new Error(
                        "CreateRunActivity.Validation",
                        validationResult.ToString()));
                }

                var createResult = await _runActivityRepository.CreateRunActivity(new CreateRunActivityRequest()
                {
                    UserProfileId = request.UserProfileId,
                    Location = request.Location,
                    Started = request.Started,
                    Ended = request.Ended,
                    Distance = request.Distance
                });

                Log.Information($"CreateRunActivity:{createResult}", createResult);
                return (createResult is null) ? -999 : createResult.Id;
            }
        }
        public class CreateRunActivityEndpoint : ICarterModule
        {
            public void AddRoutes(IEndpointRouteBuilder app)
            {
                app.MapPost("api/runactivities", async (CreateRunActivityRequest request, ISender sender) =>
                {
                    var command = request.Adapt<CreateRunActivity.Command>();

                    var result = await sender.Send(command);

                    if (result.IsFailure)
                    {
                        return Results.BadRequest(result.Error);
                    }

                    return Results.Ok(result.Value);
                });
            }
        }
    }
}
