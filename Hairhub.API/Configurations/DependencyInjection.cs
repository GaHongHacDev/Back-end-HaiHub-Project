using Hairhub.Common.CommonService.Contract;
using Hairhub.Common.CommonService.Implementation;
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Common.ThirdParties.Implementation;
using Hairhub.Domain.Entitities;
using Hairhub.Infrastructure.Repository;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Infrastructure.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddDIServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IAppointmentDetailService, AppointmentDetailService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IAppointmentDetailVoucherService, AppointmentDetailVoucherService>();
            services.AddScoped<ISalonOwnerService, SalonOwnerService>();
            services.AddScoped<ISalonEmployeeService, SalonEmployeeService>();
            services.AddScoped<ISalonInformationService, SalonInformationService>();
            services.AddScoped<IServiceHairService, ServiceHairService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<IQRCodeService, QRCodeService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IVoucherService, VoucherService>();
            services.AddScoped<IConfigService, ConfigService>();
            services.AddScoped<IApprovalService, ApprovalService>();
            return services;
        }
        public static IServiceCollection AddDIRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }

        public static IServiceCollection AddDIAccessor(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }

    }
}
