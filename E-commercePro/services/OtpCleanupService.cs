using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E_commercePro.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace E_commercePro.Services
{
    public class OtpCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OtpCleanupService> _logger;

        public OtpCleanupService(IServiceScopeFactory scopeFactory, ILogger<OtpCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("OtpCleanupService is running.");

                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var currentTime = DateTime.UtcNow;

                    var expiredOtps = await dbContext.OtpDetails
                        .Where(o => o.ExpiryDateTime <= currentTime)
                        .ToListAsync();

                    dbContext.OtpDetails.RemoveRange(expiredOtps);
                    await dbContext.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
           }
        }
    }
}
