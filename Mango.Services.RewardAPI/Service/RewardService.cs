using Mango.Services.RewardAPI.Data;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.RewardAPI.Service
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDbContext> options;

        public RewardService(DbContextOptions<AppDbContext> options)
        {
            this.options = options;
        }

        public async Task UpdateRewards(RewardsMessage rewardsMessage)
        {
            try
            {
                Rewards rewards = new()
                {
                    OrderId = rewardsMessage.OrderId,
                    UserId = rewardsMessage.UserId,
                    RewardsActivity = rewardsMessage.RewardsActivity,
                    RewardsDate = DateTime.Now,
                };

                await using var _db = new AppDbContext(options);
                await _db.Rewards.AddAsync(rewards);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}
