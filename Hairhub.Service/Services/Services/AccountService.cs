using Hairhub.Domain.Entitities;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Hairhub.Domain.Dtos.Requests.Accounts;
using AutoMapper;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace Hairhub.Service.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;
        private readonly IConfiguration _configuaration;

        public AccountService( IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService, IConfiguration configuaration)
        {

            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaService = mediaService;
            _configuaration = configuaration;
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

        public async Task<GetAccountResponse> GetAccountById(Guid id)
        {
            GetAccountResponse response = new GetAccountResponse();
            Account account = await _unitOfWork
                .GetRepository<Account>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id==id,
                    include: source => source.Include(a => a.Role)
                 );
            if (account == null)
            {
                throw new NotFoundException("Tài khoản không tồn tại");
            }
            if (account.Role.RoleName.Equals(RoleEnum.Customer.ToString())){
                Customer customer = await _unitOfWork.GetRepository<Customer>()
                                                    .SingleOrDefaultAsync(predicate: x=>x.AccountId == account.Id);
                if (customer == null)
                {
                    throw new NotFoundException("Tài khoản không tồn tại");
                }
                response = _mapper.Map<GetAccountResponse>(customer);
            }else if (account.Role.RoleName.Equals(RoleEnum.SalonOwner.ToString()))
            {
                SalonOwner salonOwner = await _unitOfWork.GetRepository<SalonOwner>()
                                    .SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
                if (salonOwner == null)
                {
                    throw new NotFoundException("Tài khoản không tồn tại");
                }
                response = _mapper.Map<GetAccountResponse>(salonOwner);
            }
            else
            {
                throw new NotFoundException("Role không tồn tại");
            }
            response = _mapper.Map(account, response);
            return response;
        }

        public async Task<CreateAccountResponse> RegisterAccount(CreateAccountRequest createAccountRequest)
        {
            var role = await _unitOfWork.GetRepository<Role>().SingleOrDefaultAsync(predicate: x => x.RoleName.Equals(createAccountRequest.RoleName));
            if (role == null)
            {
                throw new Exception("Role not found");
            }
            var userName = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x=>x.Username.Equals(createAccountRequest.Username));
            if (userName != null)
            {
                throw new Exception("Username đã tồn tại!");
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
                userInfor.Img = _configuaration["Default:Avatar_Default"];
                await _unitOfWork.GetRepository<Customer>().InsertAsync(userInfor);
            }
            else if (RoleEnum.SalonOwner.ToString().Equals(createAccountRequest.RoleName))
            {
                var salonInfo = _mapper.Map<SalonOwner>(createAccountRequest);
                account.Id = Guid.NewGuid();
                account.RoleId = role.RoleId;
                account.IsActive = true;
                salonInfo.Id = Guid.NewGuid();
                salonInfo.AccountId = account.Id;
                salonInfo.Img = _configuaration["Default:Avatar_Default"];
                await _unitOfWork.GetRepository<SalonOwner>().InsertAsync(salonInfo);
            }
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CreateAccountResponse>(createAccountRequest);
        }

        public async Task<bool> UpdateAccountById(Guid id, UpdateAccountRequest updateAccountRequest)
        {
            Guid accountId;
            if (updateAccountRequest.RoleName.Equals(RoleEnum.SalonOwner.ToString())) // Salon Owner
            {
                SalonOwner salonOwner = new SalonOwner();
                salonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.Id == id);
                if (salonOwner == null)
                {
                    throw new NotFoundException("Salon ownerwas not found!");
                }
                var urlImg = await _mediaService.UploadAnImage(updateAccountRequest.Img, MediaPath.SALON_AVATAR, salonOwner.Id.ToString());
                accountId = (Guid)salonOwner.AccountId;
                salonOwner.FullName = updateAccountRequest.FullName;
                salonOwner.DayOfBirth = updateAccountRequest.DayOfBirth;
                salonOwner.Gender = updateAccountRequest.Gender;
                salonOwner.Email = updateAccountRequest.Email;
                salonOwner.Phone = updateAccountRequest.Phone;
                salonOwner.Address = updateAccountRequest.Address;
                salonOwner.Img = urlImg;
                salonOwner.BankAccount = updateAccountRequest.BankAccount;
                salonOwner.BankName = updateAccountRequest.BankName;
                _unitOfWork.GetRepository<SalonOwner>().UpdateAsync(salonOwner);
            }
            else
            {  //Update Customer
                var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.Id == id);
                if (customer == null)
                {
                    throw new NotFoundException("Customer was not found!");
                }
                var urlImg = await _mediaService.UploadAnImage(updateAccountRequest.Img, MediaPath.CUSTOMER_AVATAR, customer.Id.ToString());
                accountId = (Guid)customer.AccountId;
                customer.FullName = updateAccountRequest.FullName;
                customer.DayOfBirth = updateAccountRequest.DayOfBirth;
                customer.Gender = updateAccountRequest.Gender;
                customer.Email = updateAccountRequest.Email;
                customer.Phone = updateAccountRequest.Phone;
                customer.Address = updateAccountRequest.Address;
                customer.Img = urlImg;
                customer.BankAccount = updateAccountRequest.BankAccount;
                customer.BankName = updateAccountRequest.BankName;
                _unitOfWork.GetRepository<Customer>().UpdateAsync(customer);
            }
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == accountId);
            if (account == null)
                throw new NotFoundException("Account was not found");
            //Update Account
            account.Username = updateAccountRequest.Username;
            account.Password = updateAccountRequest.Password;
            _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}
