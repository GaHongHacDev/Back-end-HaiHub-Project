﻿using Hairhub.Domain.Dtos.Requests.ServiceHairs;
using Hairhub.Domain.Dtos.Responses.ServiceHairs;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IServiceHairService
    {
        Task<IPaginate<GetServiceHairResponse>> GetAllServiceHair(int page, int size);
        Task<GetServiceHairResponse>? GetServiceHairById(Guid id);
        Task<CreateServiceHairResponse> CreateServiceHair(CreateServiceHairRequest createAccountRequest);
        Task<bool> UpdateServiceHairById(Guid id, UpdateServiceHairRequest updateServiceHairRequest);
        Task<bool> DeleteServiceHairById(Guid id);
        Task<bool> ActiveServiceHair(Guid id);
    }
}
