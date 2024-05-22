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
            
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["EmailSetting:EmailHost"]));
            email.To.Add(MailboxAddress.Parse(sendEmailRequest.Email));
            email.Subject = _configuration.GetSection("EmailSetting")?["Subject"];
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = emailBody
            };
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration.GetSection("EmailSetting")?["EmailHost"], 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_configuration.GetSection("EmailSetting")?["EmailUsername"], _configuration.GetSection("EmailSetting")?["EmailPassword"]);
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

        public async Task<bool> CheckOtpEmail(CheckOtpRequest checkOtpRequest)
        {
            var otpEmail = await _unitOfWork.GetRepository<OTP>().SingleOrDefaultAsync(
                                                        predicate: x => x.Email.Equals(checkOtpRequest.Email),
                                                        orderBy: y => y.OrderByDescending(y => y.EndTime));
            if (otpEmail == null)
            {
                throw new NotFoundException("Cannot found otp!");
            }
            if (otpEmail.EndTime < DateTime.Now)
            {
                throw new Exception("Time out OTP!");
            }
            if (!otpEmail.OtpKey.Equals(checkOtpRequest.OtpRequest))
            {
                return false;
            }
            return true;
        }
    }
}
