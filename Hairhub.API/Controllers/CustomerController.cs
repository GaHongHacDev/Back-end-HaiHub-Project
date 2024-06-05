﻿using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Customer.CustomersEndpoint + "/[action]")]
    [ApiController]
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(IMapper mapper, ICustomerService customerService) : base(mapper)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomer([FromQuery] int page=1, [FromQuery] int size=10) {
            try
            {
                var customers = await _customerService.GetCustomers(page, size);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id)
        {
            try
            {
                var Customer = await _customerService.GetCustomerById(id);
                if (Customer == null)
                {
                    return NotFound("Customer not found!");
                }
                return Ok(Customer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
