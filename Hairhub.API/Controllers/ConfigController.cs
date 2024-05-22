using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Config.ConfigEndpoint + "/[action]")]
    [ApiController]
    public class ConfigController : BaseController
    {
        private readonly IConfigService _configservice;

        public ConfigController(IMapper mapper, IConfigService configservice) : base(mapper)
        {
            _configservice = configservice;
        }
    }
}
