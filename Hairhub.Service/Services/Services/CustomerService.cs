using AutoMapper;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using Hairhub.Common.Security;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Enums;

namespace Hairhub.Service.Services.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IPaginate<GetCustomerResponse>> GetCustomers(int page, int size)
        {
            var customerEntities = await _unitOfWork.GetRepository<Customer>()
           .GetPagingListAsync(
               include: query => query.Include(s => s.Account),
               page: page,
               size: size
           );

            var paginateResponse = new Paginate<GetCustomerResponse>
            {
                Page = customerEntities.Page,
                Size = customerEntities.Size,
                Total = customerEntities.Total,
                TotalPages = customerEntities.TotalPages,
                Items = _mapper.Map<IList<GetCustomerResponse>>(customerEntities.Items)
            };

            return paginateResponse;
        }

        public async Task<GetCustomerResponse>? GetCustomerById(Guid id)
        {
            var customerEntity = await _unitOfWork
                .GetRepository<Customer>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(id)
                 );

            return _mapper.Map<GetCustomerResponse>(customerEntity);
        }

        public async Task<bool> CheckInByCustomer(string dataAES, Guid customerId)
        {
            Guid appointmentId;
            try
            {
                string decyptEAS = AesEncoding.DecryptAES(dataAES);
                appointmentId = Guid.Parse(decyptEAS);
            }catch(Exception ex)
            {
                throw new NotFoundException("Checkin thất bại. Vui lòng checkin lại hoặc liên hệ với admin");
            }
            var appointment = await _unitOfWork.GetRepository<Appointment>().SingleOrDefaultAsync
                                                                                                (
                                                                                                    predicate: x => x.Id==appointmentId,
                                                                                                    include: x => x.Include(s=>s.AppointmentDetails)
                                                                                                );
            if(appointment == null)
            {
                throw new NotFoundException("Không tìm thấy đơn đặt lịch");
            }
            if (appointment.CustomerId != customerId)
            {
                throw new NotFoundException("Người đăng nhập không hợp lệ. Vui lòng checkin bằng tài khoản của người đặt lịch này");
            }
            foreach (var appointmentDetail in appointment.AppointmentDetails)
            {
                appointmentDetail.Status = AppointmentStatus.Successed;
                _unitOfWork.GetRepository<AppointmentDetail>().UpdateAsync(appointmentDetail);
            }
            appointment.Status = AppointmentStatus.Successed;
            _unitOfWork.GetRepository<Appointment>().UpdateAsync(appointment);
            bool isInsert = await _unitOfWork.CommitAsync()>0;
            return isInsert;
        }
    }
}
