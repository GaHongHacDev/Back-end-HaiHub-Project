using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.BusyScheduleEmployee.BusyScheduleEmployeeEndpoint + "/[action]")]
    [ApiController]
    public class BusyScheduleEmployeeController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IBusyScheduleEmployeeSerivce _busyScheduleEmployeeSerivce;

        public BusyScheduleEmployeeController(IMapper mapper, IBusyScheduleEmployeeSerivce busyScheduleEmployeeSerivce) : base(mapper)
        {
            this.mapper = mapper;
            _busyScheduleEmployeeSerivce = busyScheduleEmployeeSerivce;
        }
    }
}
