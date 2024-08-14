using AutoMapper;
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Domain.Dtos.Requests.Appointments;
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
        private readonly IEmailService _emailService;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaService = mediaService;
            _emailService = emailService;
        }

        public async Task<IPaginate<GetReportResponse>> GetAllReport(int page, int size)
        {
            var reports = await _unitOfWork.GetRepository<Report>()
                .GetPagingListAsync(
                    include: x => x.Include(s => s.StaticFiles)
                                   .Include(s => s.SalonInformation)
                                       .ThenInclude(si => si.Schedules) // Include schedules from SalonInformation
                                   .Include(s => s.SalonInformation)
                                       .ThenInclude(si => si.SalonOwner) // Include SalonOwner from SalonInformation
                                   .Include(s => s.Customer)
                                   .Include(s => s.Appointment)
                                       .ThenInclude(a => a.AppointmentDetails)
                                           .ThenInclude(ad => ad.SalonEmployee),
                    page: page,
                    size: size
                );
            foreach (var report in reports.Items)
            {
                report.StaticFiles = await _unitOfWork.GetRepository<StaticFile>().GetListAsync(predicate: x => x.ReportId == report.Id);
            }
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
                                               include: x => x.Include(s => s.StaticFiles)
                                                               .Include(s => s.SalonInformation)
                                                                   .ThenInclude(si => si.Schedules) // Include schedules from SalonInformation
                                                               .Include(s => s.SalonInformation)
                                                                   .ThenInclude(si => si.SalonOwner) // Include SalonOwner from SalonInformation
                                                               .Include(s => s.Customer)
                                                               .Include(s => s.Appointment)
                                                                   .ThenInclude(a => a.AppointmentDetails)
                                                                       .ThenInclude(ad => ad.SalonEmployee),
                                               page: page,
                                               size: size
                                           );
            foreach (var report in reports.Items)
            {
                report.StaticFiles = await _unitOfWork.GetRepository<StaticFile>().GetListAsync(predicate: x => x.ReportId == report.Id);
            }
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

        public async Task<IPaginate<GetReportResponse>> GetReportByCustomerId(Guid customerId, string? Status, int page, int size)
        {
            IPaginate<Report> reports;
            if (String.IsNullOrWhiteSpace(Status))
            {
                reports = await _unitOfWork.GetRepository<Report>()
                               .GetPagingListAsync
                               (
                                  predicate: x => x.CustomerId == customerId,
                                  include: x => x.Include(s => s.StaticFiles)
                                                .Include(s => s.SalonInformation)
                                                    .ThenInclude(si => si.Schedules) // Include schedules from SalonInformation
                                                .Include(s => s.SalonInformation)
                                                    .ThenInclude(si => si.SalonOwner) // Include SalonOwner from SalonInformation
                                                .Include(s => s.Customer)
                                                .Include(s => s.Appointment)
                                                    .ThenInclude(a => a.AppointmentDetails)
                                                        .ThenInclude(ad => ad.SalonEmployee),
                                   page: page,
                                   size: size
                               );
            }
            else
            {
                reports = await _unitOfWork.GetRepository<Report>()
                               .GetPagingListAsync
                               (
                                  predicate: x => x.CustomerId == customerId && x.Status.Equals(Status),
                                  include: x => x.Include(s => s.StaticFiles)
                                                .Include(s => s.SalonInformation)
                                                    .ThenInclude(si => si.Schedules) // Include schedules from SalonInformation
                                                .Include(s => s.SalonInformation)
                                                    .ThenInclude(si => si.SalonOwner) // Include SalonOwner from SalonInformation
                                                .Include(s => s.Customer)
                                                .Include(s => s.Appointment)
                                                    .ThenInclude(a => a.AppointmentDetails)
                                                        .ThenInclude(ad => ad.SalonEmployee),
                                   page: page,
                                   size: size
                               );
            }

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

        public async Task<IPaginate<GetReportResponse>> GetReportBySalonId(Guid salonId, string? status, int page, int size)
        {
            IPaginate<Report> reports;
            if (String.IsNullOrWhiteSpace(status))
            {
                reports = await _unitOfWork.GetRepository<Report>()
                                           .GetPagingListAsync
                                           (
                                               predicate: x => x.SalonId == salonId,
                                               include: x => x.Include(s => s.StaticFiles)
                                                               .Include(s => s.SalonInformation)
                                                                   .ThenInclude(si => si.Schedules) // Include schedules from SalonInformation
                                                               .Include(s => s.SalonInformation)
                                                                   .ThenInclude(si => si.SalonOwner) // Include SalonOwner from SalonInformation
                                                               .Include(s => s.Customer)
                                                               .Include(s => s.Appointment)
                                                                   .ThenInclude(a => a.AppointmentDetails)
                                                                       .ThenInclude(ad => ad.SalonEmployee),
                                               page: page,
                                               size: size
                                           );
            }
            else
            {
                reports = await _unitOfWork.GetRepository<Report>()
                                           .GetPagingListAsync
                                           (
                                               predicate: x => x.SalonId == salonId && x.Status.Equals(status),
                                               include: x => x.Include(s => s.StaticFiles)
                                                               .Include(s => s.SalonInformation)
                                                                   .ThenInclude(si => si.Schedules) // Include schedules from SalonInformation
                                                               .Include(s => s.SalonInformation)
                                                                   .ThenInclude(si => si.SalonOwner) // Include SalonOwner from SalonInformation
                                                               .Include(s => s.Customer)
                                                               .Include(s => s.Appointment)
                                                                   .ThenInclude(a => a.AppointmentDetails)
                                                                       .ThenInclude(ad => ad.SalonEmployee),
                                               page: page,
                                               size: size
                                           );
            }

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
                var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.Id == report.CustomerId && x.Account.IsActive == true);
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
            else if (report.RoleNameReport.Equals(RoleEnum.SalonOwner.ToString()))
            {
                var salon = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync
                                                                         (
                                                                            predicate: x => x.Id == report.SalonId && x.Status.Equals(SalonStatus.Approved)
                                                                            && x.SalonOwner.Account.IsActive == true
                                                                         );
                if (salon == null)
                {
                    throw new NotFoundException("Tài khoản không còn hoạt động");
                }
                appointment = await _unitOfWork.GetRepository<Appointment>()
                                  .SingleOrDefaultAsync
                                  (
                                   predicate: x => x.Id == request.AppointmentId
                                               && ((x.Status.Equals(AppointmentStatus.Booking) && x.AppointmentDetails.OrderByDescending(s=>s.StartTime).FirstOrDefault().StartTime < DateTime.Now)
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
                for (int i = 0; i < request.ImgeReportRequest.Count; i++)
                {
                    var item = request.ImgeReportRequest[i];
                    StaticFile staticFile = new StaticFile();
                    staticFile.Id = Guid.NewGuid();
                    staticFile.ReportId = report.Id;
                    if (item != null)
                    {
                        try
                        {
                            staticFile.Img = await _mediaService.UploadAnImage(item, MediaPath.REPORT, "Img" + staticFile.Id.ToString() + i.ToString());
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

        public async Task<bool> ConfirmReport(Guid id, ConfirmReportRequest request)
        {
            if (!request.Status.Equals(ReportStatus.Approved) && !request.Status.Equals(ReportStatus.Rejected))
            {
                throw new NotFoundException("Sai trạng thái duyệt");
            }
            var report = await _unitOfWork.GetRepository<Report>()
                                          .SingleOrDefaultAsync
                                          (
                                            predicate: x => x.Id == id
                                          );
            if (report == null)
            {
                throw new NotFoundException("Không tìm thấy đơn báo cáo");
            }
            if (report.RoleNameReport.Equals(RoleEnum.Customer.ToString()) || request.Status.Equals(ReportStatus.Approved))
            {
                var salon = await _unitOfWork.GetRepository<SalonInformation>()
                                        .SingleOrDefaultAsync
                                        (
                                           predicate: x => x.Id == report.SalonId,
                                           include: x => x.Include(s=>s.SalonOwner)
                                        );
                if (salon != null)
                {
                    salon.NumberOfReported++;
                    //Xử lý khi salon bị báo cáo, reported+=1
                    switch (salon.NumberOfReported)
                    {
                        case 1:
                            await SendMailNotificatinRemind(salon, (int)salon.NumberOfReported);
                            break;
                        case 2:
                            await SendMailNotificatinRemind(salon, (int)salon.NumberOfReported);
                            break;
                        case 3:
                            await SendMailNotificatinRemind(salon, (int)salon.NumberOfReported);
                            break;
                        case 4:
                            salon = await SuspendedSalon(salon, (int)salon.NumberOfReported);
                            break;
                        case 5:
                            salon = await RemoveFromSystem(salon, (int)salon.NumberOfReported);
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
        private async Task<SalonInformation> RemoveFromSystem(SalonInformation salon, int numberOfReported)
        {
            //Gửi mail hoặc sdt 
            string subject = $"Thông báo từ hệ thống Hairhub: Salon {salon.Name} đã bị xóa khỏi hệ thống do vượt quá số lần vi phạm";
            string bodyEmail = $"Chúng tôi xin thông báo rằng salon {salon.Name} của bạn đã đạt đến số lần báo cáo vi phạm tối đa cho phép là {numberOfReported} lần. " +
                                $"Sau khi xem xét kỹ lưỡng và theo chính sách của chúng tôi, {salon.Name} sẽ bị xóa khỏi hệ thống.\r\n" +
                                $"Chúng tôi hiểu rằng điều này có thể gây khó khăn cho bạn và chúng tôi rất tiếc phải thực hiện biện pháp này. " +
                                $"Tuy nhiên, để đảm bảo chất lượng dịch vụ và sự hài lòng của khách hàng, chúng tôi buộc phải tuân thủ các quy định đã đề ra.\r\n" +
                                $"Nếu bạn có bất kỳ thắc mắc hoặc muốn khiếu nại về quyết định này, xin vui lòng liên hệ với chúng tôi qua email này hoặc số điện thoại bên dưới trong vòng 7 ngày kể từ ngày nhận được thông báo.\r\n" +
                                $"Chúng tôi rất mong nhận được sự thông cảm và hợp tác từ phía bạn.";
            string userEmail = salon.SalonOwner.Email;
            await _emailService.SendEmailWithBodyAsync(userEmail, subject, salon.SalonOwner.FullName, bodyEmail);
            //Chuyển trạng thái Salon
            salon.Status = SalonStatus.Disable;
            return salon;
        }

        // Đóng salon, nộp tiền để comeback, reported = 4
        private async Task<SalonInformation> SuspendedSalon(SalonInformation salon, int numberOfReported)
        {
            //Gửi mail hoặc sdt
            string subject = "Thông báo từ hệ thống Hairhub: Bạn đã bị báo cáo vi phạm và ngừng hoạt động salon tạm thời trên hệ thống";
            string bodyEmail = $"Chúng tôi xin thông báo rằng salon {salon.Name} của bạn đã bị khóa tạm thời do số lần báo cáo vi phạm đã đạt {numberOfReported} lần. " +
                $"Theo chính sách của chúng tôi, salon {salon.Name} sẽ bị khóa tạm thời để đảm bảo chất lượng dịch vụ và sự hài lòng của khách hàng. \r\n" +
                $"Để mở lại hoạt động của salon, bạn cần liên hệ với quản trị viên và nộp khoản tiền phạt theo quy định. " +
                $"Vui lòng liên hệ với chúng tôi qua email này hoặc số điện thoại bên dưới để biết thêm chi tiết về quy trình nộp phạt và mở khóa salon. " +
                $"\r\nChúng tôi rất mong nhận được sự hợp tác từ phía quý vị để cùng nhau duy trì môi trường dịch vụ chất lượng và an toàn.";
            string userEmail = salon.SalonOwner.Email;
            await _emailService.SendEmailWithBodyAsync(userEmail, subject, salon.SalonOwner.FullName, bodyEmail);
            //Chuyển trạng thái Salon
            salon.Status = SalonStatus.Suspended;
            return salon;
        }

        // Gửi thông báo cảnh cáo khi salon bị report < 4
        private async Task<bool> SendMailNotificatinRemind(SalonInformation salon, int numberOfReported)
        {
            //Gửi mail nhắc nhở
            string subject = "Thông báo từ hệ thống Hairhub: Bạn đã bị báo cáo vi phạm";
            string bodyEmail = $"Chúng tôi nhận được thông báo từ khách hàng về một số vấn đề liên quan đến hoạt động của salon {salon.Name} của bạn lần thứ {numberOfReported} trên nền tảng Hairhub. " +
                $"Đây là một phần trong cam kết của chúng tôi để duy trì chất lượng dịch vụ và đảm bảo trải nghiệm tốt nhất cho người dùng. " +
                $"Vui lòng kiểm tra và xử lý các vấn đề đang xảy ra để đảm bảo rằng hoạt động của salon của bạn đáp ứng các tiêu chuẩn của chúng tôi. " +
                $"Mọi chi tiết liên hệ phản hồi qua mail này hoặc kiểm tra thông tin report trên HairHub để hiểu rõ hơn.";
            await _emailService.SendEmailWithBodyAsync(salon.SalonOwner.Email, subject, salon.SalonOwner.FullName, bodyEmail);
            return true;
        }
    }
}
