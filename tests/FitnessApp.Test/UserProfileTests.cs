using AutoFixture;
using Fitnessapp.Api.Contracts;
using Fitnessapp.Api.Entities;
using Fitnessapp.Api.Features.UserProfiles;
using Fitnessapp.Api.Repositories;
using Fitnessapp.Api.Shared;
using FluentAssertions;
using MediatR;
using Moq;
namespace FitnessApp.Test
{
    public class CreateUserProfileTests
    {
        private Mock<IUserProfileRepository> _userProfileRepoMock;
        private Fixture _fixture;


        public CreateUserProfileTests()
        {
            _fixture = new Fixture();
            _userProfileRepoMock = new Mock<IUserProfileRepository>();
        }

        [Fact]
        public async Task CreateUserProfile_Should_ReturnValue()
        {
            var userProfileResponse = new UserProfileResponse();
            var newUserProfile = _fixture.Create<UserProfile>();
            _userProfileRepoMock.Setup(repo => repo.CreateUserProfile(
                                        new CreateUserProfileRequest()
                                        {
                                            Name = newUserProfile.Name,
                                            BirthDate = newUserProfile.BirthDate,
                                            Height = newUserProfile.Height,
                                            Weight = newUserProfile.Weight
                                        }).Result)
                                 .Returns(userProfileResponse);

            var mediator = new Mock<Mediator>();
            CreateUserProfile.Command command = new CreateUserProfile.Command()
            {
                Name = newUserProfile.Name,
                BirthDate = DateTime.Parse("1982/04/05"),
                Height = newUserProfile.Height,
                Weight = newUserProfile.Weight
            };
            CreateUserProfile.Validator validator = new CreateUserProfile.Validator();
            CreateUserProfile.Handler handler = new CreateUserProfile.Handler(_userProfileRepoMock.Object, validator);

            //Act
            Result<int> result = await handler.Handle(command, default);

            //Assert
            result.Value.Should().Be(-999); //reached the end of the command

        }

        [Fact]
        public async Task CreateUserProfile_Should_ReturnFailureResult_WhenNameIsNotUnique()
        {
            //Arrange
            var userProfileResponse = new UserProfile();
            _userProfileRepoMock.Setup(repo => repo.GetByName("ervin miranda", default).Result)
                                 .Returns(userProfileResponse);

            var mediator = new Mock<Mediator>();
            CreateUserProfile.Command command = new CreateUserProfile.Command()
            {
                Name = "ervin miranda",
                BirthDate = DateTime.Parse("1982/04/05"),
                Height = 172,
                Weight = 83
            };
            CreateUserProfile.Validator validator = new CreateUserProfile.Validator();
            CreateUserProfile.Handler handler = new CreateUserProfile.Handler(_userProfileRepoMock.Object, validator);

            //Act
            Result<int> result = await handler.Handle(command, default);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.NameAlreadyInUse);
        }

        [Fact]
        public async Task GetUserProfileById_Should_ReturnResult()
        {
            //Arrange
            var userProfileId = 1;
            var userProfile = new UserProfileResponse();
            _userProfileRepoMock.Setup(repo => repo.GetById(userProfileId, default).Result)
                                .Returns(userProfile);

            var mediator = new Mock<Mediator>();
            GetUserProfile.Query query = new GetUserProfile.Query() { Id = userProfileId };
            GetUserProfile.Handler handler = new GetUserProfile.Handler(_userProfileRepoMock.Object);

            //Act
            Result<UserProfileResponse> result = await handler.Handle(query, default);

            //Assert
            result.Value.Should().NotBeNull();
        }
    }
}