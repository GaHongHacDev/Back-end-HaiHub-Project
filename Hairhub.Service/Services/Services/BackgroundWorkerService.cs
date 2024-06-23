using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Domain.Entitities;
using Hairhub.Service.Services.IServices;
using Hairhub.Domain.Dtos.Requests.Otps;
using Microsoft.Extensions.Configuration;


namespace Hairhub.Service.Services.Services
{
    public class BackgroundWorkerService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BackgroundWorkerService> _logger;
        private readonly IConfiguration _configuration;
        


        public BackgroundWorkerService(IServiceScopeFactory scopeFactory, ILogger<BackgroundWorkerService> logger, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _configuration = configuration;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndExpireAccounts(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Check every hour
            }
        }

        private async Task CheckAndExpireAccounts(CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var salons = await uow.GetRepository<SalonInformation>().GetListAsync();

                    foreach (var salon in salons)
                    {
                        var latestPayment = await uow.GetRepository<Payment>()
                            .SingleOrDefaultAsync(
                                predicate: p => p.SalonOwner.Id == salon.SalonOwner.Id,
                                orderBy: o => o.OrderByDescending(o => o.EndDate)
                            );

                        if (latestPayment != null)
                        {
                            if (latestPayment.EndDate > DateTime.Now)
                            {
                                continue;
                            }

                            var daysToExpiry = (int)(latestPayment.EndDate - DateTime.Now).TotalDays;

                            if (daysToExpiry < 3 && daysToExpiry > 0)
                            {
                                await emailService.SendEmailAsyncNotifyOfExpired(salon.SalonOwner.Email, salon.SalonOwner.FullName, daysToExpiry, latestPayment.EndDate, _configuration["EmailPayment:LinkPayment"]);
                            }

                            if (latestPayment.EndDate < DateTime.Now && salon.Status != "DISABLED")
                            {
                                salon.Status = "DISABLED";
                            }
                        }
                    }

                    _logger.LogInformation("Expired salons checked and updated at: {time}", DateTimeOffset.Now);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CheckAndExpireAccounts");
                // Thực hiện xử lý lỗi tại đây nếu cần thiết
            }
        }

    }
}
