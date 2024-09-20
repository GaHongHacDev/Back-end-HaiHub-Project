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
        Task<IPaginate<GetSalonEmployeeResponse>> GetSalonEmployeeBySalonInformationId(Guid SalonInformationId, int page, int size, bool? orderName, bool? isActive, string? nameEmployee);
        Task<bool> CreateSalonEmployee(CreateSalonEmployeeRequest createAccountRequest);
        Task<bool> UpdateSalonEmployeeById(Guid id, UpdateSalonEmployeeRequest updateSalonEmployeeRequest);
        Task<bool> DeleteSalonEmployeeById(Guid id);
        Task<bool> ActiveSalonEmployee(Guid id);
        Task<bool> CreateAccountEmployee(CreateAccountEmployeeRequest request);
        Task<IList<GetEmployeeHighRatingResponse>> GetEmployeeHighRating(int? numberOfDay);
    }
}
