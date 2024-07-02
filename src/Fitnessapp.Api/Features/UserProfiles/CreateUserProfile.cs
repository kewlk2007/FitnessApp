using Carter;
using Fitnessapp.Api.Contracts;
using Fitnessapp.Api.Repositories;
using Fitnessapp.Api.Shared;
using FluentValidation;
using Mapster;
using MediatR;
using Serilog;

namespace Fitnessapp.Api.Features.UserProfiles
{
    public static class CreateUserProfile
    {
        public class Command : IRequest<Result<int>>
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Weight { get; set; } = 0;
            public decimal Height { get; set; } = 0;
            public DateTime BirthDate { get; set; } = DateTime.Now;
        }

        public class Validator: AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(c => c.Name).NotEmpty();
                RuleFor(c=>c.Weight).GreaterThan(0).NotEmpty().NotNull();
                RuleFor(c=>c.Height).GreaterThan(0).NotEmpty().NotNull();
                RuleFor(c=>c.BirthDate).LessThanOrEqualTo(DateTime.Now).NotEmpty();
            }
        }

        internal sealed class Handler: IRequestHandler<Command, Result<int>>
        {
            private readonly IUserProfileRepository _userProfileRepository;
            private readonly IValidator<Command> _validator;

            public Handler(IUserProfileRepository userProfileRepository, IValidator<Command> validator)
            {
                _userProfileRepository = (IUserProfileRepository)userProfileRepository;
                _validator = validator;
            }
            
            public async Task<Result<int>?> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(request);
                if(!validationResult.IsValid)
                {
                    Log.Error($"CreateUserProfileError:CreateUserProfile.Validation", validationResult.ToString());
                    return Result.Failure<int>(new Error(
                        "CreateUserProfile.Validation",
                        validationResult.ToString()));
                }

                if(await _userProfileRepository.GetByName(request.Name, default) is not null)
                {
                    Log.Error($"CreateUserProfileError:{request.Name}", Error.NameAlreadyInUse);
                    return Result.Failure<int>(Error.NameAlreadyInUse);
                }

                var createResult = await _userProfileRepository.CreateUserProfile(new CreateUserProfileRequest
                {
                    Name = request.Name,
                    Weight = request.Weight,
                    Height = request.Height,
                    BirthDate = request.BirthDate
                });

                Log.Information($"CreateUserProfile:{createResult}", createResult);
                return (createResult is null)?-999:createResult.Id;
            }
        }
    }
    public class CreateUserProfileEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/userprofiles", async (CreateUserProfileRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateUserProfile.Command>();

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