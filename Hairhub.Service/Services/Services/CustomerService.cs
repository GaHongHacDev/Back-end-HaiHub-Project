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
using Hairhub.Domain.Dtos.Responses.Dashboard;
using Hairhub.Domain.Dtos.Requests.Customers;
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Domain.Dtos.Responses.Feedbacks;
using CloudinaryDotNet.Actions;
using Hairhub.Domain.Dtos.Responses.Appointments;
using static QRCoder.Base64QRCode;

namespace Hairhub.Service.Services.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaService = mediaService;
        }
        public async Task<IPaginate<GetCustomerResponse>> GetCustomers(string? email, bool? status, int page, int size)
        {

            var customerEntities = await _unitOfWork.GetRepository<Customer>()
        .GetPagingListAsync(
            predicate: c =>
                (string.IsNullOrEmpty(email) || c.Email.Contains(email)) &&
                (!status.HasValue || c.Account.IsActive == status.Value),  // Nullable bool handling
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
            } catch (Exception ex)
            {
                throw new NotFoundException("Checkin thất bại. Vui lòng checkin lại hoặc liên hệ với admin");
            }
            var appointment = await _unitOfWork.GetRepository<Appointment>().SingleOrDefaultAsync
                                                                                                (
                                                                                                    predicate: x => x.Id == appointmentId,
                                                                                                    include: x => x.Include(s => s.AppointmentDetails)
                                                                                                );
            if (appointment == null)
            {
                throw new NotFoundException
                     ("Không tìm thấy đơn đặt lịch");
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
            bool isInsert = await _unitOfWork.CommitAsync() > 0;
            return isInsert;
        }

        public async Task<bool> SaveAsCustomerImageHistory(CustomerImageHistoryRequest request)
        {
            var customer = await _unitOfWork.GetRepository<Customer>()
                .SingleOrDefaultAsync(predicate: p => p.Id == request.CustomerId);

            if (customer == null)
            {
                throw new NotFoundException("Khách hàng không tồn tại");
            }

            var newImageCustomer = new StyleHairCustomer
            {
                Id = Guid.NewGuid(),
                CustomerId = (Guid)request.CustomerId,
                Title = request.Title,
                Description = request.Description,
                IsActive = true,
                CreatedDate = DateTime.Now,
                UpdateddAt = DateTime.Now,
            };

            await _unitOfWork.GetRepository<StyleHairCustomer>().InsertAsync(newImageCustomer);

            for (int i = 0; i < request.ImageStyles.Count; i++)
            {
                var urlImg = await _mediaService.UploadAnImage(
                    request.ImageStyles[i],
                    MediaPath.FEEDBACK_IMG,
                    newImageCustomer.Id.ToString() + "/" + i.ToString());

                var imageStyle = new ImageStyle
                {
                    Id = Guid.NewGuid(),
                    StyleHairCustomerId = newImageCustomer.Id, // Corrected to use the newImageCustomer's Id
                    IsActive = true,
                    UrlImage = urlImg,
                };

                await _unitOfWork.GetRepository<ImageStyle>().InsertAsync(imageStyle);
            }

            bool isCreated = await _unitOfWork.CommitAsync() > 0;
            return isCreated;           
        }
            
        public async Task<IPaginate<CustomerImageHistoryResponse>> GetCustomerImagesHistory(Guid customerId, int page, int size)
        {
            var customer = await _unitOfWork.GetRepository<Customer>()
                .SingleOrDefaultAsync(predicate: p => p.Id == customerId);

            if (customer == null)
            {
                throw new NotFoundException("Khách hàng không tồn tại");
            }

            var cusImages = await _unitOfWork.GetRepository<StyleHairCustomer>()
                         .GetPagingListAsync(predicate: p => p.CustomerId == customerId,
                         include: i => i.Include(m => m.ImageStyles),
                         page: page, size: size,
                         orderBy: o => o.OrderByDescending(l => l.CreatedDate));

            var customerImageHistoryResponses = new Paginate<CustomerImageHistoryResponse>()
            {
                Page = cusImages.Page,
                Size = cusImages.Size,
                Total = cusImages.Total,
                TotalPages = cusImages.TotalPages,
                Items = _mapper.Map<IList<CustomerImageHistoryResponse>>(cusImages.Items),
            };
            return customerImageHistoryResponses;
        }

        public async Task<bool> DeleteCustomerImageHistory(Guid Id)
        {
            var image = await _unitOfWork.GetRepository<StyleHairCustomer>().SingleOrDefaultAsync(predicate: (p) => p.Id == Id);
            if (image == null)
            {
                throw new NotFoundException("Lịch sử không tồn tại");
            }
            
            var imgUrl = await _unitOfWork.GetRepository<ImageStyle>().GetListAsync(predicate: p => p.StyleHairCustomerId == Id);

            _unitOfWork.GetRepository<StyleHairCustomer>().DeleteAsync(image);
            _unitOfWork.GetRepository<ImageStyle>().DeleteRangeAsync(imgUrl);
            bool isDeleted = await _unitOfWork.CommitAsync() > 0;
            return isDeleted;
        }

        public async Task<bool> UpdateCustomerImagesHistory(Guid Id, UpdateCustomerImageHistoryRequest request)
            {
            var image = await _unitOfWork.GetRepository<StyleHairCustomer>()
       .SingleOrDefaultAsync(predicate: p => p.Id == Id);

            if (image == null)
            {
                throw new NotFoundException("Lịch sử không tồn tại");
            }

            
            image.Id = Id;
            if (!string.IsNullOrEmpty(request.Title))
            {
                image.Title = request.Title;
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                image.Description = request.Description;
            }
            image.UpdateddAt = DateTime.Now;

            
            if (request.RemoveImageStyleIds != null && request.RemoveImageStyleIds.Count > 0)
            {
                var stylesToRemove = await _unitOfWork.GetRepository<ImageStyle>()
                    .GetListAsync(predicate: p => request.RemoveImageStyleIds.Contains(p.Id));

                if (stylesToRemove.Any())
                {
                    _unitOfWork.GetRepository<ImageStyle>().DeleteRangeAsync(stylesToRemove);
                }
            }
            if (request.ImageStyles != null && request.ImageStyles.Count > 0)
            {
                for (int i = 0; i < request.ImageStyles.Count; i++)
                {
                    var urlImg = await _mediaService.UploadAnImage(
                        request.ImageStyles[i],
                        MediaPath.FEEDBACK_IMG,
                        Id.ToString() + "/" + i.ToString());

                    var imageStyle = new ImageStyle
                    {
                        Id = Guid.NewGuid(),
                        StyleHairCustomerId = Id,
                        IsActive = true,
                        UrlImage = urlImg,
                    };

                    await _unitOfWork.GetRepository<ImageStyle>().InsertAsync(imageStyle);
                }
            }
            _unitOfWork.GetRepository<StyleHairCustomer>().UpdateAsync(image);
            bool isUpdated = await _unitOfWork.CommitAsync() > 0;
            return isUpdated;
        }
    }
}

