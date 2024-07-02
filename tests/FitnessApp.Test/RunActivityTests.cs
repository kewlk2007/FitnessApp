using AutoFixture;
using Fitnessapp.Api.Contracts;
using Fitnessapp.Api.Entities;
using Fitnessapp.Api.Features.RunActivities;
using Fitnessapp.Api.Features.UserProfiles;
using Fitnessapp.Api.Repositories;
using Fitnessapp.Api.Shared;
using FluentAssertions;
using MediatR;
using Moq;
namespace FitnessApp.Test
{
    public class RunActivityTests
    {
        private Mock<IRunActivityRepository> _runActivityRepoMock;
        private Fixture _fixture;


        public RunActivityTests()
        {
            _fixture = new Fixture();
            _runActivityRepoMock = new Mock<IRunActivityRepository>();
        }

        [Fact]
        public async Task CreateRunActivity_Should_ReturnValue()
        {
            var runActivityResponse = new RunActivityResponse();
            _runActivityRepoMock.Setup(repo => repo.CreateRunActivity(
                                        new CreateRunActivityRequest()
                                        {
                                            UserProfileId = 1,
                                            Location = "Rcd silang",
                                            Started = new DateTime(2024, 7, 3, 14, 17, 5),
                                            Ended = new DateTime(2024, 7, 3, 14, 56, 3),
                                            Distance = 12
                                        }).Result)
                                 .Returns(runActivityResponse);

            var mediator = new Mock<Mediator>();
            CreateRunActivity.Command command = new CreateRunActivity.Command()
            {
                UserProfileId = 1,
                Location = "Rcd silang",
                Started = new DateTime(2024, 7, 3, 14, 17, 5),
                Ended = new DateTime(2024, 7, 3, 14, 56, 3),
                Distance = 12
            };
            CreateRunActivity.Validator validator = new CreateRunActivity.Validator();
            CreateRunActivity.Handler handler = new CreateRunActivity.Handler(_runActivityRepoMock.Object, validator);

            //Act
            Result<int> result = await handler.Handle(command, default);

            //Assert
            result.Value.Should().Be(-999);

        }

        [Fact]
        public async Task CreateRunActivity_Should_ReturnFailureResult_WhenLocationIsEmpty()
        {
            //Arrange
            var runActivityResponse = new RunActivityResponse();
            _runActivityRepoMock.Setup(repo => repo.CreateRunActivity(
                                        new CreateRunActivityRequest()
                                        {
                                            UserProfileId = 1,
                                            Location = "Rcd silang",
                                            Started = new DateTime(2024, 7, 3, 14, 17, 5),
                                            Ended = new DateTime(2024, 7, 3, 14, 56, 3),
                                            Distance = 12
                                        }).Result)
                                 .Returns(runActivityResponse);

            var mediator = new Mock<Mediator>();
            CreateRunActivity.Command command = new CreateRunActivity.Command()
            {
                UserProfileId = 1,
                Location = "",
                Started = new DateTime(2024, 7, 3, 14, 17, 5),
                Ended = new DateTime(2024, 7, 3, 14, 56, 3),
                Distance = 12
            };
            CreateRunActivity.Validator validator = new CreateRunActivity.Validator();
            CreateRunActivity.Handler handler = new CreateRunActivity.Handler(_runActivityRepoMock.Object, validator);

            //Act
            Result<int> result = await handler.Handle(command, default);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("CreateRunActivity.Validation");
        }

        [Fact]
        public async Task GetRunActivityByUserProfileId_Should_ReturnResult()
        {
            //Arrange
            var userProfileId = 1;
            var runActivityList = _fixture.CreateMany<RunActivityResponse>(10).ToList();
            
            foreach(var runAct  in runActivityList)
            {
                runAct.UserProfileId = userProfileId;
            }

            _runActivityRepoMock.Setup(repo => repo.GetByUserProfileId(userProfileId,default).Result)
                                 .Returns(runActivityList);

            var mediator = new Mock<Mediator>();
            GetRunActivities.Query query = new GetRunActivities.Query() { UserProfileId = userProfileId };
            GetRunActivities.Handler handler = new GetRunActivities.Handler(_runActivityRepoMock.Object);

            //Act
            Result<List<RunActivityResponse>> result = await handler.Handle(query, default);

            //Assert
            result.Value.Should().NotBeNull();
            result.Value.Count().Should().BeGreaterThan(0);
        }
    }
}