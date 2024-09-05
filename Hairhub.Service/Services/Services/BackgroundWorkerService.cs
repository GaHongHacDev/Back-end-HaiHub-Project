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
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Hairhub.Domain.Enums;
using Hairhub.Common.ThirdParties.Contract;

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
            //while (!stoppingToken.IsCancellationRequested)
            //{

            //    var now = DateTime.Now;
            //    var nextMidnight = now.Date.AddDays(1);
            //    var delayTime = nextMidnight - now;


            //    await Task.Delay(delayTime, stoppingToken);

            //    await ExecuteExpiredSalon(stoppingToken);


            //    await ExecuteExpriredAppointment(stoppingToken);
            //}
            while (!stoppingToken.IsCancellationRequested)
            {
                // Thực hiện công việc của bạn ở đây
                await ExecuteExpiredSalon(stoppingToken);
                await ExecuteExpriredAppointment(stoppingToken);

                // Đợi 30 giây
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private async Task ExecuteExpriredAppointment(CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var mediaService = scope.ServiceProvider.GetRequiredService<IMediaService>();

                    var appontments = await uow.GetRepository<Appointment>().GetListAsync(
                        predicate: x => x.Status.Equals(AppointmentStatus.Booking)
                                && x.StartDate == DateTime.Now.AddDays(-1),
                        include: x => x.Include(s => s.AppointmentDetails)
                    );

                    foreach (var appointment in appontments)
                    {
                        foreach(var appointmentDetail in appointment.AppointmentDetails)
                        {
                            appointmentDetail.Status = AppointmentStatus.Fail;
                            uow.GetRepository<AppointmentDetail>().UpdateAsync(appointmentDetail);
                        }
                        //Delete QR image
                        await mediaService.DeleteImageAsync(appointment!.QrCodeImg!, MediaPath.QR_APPOINTMENT);
                        appointment.Status = AppointmentStatus.Fail;
                        appointment.QrCodeImg = "";
                        uow.GetRepository<Appointment>().UpdateAsync(appointment);
                    }
                    uow.CommitAsync();
                    _logger.LogInformation("Expired salappointment checked and updated at: {time}", DateTimeOffset.Now);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CheckAndExpireAccounts");
                // Thực hiện xử lý lỗi tại đây nếu cần thiết
            }
        }
        private async Task ExecuteExpiredSalon(CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var salons = await uow.GetRepository<SalonInformation>().GetListAsync(
                        include: x => x.Include(s => s.SalonOwner),
                        predicate: p => p.Status == SalonStatus.Approved
                    );

                    foreach (var salon in salons)
                    {
                        var latestPayment = await uow.GetRepository<Payment>().SingleOrDefaultAsync(
                            predicate: p => p.SalonOwner.Id == salon.SalonOwner.Id,
                            orderBy: o => o.OrderByDescending(p => p.EndDate)
                        );

                        if (latestPayment != null)
                        {
                            var daysToExpiry = (int)(latestPayment.EndDate - DateTime.Now).TotalDays;
                            if (daysToExpiry < 5 && daysToExpiry > 0)
                            {
                                await emailService.SendEmailAsyncNotifyOfExpired(salon.SalonOwner.Email, salon.SalonOwner.FullName, daysToExpiry, latestPayment.EndDate, _configuration["EmailPayment:LinkPayment"]);
                            }                        
                            if(daysToExpiry == 0)
                            {
                                salon.Status = SalonStatus.Disable;
                            }
                            if (latestPayment.EndDate < DateTime.Now && salon.Status != SalonStatus.OverDue)
                            {
                                salon.Status = SalonStatus.OverDue;
                                uow.GetRepository<SalonInformation>().UpdateAsync(salon);
                                await uow.CommitAsync();
                            }
                        }
                    }

                    _logger.LogInformation("Expired salons checked and updated at: {time}", DateTimeOffset.Now);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CheckAndExpireAccounts");
            }
        }


    }
}
