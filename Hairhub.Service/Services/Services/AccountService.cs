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
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Domain.Dtos.Responses.Dashboard;
using Org.BouncyCastle.Asn1.Ocsp;


namespace Hairhub.Service.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;
        private readonly IConfiguration _configuaration;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService, IConfiguration configuaration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaService = mediaService;
            _configuaration = configuaration;
        }

        public async Task<bool> ActiveAccount(Guid id)
        {
            var account = await _unitOfWork.GetRepository<Domain.Entitities.Account>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            account.IsActive = true;
            _unitOfWork.GetRepository<Domain.Entitities.Account>().UpdateAsync(account);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> ChangePassword(Guid id, ChangePasswordRequest changePasswordRequest)
        {
            var account = await _unitOfWork.GetRepository<Domain.Entitities.Account>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (!changePasswordRequest.CurrentPassword.Equals(account.Password))
            {
                throw new Exception("Mật khẩu hiện tại không đúng!!");
            }
            if (!changePasswordRequest.NewPassword.Equals(changePasswordRequest.ConfirmNewPassword))
            {
                throw new Exception("Không trùng khớp!!");
            }
            account.Password = changePasswordRequest.NewPassword;
            _unitOfWork.GetRepository<Domain.Entitities.Account>().UpdateAsync(account);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> DeleteAccountById(Guid id)
        {
            var account = await _unitOfWork.GetRepository<Domain.Entitities.Account>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            account.IsActive = false;
            _unitOfWork.GetRepository<Domain.Entitities.Account>().UpdateAsync(account);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<GetAccountResponse> GetAccountById(Guid id)
        {
            GetAccountResponse response = new GetAccountResponse();
            Domain.Entitities.Account account = await _unitOfWork
                .GetRepository<Domain.Entitities.Account>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id == id,
                    include: source => source.Include(a => a.Role)
                );
            if (account == null)
            {
                throw new NotFoundException("Tài khoản không tồn tại");
            }
            if (account.Role.RoleName.Equals(RoleEnum.Customer.ToString()))
            {
                Customer customer = await _unitOfWork.GetRepository<Customer>()
                                                    .SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
                if (customer == null)
                {
                    throw new NotFoundException("Tài khoản không tồn tại");
                }
                response = _mapper.Map<GetAccountResponse>(customer);
                response.Id = customer.Id;
            }
            else if (account.Role.RoleName.Equals(RoleEnum.SalonOwner.ToString()))
            {
                SalonOwner salonOwner = await _unitOfWork.GetRepository<SalonOwner>()
                                    .SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
                if (salonOwner == null)
                {
                    throw new NotFoundException("Tài khoản không tồn tại");
                }
                response = _mapper.Map<GetAccountResponse>(salonOwner);
                response.Id = salonOwner.Id;
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
            CreateAccountResponse createAccountResponse = new CreateAccountResponse();
            var role = await _unitOfWork.GetRepository<Role>().SingleOrDefaultAsync(predicate: x => x.RoleName.Equals(createAccountRequest.RoleName));
            if (role == null)
            {
                throw new Exception("Role not found");
            }
            var userName = await _unitOfWork.GetRepository<Domain.Entitities.Account>().SingleOrDefaultAsync(predicate: x => x.UserName.Equals(createAccountRequest.UserName));
            if (userName != null)
            {
                throw new Exception("Email đã tồn tại!");
            }
            var account = _mapper.Map<Domain.Entitities.Account>(createAccountRequest);
            account.Id = Guid.NewGuid();
            account.RoleId = role.RoleId;
            account.IsActive = true;
            account.CreatedDate = DateTime.UtcNow;

            if (RoleEnum.Customer.ToString().Equals(createAccountRequest.RoleName))
            {
                var customer = _mapper.Map<Customer>(createAccountRequest);
                customer.Id = Guid.NewGuid();
                customer.AccountId = account.Id;
                customer.Img = _configuaration["Default:Avatar_Default"];
                customer.Email = createAccountRequest.UserName;
                customer.NumberOfReported = 0;
                await _unitOfWork.GetRepository<Customer>().InsertAsync(customer);
                createAccountResponse.Img = customer.Img;
            }
            else if (RoleEnum.SalonOwner.ToString().Equals(createAccountRequest.RoleName))
            {
                var salonOwner = _mapper.Map<SalonOwner>(createAccountRequest);
                salonOwner.Id = Guid.NewGuid();
                salonOwner.AccountId = account.Id;
                salonOwner.Img = _configuaration["Default:Avatar_Default"];
                salonOwner.Email = createAccountRequest.UserName;
                await _unitOfWork.GetRepository<SalonOwner>().InsertAsync(salonOwner);
                createAccountResponse.Img = salonOwner.Img;
            }
            else
            {
                throw new Exception("Không thể đăng ký tài khoản");
            }
            await _unitOfWork.GetRepository<Domain.Entitities.Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();
            
            return _mapper.Map(createAccountRequest, createAccountResponse);
        }

        public async Task<bool> UpdateAccountById(Guid id, UpdateAccountRequest updateAccountRequest)
        {
            var account = await _unitOfWork.GetRepository<Domain.Entitities.Account>()
                                            .SingleOrDefaultAsync(
                                                predicate: x => x.Id == id,
                                                include: x => x.Include(y => y.Role));
            if (account == null)
                throw new NotFoundException($"Account was not found with id {id}");
            if (account.Role.RoleName.Equals(RoleEnum.SalonOwner.ToString())) // Salon Owner
            {
                SalonOwner salonOwner = new SalonOwner();
                salonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
                if (salonOwner == null)
                {
                    throw new NotFoundException("Salon ownerwas not found!");
                }
                var urlImg = await _mediaService.UploadAnImage(updateAccountRequest.Img, MediaPath.SALONOWNER_AVATAR, salonOwner.Id.ToString());
                salonOwner.AccountId = account.Id;
                salonOwner.FullName = updateAccountRequest.FullName;
                salonOwner.DayOfBirth = (DateTime)updateAccountRequest.DayOfBirth;
                salonOwner.Gender = updateAccountRequest.Gender;
                salonOwner.Phone = updateAccountRequest.Phone;
                salonOwner.Address = updateAccountRequest.Address;
                salonOwner.Img = urlImg;
                _unitOfWork.GetRepository<SalonOwner>().UpdateAsync(salonOwner);
            }
            else
            {  //Update Customer
                var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
                if (customer == null)
                {
                    throw new NotFoundException("Customer was not found!");
                }
                var urlImg = await _mediaService.UploadAnImage(updateAccountRequest.Img, MediaPath.CUSTOMER_AVATAR, customer.Id.ToString());
                customer.AccountId = account.Id;
                customer.FullName = updateAccountRequest.FullName;
                customer.DayOfBirth = (DateTime)updateAccountRequest.DayOfBirth;
                customer.Gender = updateAccountRequest.Gender;
                customer.Phone = updateAccountRequest.Phone;
                customer.Address = updateAccountRequest.Address;
                customer.Img = urlImg;
                _unitOfWork.GetRepository<Customer>().UpdateAsync(customer);
            }
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<DataOfMonths> GetNumberOfCustomerOnMonth(int? year)
        {
            var accounts = await _unitOfWork.GetRepository<Account>().GetListAsync(predicate: p => p.CreatedDate.Year == year && p.Role.RoleName == RoleEnum.Customer.ToString());
            var dataOfMonths = new DataOfMonths
            {
                Jan = accounts.Count(a => a.CreatedDate.Month == 1),
                Feb = accounts.Count(a => a.CreatedDate.Month == 2),
                March = accounts.Count(a => a.CreatedDate.Month == 3),
                April = accounts.Count(a => a.CreatedDate.Month == 4),
                May = accounts.Count(a => a.CreatedDate.Month == 5),
                June = accounts.Count(a => a.CreatedDate.Month == 6),
                July = accounts.Count(a => a.CreatedDate.Month == 7),
                August = accounts.Count(a => a.CreatedDate.Month == 8),
                September = accounts.Count(a => a.CreatedDate.Month == 9),
                October = accounts.Count(a => a.CreatedDate.Month == 10),
                November = accounts.Count(a => a.CreatedDate.Month == 11),
                December = accounts.Count(a => a.CreatedDate.Month == 12)
            };
            return dataOfMonths;
        }

        public async Task<DataOfMonths> GetNumberOfSalonOwnerOnMonth(int? year)
        {
            var accounts = await _unitOfWork.GetRepository<Account>().GetListAsync(predicate: p => p.CreatedDate.Year == year && p.Role.RoleName == RoleEnum.SalonOwner.ToString());
            var dataOfMonths = new DataOfMonths
            {
                Jan = accounts.Count(a => a.CreatedDate.Month == 1),
                Feb = accounts.Count(a => a.CreatedDate.Month == 2),
                March = accounts.Count(a => a.CreatedDate.Month == 3),
                April = accounts.Count(a => a.CreatedDate.Month == 4),
                May = accounts.Count(a => a.CreatedDate.Month == 5),
                June = accounts.Count(a => a.CreatedDate.Month == 6),
                July = accounts.Count(a => a.CreatedDate.Month == 7),
                August = accounts.Count(a => a.CreatedDate.Month == 8),
                September = accounts.Count(a => a.CreatedDate.Month == 9),
                October = accounts.Count(a => a.CreatedDate.Month == 10),
                November = accounts.Count(a => a.CreatedDate.Month == 11),
                December = accounts.Count(a => a.CreatedDate.Month == 12)
            };
            return dataOfMonths;
        }

        public async Task<int> GetCustomersActive()
        {
            var customers = await _unitOfWork.GetRepository<Customer>().GetListAsync(predicate: p => p.Account.IsActive == true);

            return customers.Count;
        }

        public async Task<int> GetSalonsActive()
        {
            var salons = await _unitOfWork.GetRepository<SalonInformation>().GetListAsync(predicate: p => p.Status == SalonStatus.Approved);

            return salons.Count;
        }

        public async Task<bool> ForgotPassword(ForgotPasswordRequest request)
        {
            var account = await _unitOfWork.GetRepository<Domain.Entitities.Account>().SingleOrDefaultAsync(predicate: x => x.UserName == request.Email);
            if (!request.NewPassword.Equals(request.ConfirmNewPassword))
            {
                throw new Exception("Không trùng khớp!!");
            }
            account.Password = request.NewPassword;
            _unitOfWork.GetRepository<Domain.Entitities.Account>().UpdateAsync(account);
            return await _unitOfWork.CommitAsync() > 0;
        }
    }
}
