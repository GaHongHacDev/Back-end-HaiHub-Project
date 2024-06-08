using Hairhub.Domain.Dtos.Requests.SalonEmployees;
using Hairhub.Domain.Dtos.Responses.SalonEmployees;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface ISalonEmployeeService
    {
        Task<IPaginate<GetSalonEmployeeResponse>> GetAllSalonEmployee(int page, int size);
        Task<GetSalonEmployeeResponse> GetSalonEmployeeById(Guid id);
        Task<GetSalonEmployeeResponse> GetSalonEmployeeBySalonInformationId(Guid SalonInformationId, int page, int size);
        Task<CreateSalonEmployeeResponse> CreateSalonEmployee(CreateSalonEmployeeRequest createAccountRequest);
        Task<bool> UpdateSalonEmployeeById(Guid id, UpdateSalonEmployeeRequest updateSalonEmployeeRequest);
        Task<bool> DeleteSalonEmployeeById(Guid id);
        Task<bool> ActiveSalonEmployee(Guid id);
    }
}
