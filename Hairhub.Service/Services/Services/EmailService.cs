using Hairhub.Domain.Entitities;
using Hairhub.Service.Services.IServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using MailKit.Security;
using Hairhub.Domain.Dtos.Requests.Otps;
using Hairhub.Domain.Dtos.Responses.Otps;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Exceptions;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Hairhub.Service.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public EmailService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }
        private string GenerateOTP(int n)
        {
            string numbers = "1234567890";
            string otpKey = string.Empty;
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                otpKey += numbers[random.Next(0, numbers.Length)];
            }
            return otpKey;
        }

        public async Task<bool> SendEmailAsync(SendOtpEmailRequest sendEmailRequest)
        {
            var otpKey = GenerateOTP(6);
            // Send OTP to Email       
            var emailBody = _configuration["EmailSetting:EmailBody"];
            emailBody = emailBody.Replace("{PROJECT_NAME}", _configuration["Project_HairHub:PROJECT_NAME"]);
            emailBody = emailBody.Replace("{FULL_NAME}", sendEmailRequest.FullName);
            emailBody = emailBody.Replace("{EXPIRE_TIME}", "2");
            emailBody = emailBody.Replace("{OTP}", otpKey);
            emailBody = emailBody.Replace("{PHONE_NUMBER}", _configuration["Project_HairHub:PHONE_NUMBER"]);
            emailBody = emailBody.Replace("{EMAIL_ADDRESS}", _configuration["Project_HairHub:EMAIL_ADDRESS"]);
            var emailHost = _configuration["EmailSetting:EmailHost"];
            var userName = _configuration["EmailSetting:EmailUsername"];
            var password = _configuration["EmailSetting:EmailPassword"];
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailHost));
            email.To.Add(MailboxAddress.Parse(sendEmailRequest.Email));
            email.Subject = _configuration.GetSection("EmailSetting")?["Subject"];
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = emailBody
            };
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(emailHost, 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(userName, password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
            //InserDB
            OTP otp = new OTP()
            {
                Id = Guid.NewGuid(),
                Email = sendEmailRequest.Email,
                OtpKey = otpKey,
                CreatedTime = DateTime.Now,
                ExpireTime = 2,
                TypeOtp = OtpTypeEnum.OtpMail.ToString(),
            };
            otp.EndTime = otp.CreatedTime.GetValueOrDefault().AddMinutes(otp.ExpireTime??=2);
            await _unitOfWork.GetRepository<OTP>().InsertAsync(otp);
            bool isInsertAsync = await _unitOfWork.CommitAsync()>0;
            if (!isInsertAsync)
            {
                throw new Exception("Cannot insert otp to database");
            }
            return true;
        }

        public async Task<bool> SendEmailWithBodyAsync(string emailRequest, string subjectEmail, string fullName, string bodyEmail)
        {
            try
            {
                var emailBody = _configuration["EmailSetting:GeneralEmailBody"];
                emailBody = emailBody.Replace("{PROJECT_NAME}", _configuration["Project_HairHub:PROJECT_NAME"]);
                emailBody = emailBody.Replace("{FULL_NAME}", fullName);
                emailBody = emailBody.Replace("{BODY_EMAIL}", bodyEmail);
                emailBody = emailBody.Replace("{PHONE_NUMBER}", _configuration["Project_HairHub:PHONE_NUMBER"]);
                emailBody = emailBody.Replace("{EMAIL_ADDRESS}", _configuration["Project_HairHub:EMAIL_ADDRESS"]);
                var emailHost = _configuration["EmailSetting:EmailHost"];
                var userName = _configuration["EmailSetting:EmailUsername"];
                var password = _configuration["EmailSetting:EmailPassword"];
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailHost));
                email.To.Add(MailboxAddress.Parse(emailRequest));
                email.Subject = subjectEmail;
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = emailBody
                };
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(emailHost, 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(userName, password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendEmailRegisterAccountAsync(string emailRequest, string subjectEmail, string fullName, string userNameAccount, string passwordAccount)
        {
            try
            {
                var emailBody = _configuration["EmailSetting:EmailRegisterAccount"];
                emailBody = emailBody.Replace("{PROJECT_NAME}", _configuration["Project_HairHub:PROJECT_NAME"]);
                emailBody = emailBody.Replace("{FULL_NAME}", fullName);
                emailBody = emailBody.Replace("{USERNAME}", userNameAccount);
                emailBody = emailBody.Replace("{PASSWORD}", passwordAccount);
                emailBody = emailBody.Replace("{PHONE_NUMBER}", _configuration["Project_HairHub:PHONE_NUMBER"]);
                emailBody = emailBody.Replace("{EMAIL_ADDRESS}", _configuration["Project_HairHub:EMAIL_ADDRESS"]);

                var emailHost = _configuration["EmailSetting:EmailHost"];
                var userName = _configuration["EmailSetting:EmailUsername"];
                var password = _configuration["EmailSetting:EmailPassword"];
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(emailHost));
                email.To.Add(MailboxAddress.Parse(emailRequest));
                email.Subject = subjectEmail;
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = emailBody
                };
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(emailHost, 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(userName, password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendEmailAsyncNotifyOfExpired(string emailIndividual, string  fullname, int REMAINING_DAY, DateTime EXPIRATION_DATE, string LINK_PAYMENT)
        {  
            var emailBody = _configuration["EmailPayment:EmailBody"];
            emailBody = emailBody.Replace("{FULL_NAME_OWNER}", fullname);
            emailBody = emailBody.Replace("{REMAINING_DAY}", REMAINING_DAY.ToString());
            emailBody = emailBody.Replace("{EXPIRATION_DATE}", EXPIRATION_DATE.Date.ToString());
            emailBody = emailBody.Replace("{LINK_PAYMENT}", LINK_PAYMENT);
            emailBody = emailBody.Replace("{PHONE_NUMBER}", _configuration["Project_HairHub:PHONE_NUMBER"]);
            emailBody = emailBody.Replace("{EMAIL_ADDRESS}", _configuration["Project_HairHub:EMAIL_ADDRESS"]);
            var emailHost = _configuration["EmailSetting:EmailHost"];
            var userName = _configuration["EmailSetting:EmailUsername"];
            var password = _configuration["EmailSetting:EmailPassword"];
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailHost));
            email.To.Add(MailboxAddress.Parse(emailIndividual));
            email.Subject = _configuration.GetSection("EmailPayment")?["Subject"];
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = emailBody
            };
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(emailHost, 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(userName, password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            return true;
        }

        public async Task<bool> CheckOtpEmail(CheckOtpRequest checkOtpRequest)
        {
            var otpEmail = await _unitOfWork.GetRepository<OTP>().SingleOrDefaultAsync(
                                                        predicate: x => x.Email.Equals(checkOtpRequest.Email),
                                                        orderBy: y => y.OrderByDescending(y => y.EndTime));
            if (otpEmail == null)
            {
                throw new NotFoundException("OTP không đúng. Vui lòng nhập lại");
            }
            if (otpEmail.EndTime < DateTime.Now)
            {
                throw new Exception("OTP đã hết hạn");
            }
            if (!otpEmail.OtpKey.Equals(checkOtpRequest.OtpRequest))
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CheckExistEmail(CheckExistEmailResrequest checkExistEmailResrequest)
        {
            var email = checkExistEmailResrequest.Email.ToLower().Trim();
            var emailAccount = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.UserName.Equals(email));
            if (emailAccount == null)
            {
                return false;
            }
            return true;
        }
       

    }
}
