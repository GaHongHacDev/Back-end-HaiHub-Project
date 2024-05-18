using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IPaginate<GetAppointmentResponse>> GetAllAppointment(int page, int size)
        {
            try
            {
                IPaginate<GetAppointmentResponse> appointmentsResponse = await _unitOfWork.GetRepository<Appointment>()
            .GetPagingListAsync(
                selector: x => new GetAppointmentResponse(
                    x.Id,
                    new CustomerInfomation
                    {
                        Id = x.Customer.Id,
                        AccountId = x.Customer.AccountId,
                        DayOfBirth = x.Customer.DayOfBirth,
                        Gender = x.Customer.Gender,
                        FullName = x.Customer.FullName,
                        Email = x.Customer.Email,
                        Phone = x.Customer.Phone,
                        Address = x.Customer.Address,
                        HumanId = x.Customer.HumanId,
                        Img = x.Customer.Img,
                        BankAccount = x.Customer.BankAccount,
                        BankName = x.Customer.BankName
                    },
                    x.Date,
                    x.TotalPrice,
                    x.IsActive),
                page: page,
                size: size,
                orderBy: x => x.OrderBy(x => x.Date),
                include: source => source.Include(a => a.Customer)
            );

                return appointmentsResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<GetAppointmentResponse>? GetAppointmentById(Guid id)
        {
            GetAppointmentResponse appointmentResponse = await _unitOfWork
                .GetRepository<Appointment>()
                .SingleOrDefaultAsync(
                    selector: x => new GetAppointmentResponse(x.Id,
                    new CustomerInfomation
                    {
                        Id = x.Customer.Id,
                        AccountId = x.Customer.AccountId,
                        DayOfBirth = x.Customer.DayOfBirth,
                        Gender = x.Customer.Gender,
                        FullName = x.Customer.FullName,
                        Email = x.Customer.Email,
                        Phone = x.Customer.Phone,
                        Address = x.Customer.Address,
                        HumanId = x.Customer.HumanId,
                        Img = x.Customer.Img,
                        BankAccount = x.Customer.BankAccount,
                        BankName = x.Customer.BankName
                    },
                    x.Date,
                    x.TotalPrice,
                    x.IsActive),
                    predicate: x => x.Id.Equals(id),
                    include: source => source.Include(a => a.Customer)
                 );
            if (appointmentResponse == null)
                return null;
            return appointmentResponse;
        }
    }
}
