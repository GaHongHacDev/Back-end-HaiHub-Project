using AutoMapper;
using Hairhub.API.Hubs;
using Hairhub.Domain.Dtos.Requests.Notification;
using Hairhub.Domain.Dtos.Responses.Notification;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet;

namespace Hairhub.Service.Services.Services
{
    public class NotificationService : INotificationService
    {
        public readonly IUnitOfWork _unitofwork;
        public readonly IMapper _mapper;
        private readonly IHubContext<BookAppointmentHub> _hubContext;

        public NotificationService(IUnitOfWork unitofwork, IMapper mapper, IHubContext<BookAppointmentHub> hubContext)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        public async Task<bool> CreatedNotification(Guid salonid, NotificationRequest request)
        {
            var salon = await _unitofwork.GetRepository<SalonInformation>()
                                .SingleOrDefaultAsync(predicate: p => p.Id == salonid);
            if (salon == null) { throw new Exception("Salon không tồn tại"); }
            List<SalonEmployee> accounts = new List<SalonEmployee>();


            var appointmentDetails = await _unitofwork.GetRepository<AppointmentDetail>()
          .GetListAsync(
               predicate: x => x.AppointmentId == request.AppointmentId,
              include: query => query.Include(s => s.SalonEmployee).Include(s => s.ServiceHair).Include(s => s.Appointment)
          );


            var appointment = await _unitofwork.GetRepository<Appointment>()
                                               .SingleOrDefaultAsync(
                                                   predicate: p => p.Id == request.AppointmentId
                                               );

            var customerName = await _unitofwork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: p => p.Id == appointment.CustomerId);

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Title = request.Title!,
                Message = request.Message!,
                CreatedDate = DateTime.Now,
                Type = request.Type!,
            };
            await _unitofwork.GetRepository<Notification>().InsertAsync(notification);
            List<NotificationDetail> list = new List<NotificationDetail>();

            foreach (var employee in appointmentDetails)
            {
                var notidetail = new NotificationDetail
                {
                    Id = Guid.NewGuid(),
                    NotificationId = notification.Id,
                    AccountId = employee.SalonEmployee.AccountId, 
                    AppointmentId = appointment.Id,
                    IsRead = false,
                    ReadDate = DateTime.Now,
                };

                list.Add(notidetail);
            }
            var notiSalon = new NotificationDetail
            {
                Id = Guid.NewGuid(),
                NotificationId = notification.Id,
                AccountId = salon.SalonOwner.AccountId, 
                AppointmentId = appointment.Id,
                IsRead = false,
                ReadDate = DateTime.Now,
            };
            list.Add(notiSalon);
            var notiCustomer = new NotificationDetail
            {
                Id = Guid.NewGuid(),
                NotificationId = notification.Id,
                AccountId = customerName.AccountId, 
                AppointmentId = appointment.Id,
                IsRead = false,
                ReadDate = DateTime.Now,
            };
            list.Add(notiCustomer);
            await _hubContext.Clients.All
                .SendAsync("ReceiveNotification", new
                {
                    Title = request.Title,
                    Message = request.Message,
                    list,
                    apps = appointment.Id,
                    customer = customerName.FullName,
                    CreatedDate = DateTime.Now
                });
            await _unitofwork.GetRepository<NotificationDetail>().InsertRangeAsync(list);
            bool isSuccessed = await _unitofwork.CommitAsync() > 0;
            return isSuccessed;
        }

        public async Task<IPaginate<NotificationResponse>> GetNotification(Guid accountid, int page, int size)
        {
            var account = await _unitofwork.GetRepository<NotificationDetail>()
                                .GetPagingListAsync(predicate: p => p.AccountId == accountid,
                                                    page: page, size: size,
                                                    include: i => i.Include(p => p.Account).Include(p => p.Appointment).ThenInclude(p => p.Customer).Include(p => p.Notification)

                                );
            if (account == null) { throw new Exception("Salon không tồn tại"); }
            var paginateResponse = new Paginate<NotificationResponse>
            {
                Page = account.Page,
                Size = account.Size,
                Total = account.Total,
                TotalPages = account.TotalPages,
                Items = _mapper.Map<IList<NotificationResponse>>(account.Items)
            };

            return paginateResponse;
        }

        public async Task<bool> ReadedNotification(Guid notiId)
        {
            var notification = await _unitofwork.GetRepository<NotificationDetail>().SingleOrDefaultAsync(predicate: p => p.Id == notiId);
            if (notification == null)  throw new Exception("Salon không tồn tại"); 
            notification.IsRead = true;
            notification.ReadDate = DateTime.Now;
            notification.Id = notiId;
            _unitofwork.GetRepository<NotificationDetail>().UpdateAsync(notification);
            bool isSuccessed = await _unitofwork.CommitAsync() > 0;
            return isSuccessed;
        }
    }
}
