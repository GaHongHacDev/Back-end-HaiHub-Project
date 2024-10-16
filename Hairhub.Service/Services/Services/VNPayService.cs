using AutoMapper;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class VNPayService : IVNPayService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfigService _configService;

        public VNPayService(IUnitOfWork unitOfWork, IMapper mapper, IConfigService configService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configService = configService;
        }

        public Task<string> Pay()
        {
            //Get Config Info
            //string vnp_Returnurl = _configService.AppSettings["vnp_Returnurl"]; 
            //string vnp_Url = _configService.AppSettings["vnp_Url"];  
            //string vnp_TmnCode = _configService.AppSettings["vnp_TmnCode"]; 
            //string vnp_HashSecret = _configService.AppSettings["vnp_HashSecret"]; 
            //if (string.IsNullOrEmpty(vnp_TmnCode) || string.IsNullOrEmpty(vnp_HashSecret))
            //{
            //    lblMessage.Text = "Vui lòng cấu hình các tham số: vnp_TmnCode,vnp_HashSecret trong file web.config";
            //    return;
            //}
            throw new NotImplementedException();
        }
    }
}
