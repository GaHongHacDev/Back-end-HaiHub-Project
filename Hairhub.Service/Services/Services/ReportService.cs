using AutoMapper;
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
        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IPaginate<GetReportResponse>> GetAllReport(int page, int size)
        {
            var reports = await _unitOfWork.GetRepository<Report>()
                                           .GetPagingListAsync
                                           (
                                               include: x => x.Include(s => s.SalonInformation)
                                                                  .ThenInclude(s => s.SalonOwner)
                                                              .Include(s => s.Customer)
                                                              .Include(s => s.Appointment),
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
                                                              .Include(s => s.Appointment),
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
                                                            .Include(s => s.Appointment),
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
            report.Id = new Guid();
            report.Status = ReportStatus.Pending;
            report.CreateDate = DateTime.Now;
            Appointment appointment = null;
            if (request.CustomerId != null)
            {
                 appointment = await _unitOfWork.GetRepository<Appointment>()
                                   .SingleOrDefaultAsync
                                   (
                                    predicate: x => x.Id == request.AppointmentId
                                                && (
                                                x.Status.Equals(AppointmentStatus.Successed)
                                                || x.Status.Equals(AppointmentStatus.Booking)
                                                )
                                   );
                if (appointment == null)
                {
                    throw new NotFoundException("Bạn không được báo cáo salon, barber shop khi chưa đặt hoặc chưa check in");
                }
            }
            else if(request.SalonId != null)
            {
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
            }
            else
            {
                throw new Exception("Không tìm thông tin của bạn để báo cáo");
            }

            if (appointment == null)
            {
                throw new NotFoundException("Không tìm thấy đơn đặt lịch");
            }
            if (request.CustomerId!=null)
            {
                appointment.IsReportByCustomer = true;
            }
            else
            {
                appointment.IsReportBySalon = true;
            }
            await _unitOfWork.GetRepository<Report>().InsertAsync(report);
            bool isInsert = await _unitOfWork.CommitAsync() > 0;
            return isInsert;
        }

        public async Task<bool> ConfirmReport(Guid id,  ConfirmReportRequest request)
        {
            if (!request.Status.Equals(ReportStatus.Pending))
            {
                throw new NotFoundException("Không thể duyệt báo cáo với trạng thái là đồng ý hoặc từ chối");
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
                                           predicate: x=>x.Id == report.SalonId && x.Status.Equals(SalonStatus.Approved)
                                        );
                if (salon == null)
                {
                    throw new NotFoundException("Không tìm thấy salon, barber shop");
                }
                salon.NumberOfReported++;
                _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salon);
            }
            else if (report.RoleNameReport.Equals(RoleEnum.SalonOwner.ToString()) || request.Status.Equals(ReportStatus.Approved))
            {
                var customer = await _unitOfWork.GetRepository<Customer>()
                                        .SingleOrDefaultAsync
                                        (
                                           predicate: x => x.Id == report.SalonId && x.Account.IsActive
                                        );
                if (customer == null)
                {
                    throw new NotFoundException("Không tìm thấy người đặt lịch");
                }
                customer.NumberOfReported++;
                _unitOfWork.GetRepository<Customer>().UpdateAsync(customer);
            }
            report.TimeConfirm = DateTime.Now;
            report.Status = request.Status;
            report.DescriptionAdmin = request.DescriptionAdmin;
            _unitOfWork.GetRepository<Report>().UpdateAsync(report);
            bool isConfirm = await _unitOfWork.CommitAsync() > 0;
            return isConfirm;
        }
    }
}
