using AutoMapper;
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Domain.Dtos.Requests.Reports;
using Hairhub.Domain.Dtos.Responses.Reports;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Hairhub.Service.Services.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaService = mediaService;
        }

        public async Task<IPaginate<GetReportResponse>> GetAllReport(int page, int size)
        {
            var reports = await _unitOfWork.GetRepository<Report>()
                                           .GetPagingListAsync
                                           (
                                               include: x => x.Include(s => s.SalonInformation)
                                                                  .ThenInclude(s => s.SalonOwner)
                                                              .Include(s => s.Customer)
                                                              .Include(s => s.Appointment).ThenInclude(s=>s.AppointmentDetails)
                                                              .Include(s => s.StaticFiles),
                                               page: page,
                                               size: size
                                           );
            var reportResponse = new Paginate<GetReportResponse>()
            {
                Page = reports.Page,
                Size = reports.Size,
                Total = reports.Total,
                TotalPages = reports.TotalPages,
                Items = _mapper.Map<IList<GetReportResponse>>(reports.Items),
            };
            return reportResponse;
        }

        public async Task<IPaginate<GetReportResponse>> GetAllReportByRoleName(string roleNameReport, int page, int size)
        {
            var reports = await _unitOfWork.GetRepository<Report>()
                                           .GetPagingListAsync
                                           (
                                               predicate: x => x.RoleNameReport.Equals(roleNameReport),
                                               include: x => x.Include(s => s.SalonInformation)
                                                                  .ThenInclude(s => s.SalonOwner)
                                                              .Include(s => s.Customer)
                                                              .Include(s => s.Appointment).ThenInclude(s => s.AppointmentDetails)
                                                              .Include(s => s.StaticFiles),
                                               page: page,
                                               size: size
                                           );
            var reportResponse = new Paginate<GetReportResponse>()
            {
                Page = reports.Page,
                Size = reports.Size,
                Total = reports.Total,
                TotalPages = reports.TotalPages,
                Items = _mapper.Map<IList<GetReportResponse>>(reports.Items),
            };
            return reportResponse;
        }

        public async Task<IPaginate<GetReportResponse>> GetReportByCustomerId(Guid customerId, int page, int size)
        {
            var reports = await _unitOfWork.GetRepository<Report>()
                                           .GetPagingListAsync
                                           (
                                               predicate: x=>x.CustomerId == customerId,
                                               include: x => x.Include(s => s.SalonInformation)
                                                                .ThenInclude(s => s.SalonOwner)
                                                              .Include(s => s.Customer)
                                                              .Include(s => s.Appointment).ThenInclude(s => s.AppointmentDetails)
                                                              .Include(s => s.StaticFiles),
                                               page: page,
                                               size: size
                                           );
            var reportResponse = new Paginate<GetReportResponse>()
            {
                Page = reports.Page,
                Size = reports.Size,
                Total = reports.Total,
                TotalPages = reports.TotalPages,
                Items = _mapper.Map<IList<GetReportResponse>>(reports.Items),
            };
            return reportResponse;
        }

        public async Task<IPaginate<GetReportResponse>> GetReportBySalonId(Guid salonId, int page, int size)
        {
            var reports = await _unitOfWork.GetRepository<Report>()
                                           .GetPagingListAsync
                                           (
                                             include: x => x.Include(s => s.SalonInformation)
                                                                .ThenInclude(s => s.SalonOwner)
                                                            .Include(s => s.Customer)
                                                            .Include(s => s.Appointment).ThenInclude(s => s.AppointmentDetails)
                                                            .Include(s => s.StaticFiles),
                                               predicate: x => x.SalonId == salonId,
                                               page: page,
                                               size: size
                                           );
            var reportResponse = new Paginate<GetReportResponse>()
            {
                Page = reports.Page,
                Size = reports.Size,
                Total = reports.Total,
                TotalPages = reports.TotalPages,
                Items = _mapper.Map<IList<GetReportResponse>>(reports.Items),
            };
            return reportResponse;
        }
        public async Task<bool> CreateReport(CreateReportRequest request)
        {
            var report = _mapper.Map<Report>(request);
            report.Id = Guid.NewGuid();
            report.Status = ReportStatus.Pending;
            report.CreateDate = DateTime.Now;
            report.ReasonReport = request.ReasonReport;
            Appointment appointment = null;
            if (report.RoleNameReport.Equals(RoleEnum.Customer.ToString()))
            {
                var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x=>x.Id == report.CustomerId && x.Account.IsActive==true);
                if (customer == null)
                {
                    throw new NotFoundException("Không thể tạo báo cáo vì tài khoản của bạn không hoạt động");
                }
                 appointment = await _unitOfWork.GetRepository<Appointment>()
                                   .SingleOrDefaultAsync
                                   (
                                    predicate: x => x.Id == request.AppointmentId
                                                && (
                                                x.Status.Equals(AppointmentStatus.Successed)
                                                || x.Status.Equals(AppointmentStatus.Booking) || x.Status.Equals(AppointmentStatus.CancelByCustomer)
                                                )
                                   );
                if (appointment == null)
                {
                    throw new NotFoundException("Bạn không thể báo cáo salon, barber shop khi chưa đặt lịch");
                }
                appointment.IsReportByCustomer = true;
            }
            else if(report.RoleNameReport.Equals(RoleEnum.SalonOwner.ToString()))
            {
                var salon = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync
                                                                         (
                                                                            predicate: x => x.Id == report.SalonId && x.Status.Equals(SalonStatus.Approved) 
                                                                            && x.SalonOwner.Account.IsActive==true
                                                                         );
                if (salon == null)
                {
                    throw new NotFoundException("Tài khoản không còn hoạt động");
                }
                appointment = await _unitOfWork.GetRepository<Appointment>()
                                  .SingleOrDefaultAsync
                                  (
                                   predicate: x => x.Id == request.AppointmentId
                                               && ((x.Status.Equals(AppointmentStatus.Booking) && x.StartDate<DateTime.Now) 
                                               || x.Status.Equals(AppointmentStatus.Successed)
                                               || x.Status.Equals(AppointmentStatus.CancelByCustomer))
                                  );
                if (appointment == null)
                {
                    throw new NotFoundException("Bạn không được báo cáo người dùng khi chưa đến thời gian đặt lịch");
                }
                appointment.IsReportBySalon = true;
            }
            else
            {
                throw new Exception("Thông tin người dùng báo cáo không chính xác");
            }
            _unitOfWork.GetRepository<Appointment>().UpdateAsync(appointment);
            if (request.ImgeReportRequest != null && request.ImgeReportRequest.Count > 0)
            {
                for (int i=0; i<request.ImgeReportRequest.Count; i++)
                {
                    var item = request.ImgeReportRequest[i];
                    StaticFile staticFile = new StaticFile();
                    staticFile.Id = Guid.NewGuid();
                    staticFile.ReportId = report.Id;
                    if (item != null)
                    {
                        try
                        {
                            staticFile.Img = await _mediaService.UploadAnImage(item, MediaPath.REPORT, "Img"+staticFile.Id.ToString() + i.ToString());
                        }
                        catch (Exception ex) 
                        {
                            throw new Exception("Lỗi tải ảnh lên");
                        }
                    }
                    //if (item.VideoReport != null)
                    //{
                    //    try
                    //    {
                    //        staticFile.Video = await _mediaService.UploadAVideo(item.VideoReport, MediaPath.REPORT, "Video" + staticFile.Id.ToString() + i.ToString());
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        throw new Exception("Lỗi tải video lên");
                    //    }
                    //}
                    await _unitOfWork.GetRepository<StaticFile>().InsertAsync(staticFile);
                }
            }

            await _unitOfWork.GetRepository<Report>().InsertAsync(report);
            bool isInsert = await _unitOfWork.CommitAsync() > 0;
            return isInsert;
        }

        public async Task<bool> ConfirmReport(Guid id,  ConfirmReportRequest request)
        {
            if (!request.Status.Equals(ReportStatus.Approved)&& !request.Status.Equals(ReportStatus.Rejected))
            {
                throw new NotFoundException("Sai trạng thái duyệt");
            }
            var report = await _unitOfWork.GetRepository<Report>()
                                          .SingleOrDefaultAsync
                                          (
                                            predicate: x=>x.Id == id  
                                          );
            if(report == null)
            {
                throw new NotFoundException("Không tìm thấy đơn báo cáo");
            }
            if (report.RoleNameReport.Equals(RoleEnum.Customer.ToString()) || request.Status.Equals(ReportStatus.Approved))
            {
                var salon = await _unitOfWork.GetRepository<SalonInformation>()
                                        .SingleOrDefaultAsync
                                        (
                                           predicate: x=>x.Id == report.SalonId
                                        );
                if (salon != null)
                {
                    salon.NumberOfReported++;
                    //Xử lý khi salon bị báo cáo, reported+=1
                    switch (salon.NumberOfReported)
                    {
                        case 1:
                            SendMailNotificatinRemind(salon);
                            break;
                        case 2:
                            SendMailNotificatinRemind(salon);
                            break;
                        case 3:
                            SendMailNotificatinRemind(salon);
                            break;
                        case 4:
                            salon = SuspendedSalon(salon);
                            break;
                        case 5:
                            salon = RemoveFromSystem(salon);
                            break;

                    }
                    _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salon);
                }
            }
               
            else if (report.RoleNameReport.Equals(RoleEnum.SalonOwner.ToString()) || request.Status.Equals(ReportStatus.Approved))
            {
                var customer = await _unitOfWork.GetRepository<Customer>()
                                        .SingleOrDefaultAsync
                                        (
                                           predicate: x => x.Id == report.SalonId && x.Account.IsActive
                                        );
                if (customer != null)
                {
                    customer.NumberOfReported++;
                    _unitOfWork.GetRepository<Customer>().UpdateAsync(customer);
                }
            }
            report.TimeConfirm = DateTime.Now;
            report.Status = request.Status;
            report.DescriptionAdmin = request.DescriptionAdmin;
            _unitOfWork.GetRepository<Report>().UpdateAsync(report);
            bool isConfirm = await _unitOfWork.CommitAsync() > 0;
            return isConfirm;
        }

        // Xóa salon khởi hệ thống, reported = 5
        private SalonInformation RemoveFromSystem(SalonInformation salon)
        {
            //Gửi mail hoặc sdt 
            //Chuyển trạng thái Salon
            salon.Status = SalonStatus.Disable;
            return salon;
        }

        // Đóng salon, nộp tiền để comeback, reported = 4
        private SalonInformation SuspendedSalon(SalonInformation salon)
        {
            //Gửi mail hoặc sdt
            //Chuyển trạng thái Salon
            salon.Status = SalonStatus.Suspended;
            return salon;
        }

        // Gửi thông báo cảnh cáo khi salon bị report < 4
        private bool SendMailNotificatinRemind(SalonInformation salon)
        {
            //Gửi mail nhắc nhở
            string userEmail = salon.SalonOwner.Email;
            return true;
        }
    }
}
