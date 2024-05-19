using Hairhub.Domain.Entitities;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Hairhub.Domain.Dtos.Requests.Accounts;
using AutoMapper;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Exceptions;

namespace Hairhub.Service.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService( IUnitOfWork unitOfWork, IMapper mapper)
        {

            this._unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> ActiveAccount(Guid id)
        {
            var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (customer == null)
            {
                SalonOwner salonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.Id == id);
                if (salonOwner == null)
                {
                    throw new NotFoundException("This account is not exist!");
                }
                var accountSalonOwner = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == salonOwner.AccountId);
                accountSalonOwner.IsActive = true;
                _unitOfWork.GetRepository<Account>().UpdateAsync(accountSalonOwner);
                return await _unitOfWork.CommitAsync() > 0;
            }
            var accountCustomer = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == customer.AccountId);
            accountCustomer.IsActive = true;
            _unitOfWork.GetRepository<Account>().UpdateAsync(accountCustomer);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> ChangePassword(Guid id, ChangePasswordRequest changePasswordRequest)
        {
            var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (customer == null)
            {
                SalonOwner salonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.Id == id);
                if (salonOwner == null)
                {
                    throw new NotFoundException("This account is not exist!");
                }
                var accountSalonOwner = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == salonOwner.AccountId);
                if (!changePasswordRequest.CurrentPassword.Equals(accountSalonOwner.Password))
                {
                    throw new Exception("Current password is not correct!");
                }
                accountSalonOwner.Password = changePasswordRequest.NewPassword;
                _unitOfWork.GetRepository<Account>().UpdateAsync(accountSalonOwner);
                return await _unitOfWork.CommitAsync() > 0;
            }
            var accountCustomer = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == customer.AccountId);
            if (!changePasswordRequest.CurrentPassword.Equals(accountCustomer.Password))
            {
                throw new Exception("Current password is not correct!");
            }
            accountCustomer.Password = changePasswordRequest.NewPassword;
            _unitOfWork.GetRepository<Account>().UpdateAsync(accountCustomer);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> DeleteAccountById(Guid id)
        {
            var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if(customer == null)
            {
                SalonOwner salonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.Id == id);
                if (salonOwner == null)
                {
                    throw new NotFoundException("This account is not exist!");
                }
                var accountSalonOwner = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == salonOwner.AccountId);
                accountSalonOwner.IsActive = false;
                _unitOfWork.GetRepository<Account>().UpdateAsync(accountSalonOwner);
                return await _unitOfWork.CommitAsync()>0;
            }
            var accountCustomer = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == customer.AccountId);
            accountCustomer.IsActive = false;
            _unitOfWork.GetRepository<Account>().UpdateAsync(accountCustomer);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<CreateAccountResponse> RegisterAccount(CreateAccountRequest createAccountRequest)
        {
            var role = await _unitOfWork.GetRepository<Role>().SingleOrDefaultAsync(predicate: x => x.RoleName.Equals(createAccountRequest.RoleName));
            if (role == null)
            {
                throw new Exception("Role not found");
            }
            var account = _mapper.Map<Account>(createAccountRequest);
            if (RoleEnum.Customer.ToString().Equals(createAccountRequest.RoleName))
            {
                var userInfor = _mapper.Map<Customer>(createAccountRequest);
                account.Id = Guid.NewGuid();
                account.RoleId = role.RoleId;
                account.IsActive = true;
                userInfor.Id = Guid.NewGuid();
                userInfor.AccountId = account.Id;
                await _unitOfWork.GetRepository<Customer>().InsertAsync(userInfor);
            }
            else if (RoleEnum.SalonOwner.ToString().Equals(createAccountRequest.RoleName))
            {
                var userInfor = _mapper.Map<SalonOwner>(createAccountRequest);
                account.Id = Guid.NewGuid();
                account.RoleId = role.RoleId;
                account.IsActive = true;
                userInfor.Id = Guid.NewGuid();
                userInfor.AccountId = account.Id;
                await _unitOfWork.GetRepository<SalonOwner>().InsertAsync(userInfor);
            }
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CreateAccountResponse>(createAccountRequest);
        }

        public async Task<bool> UpdateAccountById(Guid id, UpdateAccountRequest updateAccountRequest)
        {
            var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            SalonOwner salonOwner = new SalonOwner();
            Guid accountId;
            if (customer == null)
            {
                salonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.Id == id);
                if (salonOwner == null)
                {
                    throw new NotFoundException("Salon owner or customer was not found!");
                }
                accountId = salonOwner.AccountId;
                salonOwner.FullName = updateAccountRequest.FullName;
                salonOwner.DayOfBirth = updateAccountRequest.DayOfBirth;
                salonOwner.Gender = updateAccountRequest.Gender;
                salonOwner.Email = updateAccountRequest.Email;
                salonOwner.Phone = updateAccountRequest.Phone;
                salonOwner.Address = updateAccountRequest.Address;
                salonOwner.HumanId = updateAccountRequest.HumanId;
                salonOwner.Img = updateAccountRequest.Img;
                salonOwner.BankAccount = updateAccountRequest.BankAccount;
                salonOwner.BankName = updateAccountRequest.BankName;
            }
            else
            {
                accountId = customer.AccountId;
                customer.FullName = updateAccountRequest.FullName;
                customer.DayOfBirth = updateAccountRequest.DayOfBirth;
                customer.Gender = updateAccountRequest.Gender;
                customer.Email = updateAccountRequest.Email;
                customer.Phone = updateAccountRequest.Phone;
                customer.Address = updateAccountRequest.Address;
                customer.HumanId = updateAccountRequest.HumanId;
                customer.Img = updateAccountRequest.Img;
                customer.BankAccount = updateAccountRequest.BankAccount;
                customer.BankName = updateAccountRequest.BankName;
            }
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == accountId);
            if (account == null)
                throw new NotFoundException("Account was not found");
            //Update Account
            account.Username = updateAccountRequest.Username;
            account.Password = updateAccountRequest.Password;
            if (customer != null)
            {
                _unitOfWork.GetRepository<Customer>().UpdateAsync(customer);
            }
            else
            {
                _unitOfWork.GetRepository<SalonOwner>().UpdateAsync(salonOwner);
            }
            _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}
