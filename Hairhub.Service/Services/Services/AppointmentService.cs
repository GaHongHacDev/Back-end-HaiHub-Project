using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Exceptions;
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
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IPaginate<GetAppointmentResponse>> GetAllAppointment(int page, int size)
        {
            var schedules = await _unitOfWork.GetRepository<Appointment>()
           .GetPagingListAsync(
               include: query => query.Include(s => s.Customer),
               page: page,
               size: size
           );

            var scheduleResponses = new Paginate<GetAppointmentResponse>()
            {
                Page = schedules.Page,
                Size = schedules.Size,
                Total = schedules.Total,
                TotalPages = schedules.TotalPages,
                Items = _mapper.Map<IList<GetAppointmentResponse>>(schedules.Items),
            };
            return scheduleResponses;
        }

        public async Task<GetAppointmentResponse>? GetAppointmentById(Guid id)
        {
            Appointment appointmentResponse = await _unitOfWork
                .GetRepository<Appointment>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(id),
                    include: source => source.Include(a => a.Customer)
                 );
            if (appointmentResponse == null)
                return null;
            return _mapper.Map<GetAppointmentResponse>(appointmentResponse);
        }

        public async Task<CreateAppointmentResponse> CreateAppointment(CreateAppointmentRequest createAccountRequest)
        {
            var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(createAccountRequest.CustomerId));
            if (customer == null)
            {
                throw new Exception("AccountId not found");
            }
            var appointment = _mapper.Map<Appointment>(createAccountRequest);
            await _unitOfWork.GetRepository<Appointment>().InsertAsync(appointment);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CreateAppointmentResponse>(appointment);
        }

        public async Task<bool> UpdateAppointmentById(Guid id, UpdateAppointmentRequest updateAppointmentRequest)
        {
            var appoinment = await _unitOfWork.GetRepository<Appointment>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (appoinment == null)
            {
                throw new NotFoundException("Appoint not found!");
            }
            appoinment = _mapper.Map<Appointment>(updateAppointmentRequest);
            _unitOfWork.GetRepository<Appointment>().UpdateAsync(appoinment);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<bool> DeleteAppoinmentById(Guid id)
        {
            var appoinment = await _unitOfWork.GetRepository<Appointment>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (appoinment == null)
            {
                throw new NotFoundException("Appoint not found!");
            }
            appoinment.IsActive = false;
            _unitOfWork.GetRepository<Appointment>().UpdateAsync(appoinment);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<bool> ActiveAppointment(Guid id)
        {
            var appoinment = await _unitOfWork.GetRepository<Appointment>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (appoinment == null)
            {
                throw new NotFoundException("Appoint not found!");
            }
            appoinment.IsActive = true;
            _unitOfWork.GetRepository<Appointment>().UpdateAsync(appoinment);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }
    }
}
