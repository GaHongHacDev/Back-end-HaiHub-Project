using CloudinaryDotNet.Actions;
using Hairhub.Domain.Dtos.Requests.Notification;
using Hairhub.Domain.Dtos.Responses.Notification;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface INotificationService
    {
        Task<bool> CreatedNotification(Guid salonid, NotificationRequest request);
        Task<IPaginate<NotificationResponse>> GetNotification(Guid accountid, int page, int size);
        Task<bool> ReadedNotification(Guid notiId);
    }
}
