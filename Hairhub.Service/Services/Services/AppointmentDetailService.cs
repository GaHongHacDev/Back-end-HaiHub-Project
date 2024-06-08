using AutoMapper;
using Hairhub.Domain.Dtos.Requests.AppointmentDetails;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class AppointmentDetailService : IAppointmentDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region GetAllAppointmentDetail
        public async Task<IPaginate<GetAppointmentDetailResponse>> GetAllAppointmentDetail(int page, int size)
        {
            var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
           .GetPagingListAsync(
               include: query => query.Include(s => s.SalonEmployee).Include(s => s.ServiceHair).Include(s => s.Appointment),
               page: page,
               size: size
           );
            var appointmentDetailResponses = new Paginate<GetAppointmentDetailResponse>()
            {
                Page = appointmentDetails.Page,
                Size = appointmentDetails.Size,
                Total = appointmentDetails.Total,
                TotalPages = appointmentDetails.TotalPages,
                Items = (IList<GetAppointmentDetailResponse>)_mapper.Map<IList<GetAppointmentDetailResponse>>(appointmentDetails.Items),
            };
            return appointmentDetailResponses;
        }
        #endregion

        #region GetAppointmentDetailById
        public async Task<GetAppointmentDetailResponse>? GetAppointmentDetailById(Guid id)
        {
            var appointmentDetail = await _unitOfWork
                .GetRepository<AppointmentDetail>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(id),
                    include: query => query.Include(s => s.SalonEmployee).Include(s => s.ServiceHair).Include(s => s.Appointment)
                 );
            if (appointmentDetail == null)
                return null;
            return _mapper.Map<GetAppointmentDetailResponse>(appointmentDetail);
        }
        #endregion

        #region CreateAppointment
        public async Task<CreateAppointmentDetailResponse> CreateAppointmentDetail(CreateAppointmentDetailRequest createAppointmentDetailRequest)
        {
            var salonEmployee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(createAppointmentDetailRequest.SalonEmployeeId));
            if (salonEmployee == null)
            {
                throw new Exception("Salon employee not found!");
            }
            var serviceHair = await _unitOfWork.GetRepository<ServiceHair>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(createAppointmentDetailRequest.ServiceHairId));
            if (serviceHair == null)
            {
                throw new Exception("Service hair not found!");
            }
            var appointment = await _unitOfWork.GetRepository<Appointment>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(createAppointmentDetailRequest.AppointmentId));
            if (appointment == null)
            {
                throw new Exception("Appointment not found!");
            }

            var appointmentDetail = _mapper.Map<AppointmentDetail>(createAppointmentDetailRequest);
            appointmentDetail.Id = Guid.NewGuid();
            await _unitOfWork.GetRepository<AppointmentDetail>().InsertAsync(appointmentDetail);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CreateAppointmentDetailResponse>(appointmentDetail);
        }

        public async Task<CreateAppointmentDetailResponse> CreateAppointmentDetailFromAppointment(Guid appointmentId, AppointmentDetailRequest createAppointmentDetailRequest)
        {
            var salonEmployee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(createAppointmentDetailRequest.SalonEmployeeId));
            if (salonEmployee == null)
            {
                throw new Exception("Salon employee not found!");
            }
            var serviceHair = await _unitOfWork.GetRepository<ServiceHair>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(createAppointmentDetailRequest.ServiceHairId));
            if (serviceHair == null)
            {
                throw new Exception("Service hair not found!");
            }

            var appointmentDetail = _mapper.Map<AppointmentDetail>(createAppointmentDetailRequest);
            appointmentDetail.Id = Guid.NewGuid();
            appointmentDetail.AppointmentId = appointmentId;
            await _unitOfWork.GetRepository<AppointmentDetail>().InsertAsync(appointmentDetail);
            return _mapper.Map<CreateAppointmentDetailResponse>(appointmentDetail);
        }
        #endregion

        #region UpdateAppointmentById
        public async Task<bool> UpdateAppointmentDetailById(Guid id, UpdateAppointmentDetailRequest updateAppointmentDetailRequest)
        {
            var appoinmentDetail = await _unitOfWork.GetRepository<AppointmentDetail>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (appoinmentDetail == null)
            {
                throw new NotFoundException("Appoint detail not found!");
            }
            appoinmentDetail = _mapper.Map<AppointmentDetail>(updateAppointmentDetailRequest);
            _unitOfWork.GetRepository<AppointmentDetail>().UpdateAsync(appoinmentDetail);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }
        #endregion

        #region DeleteAppoinmentById
        public async Task<bool> DeleteAppoinmentDetailById(Guid id)
        {
            var appoinmentDetail = await _unitOfWork.GetRepository<AppointmentDetail>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (appoinmentDetail == null)
            {
                throw new NotFoundException("Appoint not found!");
            }
            appoinmentDetail.Status = false;
            _unitOfWork.GetRepository<AppointmentDetail>().UpdateAsync(appoinmentDetail);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }
        #endregion
    }
}
