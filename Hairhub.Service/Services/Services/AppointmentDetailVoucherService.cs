using AutoMapper;
using Hairhub.Domain.Dtos.Requests.AppointmentDetailVouchers;
using Hairhub.Domain.Dtos.Responses.AppointmentDetailVoucher;
using Hairhub.Domain.Dtos.Responses.Feedbacks;
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
    public class AppointmentDetailVoucherService : IAppointmentDetailVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentDetailVoucherService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            this._unitOfWork = _unitOfWork;
            this._mapper = _mapper;
        }

        public async Task<IPaginate<GetAppointmentDetailVoucherResponse>> GetAppointmentDetailVouchers(int page, int size)
        {
            var appointmentDetailVouchers = await _unitOfWork.GetRepository<AppointmentDetailVoucher>()
                .GetPagingListAsync(
                predicate: x => x.IsActive == true,
                include: query => query.Include(s => s.Voucher).Include(s => s.Appointment),
                page: page,
                size: size);

            var appointmentDetailVoucherResponses = new Paginate<GetAppointmentDetailVoucherResponse>()
            {
                Page = appointmentDetailVouchers.Page,
                Size = appointmentDetailVouchers.Size,
                Total = appointmentDetailVouchers.Total,
                TotalPages = appointmentDetailVouchers.TotalPages,
                Items = _mapper.Map<IList<GetAppointmentDetailVoucherResponse>>(appointmentDetailVouchers.Items),
            };

            return appointmentDetailVoucherResponses;
        }

        public async Task<GetAppointmentDetailVoucherResponse> GetAppointmentDetailVoucherById(Guid id)
        {
            var appointmentDetailVoucher = await _unitOfWork.GetRepository<AppointmentDetailVoucher>()
                .SingleOrDefaultAsync(
                 predicate: predicate => predicate.Id.Equals(id),
                 include: query => query.Include(s => s.Voucher).Include(s => s.Appointment));

            var appointmentDetailVoucherResponse = _mapper.Map<GetAppointmentDetailVoucherResponse>(appointmentDetailVoucher);

            return appointmentDetailVoucherResponse;
        }

        public async Task<bool> CreateAppointmentDetailVoucher(CreateAppointmentDetailVoucherRequest request)
        {
            AppointmentDetailVoucher newAppointmentDetailVoucher = new AppointmentDetailVoucher()
            {
                Id = Guid.NewGuid(),
                VoucherId = (Guid)request.VoucherId,
                AppointmentId = (Guid)request.AppointmentId,
                AppliedAmount = (decimal)request.AppliedAmount,
                AppliedDate = (DateTime)request.AppliedDate,
                
            };
            await _unitOfWork.GetRepository<AppointmentDetailVoucher>().InsertAsync(newAppointmentDetailVoucher);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> UpdateAppointmentDetailVoucher(Guid id, UpdateAppointmentDetailVoucherRequest request)
        {
            var appointmentDetailVoucher = await _unitOfWork.GetRepository<AppointmentDetailVoucher>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(id));

            if (appointmentDetailVoucher == null) throw new Exception("AppointmentDetailVoucher is not exist!!!");

            appointmentDetailVoucher.AppliedAmount = (decimal)request.AppliedAmount;
            appointmentDetailVoucher.AppliedDate = (DateTime)request.AppliedDate;

            _unitOfWork.GetRepository<AppointmentDetailVoucher>().UpdateAsync(appointmentDetailVoucher);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteAppointmentDetailVoucher(Guid id)
        {
            var appointmentDetailVoucher = await _unitOfWork.GetRepository<AppointmentDetailVoucher>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(id));

            if (appointmentDetailVoucher == null) throw new Exception("AppointmentDetailVoucher is not exist!!!");

            appointmentDetailVoucher.IsActive = false;

            _unitOfWork.GetRepository<AppointmentDetailVoucher>().UpdateAsync(appointmentDetailVoucher);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

    }
}
