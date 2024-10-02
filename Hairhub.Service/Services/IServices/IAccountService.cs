using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
using Hairhub.Domain.Dtos.Responses.Authentication;
using Hairhub.Domain.Dtos.Responses.Dashboard;
using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IAccountService
    {
        Task<CreateAccountResponse> RegisterAccount(CreateAccountRequest createAccountRequest);
        Task<bool> UpdateAccountById(Guid id, UpdateAccountRequest updateAccountRequest);
        Task<bool> DeleteAccountById(Guid id);
        Task<bool> ActiveAccount(Guid id);
        Task<bool> ChangePassword(Guid id, ChangePasswordRequest changePasswordRequest);
        Task<GetAccountResponse> GetAccountById(Guid id);
        Task<DataOfMonths> GetNumberOfCustomerOnMonth(int? year);
        Task<DataOfMonths> GetNumberOfSalonOwnerOnMonth(int? year);
        Task<int> GetCustomersActive();
        Task<int> GetSalonsActive();
        Task<bool> ForgotPassword(ForgotPasswordRequest request);
        Task<LoginResponse> LoginGoogle(CheckLoginGoogleRequest checkLoginGoogle);
        Task<LoginResponse> RegisterAccountLoginGoogle(CreateAccountLoginGoogleRequest createAccountLoginGoogleRequest);
    }
}
