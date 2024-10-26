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
using Google.Apis.Auth;
using Hairhub.Domain.Specifications;
using Hairhub.Common.Security;
using Hairhub.Domain.Dtos.Responses.Authentication;
using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Service.Helpers;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using System.Security.Principal;
using Role = Hairhub.Domain.Entitities.Role;
using Account = Hairhub.Domain.Entitities.Account;
using System.Data;


namespace Hairhub.Service.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;
        private readonly IConfiguration _configuaration;
        private readonly IEmailService _emailService;
        private string clientIdWeb = "160573115812-l88je63eolr52ichb690e7i8g3f59r9t.apps.googleusercontent.com";
        private string clientIdAndroid = "435735956374-biv1qavtd6b0b79a1372s99v10qfsnpj.apps.googleusercontent.com";
        public AccountService(IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService, IConfiguration configuaration, IEmailService email)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaService = mediaService;
            _configuaration = configuaration;
            _emailService = email;
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
            else if (account.Role.RoleName.Equals(RoleEnum.SalonEmployee.ToString()))
            {
                SalonEmployee salonEmployee = await _unitOfWork.GetRepository<SalonEmployee>()
                                    .SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
                if (salonEmployee == null)
                {
                    throw new NotFoundException("Tài khoản không tồn tại");
                }
                response = _mapper.Map<GetAccountResponse>(salonEmployee);
                response.Id = salonEmployee.Id;
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
            var userName = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.UserName.Equals(createAccountRequest.UserName));
            if (userName != null)
            {
                throw new Exception("Email đã tồn tại!");
            }
            var account = _mapper.Map<Account>(createAccountRequest);
            account.Id = Guid.NewGuid();
            account.RoleId = role.RoleId;
            account.IsActive = true;
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
               // createAccountResponse.Img = customer.Img;
            }
            else if (RoleEnum.SalonOwner.ToString().Equals(createAccountRequest.RoleName))
            {
                var salonOwner = _mapper.Map<SalonOwner>(createAccountRequest);
                salonOwner.Id = Guid.NewGuid();
                salonOwner.AccountId = account.Id;
                salonOwner.Img = _configuaration["Default:Avatar_Default"];
                salonOwner.Email = createAccountRequest.UserName;
                await _unitOfWork.GetRepository<SalonOwner>().InsertAsync(salonOwner);
                //createAccountResponse.Img = salonOwner.Img;
            }
            else
            {
                throw new Exception("Không thể đăng ký tài khoản");
            }
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            bool isSuccess = await _unitOfWork.CommitAsync()>0;
            if (isSuccess)
            {
                await _emailService.SendEmailRegisterAccountAsync(account.UserName, "Tạo tài khoản thành công trên Hairhub", createAccountRequest.FullName, account.UserName, account.Password);
            }
            return createAccountResponse; //_mapper.Map(createAccountRequest, createAccountResponse);
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
                salonOwner.FullName = updateAccountRequest.FullName!;
                salonOwner.DayOfBirth = updateAccountRequest.DayOfBirth!;
                salonOwner.Gender = updateAccountRequest.Gender;
                salonOwner.Phone = updateAccountRequest.Phone!;
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
                customer.FullName = updateAccountRequest.FullName!;
                customer.DayOfBirth = updateAccountRequest.DayOfBirth;
                customer.Gender = updateAccountRequest.Gender!;
                customer.Phone = updateAccountRequest.Phone!;
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

        public async Task<LoginResponse> LoginGoogle(CheckLoginGoogleRequest checkLoginGoogle)
        {
            try
            {
                string clientId = "";
                if(checkLoginGoogle.type == null || checkLoginGoogle.type.Equals("Web"))
                {
                    clientId = clientIdWeb;
                }
                else if (checkLoginGoogle.type.Equals("Android"))
                {
                    clientId = clientIdAndroid;
                }
                // Xác thực token với Google
                var payload = await GoogleJsonWebSignature.ValidateAsync(checkLoginGoogle.IdToken, new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { clientId }
                });

                if (payload == null)
                {
                    throw new Exception("Không có dữ liệu từ Google API");
                }
                // Lấy thông tin người dùng từ payload
                
                if(payload.Email == null)
                {
                    throw new Exception("Không có dữ liệu email từ Google API");
                }
                var email = payload.Email;
                var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.UserName.Equals(email), include: x=>x.Include(s => s.Role));
                if (account == null)
                {
                    throw new NotFoundException("Email không tồn tại trên hệ thống");
                }
                SalonOwner salonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
                Customer customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
                SalonEmployee salonEmployee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
                if (salonOwner == null && customer == null && salonEmployee == null)
                {
                    throw new Exception("Không tìm thấy tài khoản");
                }
                var accessToken = JWTHelper.GenerateToken(account.UserName, account.Role.RoleName!, _configuaration["JWTSettings:Key"]!, _configuaration["JWTSettings:Issuer"]!, _configuaration["JWTSettings:Audience"]!);
                var refreshToken = JWTHelper.GenerateRefreshToken();
                var newRefrehToken = new RefreshTokenAccount()
                {
                    Id = Guid.NewGuid(),
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccountId = account.Id,
                    Expires = DateTime.UtcNow.AddDays(30),
                };
                await _unitOfWork.GetRepository<RefreshTokenAccount>().InsertAsync(newRefrehToken);

                bool isInsert = await _unitOfWork.CommitAsync() > 0;
                return new LoginResponse()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccountId = account.Id,
                    RoleName = account.Role?.RoleName,
                    CustomerResponse = customer != null ? _mapper.Map<CustomerLoginResponse>(customer) : null,
                    SalonOwnerResponse = salonOwner != null ? _mapper.Map<SalonOwnerLoginResponse>(salonOwner) : null,
                    SalonEmployeeResponse = salonEmployee != null ? _mapper.Map<SalonEmployeeResponse>(salonEmployee) : null,
                };
            }
            catch (InvalidJwtException ex)
            {
                throw new Exception("Id Token hoặc JWT không hợp lệ");
            }
            catch(NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<LoginResponse> RegisterAccountLoginGoogle(CreateAccountLoginGoogleRequest createAccountLoginGoogleRequest)
        {
            string clientId = "";
            if (createAccountLoginGoogleRequest.type == null || createAccountLoginGoogleRequest.type.Equals("Web"))
            {
                clientId = clientIdWeb;
            }
            else if (createAccountLoginGoogleRequest.type.Equals("Android"))
            {
                clientId = clientIdAndroid;
            }
            // Xác thực token với Google
            var payload = await GoogleJsonWebSignature.ValidateAsync(createAccountLoginGoogleRequest.IdToken, new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new[] { clientId }
            });

            if (payload == null)
            {
                throw new Exception("Lỗi không có dữ liệu từ Google API");
            }
            // Lấy thông tin người dùng từ payload
            if (payload.Email == null || payload.Name == null)
            {
                throw new Exception("Không có dữ liệu email, name từ Google API");
            }
            var existEmail = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.UserName.Equals(payload.Email));
            if (existEmail != null)
            {
                throw new NotFoundException("Email đã tồn tại trên hệ thống");
            }

            var role = await _unitOfWork.GetRepository<Role>().SingleOrDefaultAsync(predicate: x => x.RoleName.Equals(createAccountLoginGoogleRequest.RoleName));
            if (role == null)
            {
                throw new Exception("Role not found");
            }

            var account = new Account()
            {
                Id = Guid.NewGuid(),
                UserName = payload.Email,
                Password = AesEncoding.GenerateRandomPassword(),
                RoleId = role.RoleId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            Customer customer = null!;
            SalonOwner salonOwner = null!;
            if (RoleEnum.Customer.ToString().Equals(createAccountLoginGoogleRequest.RoleName))
            {
                customer = new Customer() 
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    Img = _configuaration["Default:Avatar_Default"],
                    Email = account.UserName,
                    Phone = createAccountLoginGoogleRequest.Phone,
                    FullName = payload.Name,
                    NumberOfReported = 0,
                };
                
                await _unitOfWork.GetRepository<Customer>().InsertAsync(customer);
            }
            else if (RoleEnum.SalonOwner.ToString().Equals(createAccountLoginGoogleRequest.RoleName))
            {
                salonOwner = new SalonOwner()
                {
                    Id = Guid.NewGuid(),
                    AccountId = account.Id,
                    Img = _configuaration["Default:Avatar_Default"],
                    Email = account.UserName,
                    Phone = createAccountLoginGoogleRequest.Phone,
                    FullName = payload.Name
                };
                
                await _unitOfWork.GetRepository<SalonOwner>().InsertAsync(salonOwner);
            }
            else
            {
                throw new Exception("Không thể đăng ký tài khoản");
            }

            await _unitOfWork.GetRepository<Account>().InsertAsync(account);

            // authentication successful so generate jwt token and refresh token
            var accessToken = JWTHelper.GenerateToken(account.UserName, role.RoleName!, _configuaration["JWTSettings:Key"]!, _configuaration["JWTSettings:Issuer"]!, _configuaration["JWTSettings:Audience"]!);
            var refreshToken = JWTHelper.GenerateRefreshToken();
            var newRefrehToken = new RefreshTokenAccount()
            {
                Id = Guid.NewGuid(),
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccountId = account.Id,
                Expires = DateTime.UtcNow.AddDays(30),
            };
            await _unitOfWork.GetRepository<RefreshTokenAccount>().InsertAsync(newRefrehToken);

            bool isInsert = await _unitOfWork.CommitAsync() > 0;
            if (isInsert)
            {
                await _emailService.SendEmailRegisterAccountAsync(payload.Email, "Tạo tài khoản thành công trên Hairhub", payload.Name, account.UserName, account.Password);
            }
            else
            {
                throw new Exception("Lỗi không thể lưu vào thông tin vào Database");
            }

            return new LoginResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccountId = account.Id,
                RoleName = role.RoleName!,
                CustomerResponse = customer != null ? _mapper.Map<CustomerLoginResponse>(customer) : null,
                SalonOwnerResponse = salonOwner != null ? _mapper.Map<SalonOwnerLoginResponse>(salonOwner) : null,
            };
        }
    }
}
