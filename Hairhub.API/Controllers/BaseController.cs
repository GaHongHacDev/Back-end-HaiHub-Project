using AutoMapper;
using Hairhub.API.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.ApiEndpoint)]
    [ApiController]
    //public class BaseController<T> : ControllerBase where T : BaseController<T>
    //{
    //    protected ILogger<T> _logger;
    //    public BaseController(ILogger<T> logger)
    //    {
    //        _logger = logger;
    //    }
    //}
    public class BaseController: ControllerBase
    {
        protected readonly IMapper _mapper;

        public BaseController(IMapper mapper)
        {
            _mapper = mapper;
        }
    }
}
