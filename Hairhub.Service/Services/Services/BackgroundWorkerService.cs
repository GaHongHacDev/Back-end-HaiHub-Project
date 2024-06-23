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
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;

        public BackgroundWorkerService(IServiceScopeFactory scopeFactory, ILogger<BackgroundWorkerService> logger, IUnitOfWork uow, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _uow = uow;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BackgroundWorkerService is starting.");
            await ExecuteAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BackgroundWorkerService is stopping.");
            await Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndExpireAccounts(stoppingToken);
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Check every hour
            }
        }

        private async Task CheckAndExpireAccounts(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var salons = await _uow.GetRepository<SalonInformation>().GetListAsync();

                foreach (var salon in salons)
                {
                    var latestPayment = await _uow.GetRepository<Payment>().SingleOrDefaultAsync(
                        predicate: p => p.Id == salon.SalonOwner.Id,
                        orderBy: o => o.OrderByDescending(p => p.EndDate)
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
    }
}
