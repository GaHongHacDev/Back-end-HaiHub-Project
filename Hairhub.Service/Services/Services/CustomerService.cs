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
using System.Xml.Linq;
using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;

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
        public async Task<IPaginate<GetCustomerResponse>> GetCustomers(string? email, bool? status, string? customerName, bool? isAscendingBooking, int page, int size)
        {
            ICollection<Customer> customerEntities;
            if (email.IsNullOrEmpty())
            {
                email = "";
            }
            if (customerName.IsNullOrEmpty())
            {
                customerName = "";
            }
            if (status==null)
            {
                customerEntities = await _unitOfWork.GetRepository<Customer>()
                .GetListAsync(
                    predicate: c => c.Email!.Contains(email!) && c.FullName.Contains(customerName!),
                    include: query => query.Include(s => s.Account)
                );
            }
            else
            {
                customerEntities = await _unitOfWork.GetRepository<Customer>()
                .GetListAsync(
                    predicate: c => c.Email!.Contains(email!) && c.FullName.Contains(customerName!) && c.Account.IsActive == status.Value,
                    include: query => query.Include(s => s.Account)
                );
            }

            
            var result = _mapper.Map<IList<GetCustomerResponse>>(customerEntities);
            foreach (var item in result) {
                item.NumberOfAppointment = (await _unitOfWork.GetRepository<Appointment>().GetListAsync(predicate: x => x.CustomerId == item.Id && x.Status.Equals(AppointmentStatus.Successed))).Count;
                if (item.NumberOfAppointment > 0)
                {
                    Console.WriteLine(item.Id);
                }
            }

            if (isAscendingBooking != null)
            {
                result = (isAscendingBooking == true) ? result.OrderBy(x => x.NumberOfAppointment).ToList() : result.OrderByDescending(x => x.NumberOfAppointment).ToList();
            }

            var pagedResult = result.Skip((page - 1) * size).Take(size).ToList();

            var paginateResponse = new Paginate<GetCustomerResponse>
            {
                Page = page,
                Size = size,
                Total = result.Count,
                TotalPages = (int)Math.Ceiling((double)result.Count / size),
                Items = pagedResult
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
            }
            catch (Exception ex)
            {
                throw new NotFoundException("Checkin thất bại. Vui lòng checkin lại hoặc liên hệ với admin");
            }
            var appointment = await _unitOfWork.GetRepository<Appointment>().SingleOrDefaultAsync
                                                                                                (
                                                                                                    predicate: x => x.Id == appointmentId,
                                                                                                    include: x => x.Include(s => s.AppointmentDetails).Include(s => s.AppointmentDetailVouchers)
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


            if (appointment.PaymentMethod.Equals(AppointmentPaymentMethod.PayByWallet) || appointment.PaymentMethod.Equals(AppointmentPaymentMethod.PayByBank))
            {
                var employeeId = appointment.AppointmentDetails.ElementAt(0).SalonEmployeeId;
                var employee = await _unitOfWork.GetRepository<SalonEmployee>()
                                                .SingleOrDefaultAsync(
                                                    predicate: x => x.Id == employeeId,
                                                    include: x => x.Include(s => s.SalonInformation.SalonOwner)
                                                );
                var accountSalon = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == employee.SalonInformation.SalonOwner.AccountId);

                decimal payMoney = appointment.TotalPrice;
                if (appointment.AppointmentDetailVouchers != null && appointment.AppointmentDetailVouchers!.Count != 0)
                {
                    foreach (var item in appointment.AppointmentDetailVouchers)
                    {
                        var voucher = await _unitOfWork.GetRepository<Voucher>().SingleOrDefaultAsync(predicate: x => x.Id == item.VoucherId);
                        if (voucher.IsSystemCreated)
                        {
                            payMoney = appointment.OriginalPrice;
                            break;
                        }
                    }
                }

                accountSalon.Balance += payMoney;
                _unitOfWork.GetRepository<Account>().UpdateAsync(accountSalon);

                var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.Id == customerId);
                string fullName = customer != null ? customer.FullName : "";
                //Tao payment
                Payment payment = new Payment()
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountSalon.Id,
                    AppointmentId = appointment.Id,
                    Description = $"Nhận tiền từ cuộc hẹn với khách hàng {customer!.FullName}",
                    PaymentDate = DateTime.Now,
                    TotalAmount = payMoney,
                    PaymentType = PaymentType.Deposit,
                    Status = PaymentStatus.Paid,
                };
                await _unitOfWork.GetRepository<Payment>().InsertAsync(payment);
            }

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
                    MediaPath.STYLE_HAIR_CUSTOMER,
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

        public async Task<GetCustomerByEmailReponse> GetCustomerByEmail(string? email)
        {
            email = email == null ? "" : email.Trim();
            var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.Email.Equals(email));
            if (customer == null)
            {
                return null;
            }
            return _mapper.Map<GetCustomerByEmailReponse>(customer);
        }
    }
}

