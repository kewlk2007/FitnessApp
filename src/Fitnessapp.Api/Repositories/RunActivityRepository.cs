using Azure.Core;
using Fitnessapp.Api.Contracts;
using Fitnessapp.Api.Database;
using Fitnessapp.Api.Entities;
using Fitnessapp.Api.Features.UserProfiles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Fitnessapp.Api.Repositories
{
    public interface IRunActivityRepository
    {
        Task<RunActivityResponse?> CreateRunActivity(CreateRunActivityRequest request);
        Task<List<RunActivityResponse>?> GetByUserProfileId(int userProfileId, CancellationToken cancellationToken);
    }

    public class RunActivityRepository : IRunActivityRepository
    {
        private ApplicationDbContext _dbContext;

        public RunActivityRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RunActivityResponse?> CreateRunActivity(CreateRunActivityRequest request)
        {
            var runactivity = new RunActivity()
            {
                UserProfileId = request.UserProfileId,
                Location = request.Location,
                Started = request.Started,
                Ended = request.Ended,
                Distance = request.Distance
            };

            _dbContext.Add(runactivity);

            await _dbContext.SaveChangesAsync();

            return new RunActivityResponse()
            {
                Id = runactivity.Id,
                UserProfileId = runactivity.UserProfileId,
                Location = runactivity.Location,
                Started = runactivity.Started,
                Ended = runactivity.Ended,
                Distance = runactivity.Distance
            };
        }     

        public async Task<List<RunActivityResponse>?> GetByUserProfileId(int userProfileId, CancellationToken cancellationToken)
        {
            return await _dbContext
                        .RunActivities
                        .Where(runacvty => runacvty.UserProfileId == userProfileId)
                        .Select(runacvty => new RunActivityResponse()
                        {
                            Id = runacvty.Id,
                            UserProfileId = runacvty.UserProfileId,
                            Location = runacvty.Location,
                            Started = runacvty.Started,
                            Ended = runacvty.Ended,
                            Distance = runacvty.Distance
                        })
                        .ToListAsync(cancellationToken);
        }
    }
}
