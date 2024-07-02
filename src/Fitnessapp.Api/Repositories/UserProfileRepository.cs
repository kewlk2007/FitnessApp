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
    public interface IUserProfileRepository
    {
        Task<UserProfileResponse?> CreateUserProfile(CreateUserProfileRequest request);
        Task<UserProfileResponse?> GetById(int Id, CancellationToken cancellationToken);
        Task<UserProfile?> GetByName(string Name, CancellationToken cancellationToken);
    }

    public class UserProfileRepository : IUserProfileRepository
    {
        private ApplicationDbContext _dbContext;
        public UserProfileRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<UserProfileResponse?> CreateUserProfile(CreateUserProfileRequest request)
        {
            var userprofile = new UserProfile
            {
                Name = request.Name,
                Weight = request.Weight,
                Height = request.Height,
                BirthDate = request.BirthDate
            };

            _dbContext.Add(userprofile);
            
            await _dbContext.SaveChangesAsync();

            return new UserProfileResponse()
            {
                Id = userprofile.Id,
                Name = userprofile.Name,
                Weight = userprofile.Weight,
                Height = userprofile.Height,
                BirthDate = userprofile.BirthDate
            };
        }

        public async Task<UserProfileResponse?> GetById(int Id, CancellationToken cancellationToken)
        {
            return await _dbContext.UserProfiles
                                .Where(userprofile => userprofile.Id == Id)
                                .Select(userprofile => new UserProfileResponse()
                                {
                                    Id = userprofile.Id,
                                    Name = userprofile.Name,
                                    BirthDate = userprofile.BirthDate,
                                    Height = userprofile.Height,
                                    Weight = userprofile.Weight
                                })
                                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<UserProfile?> GetByName(string Name, CancellationToken cancellationToken)
        {
            return await _dbContext
                        .UserProfiles
                        .Where(f => f.Name.ToUpper() == Name.ToUpper())
                        .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
