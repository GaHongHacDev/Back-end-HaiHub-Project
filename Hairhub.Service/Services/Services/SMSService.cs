using CloudinaryDotNet;
using Hairhub.Domain.Dtos.Requests.SMS;
using Hairhub.Service.Services.IServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;

namespace Hairhub.Service.Services.Services
{
    public class SMSService : ISMSService
    {
        private readonly IConfiguration _config;

        public SMSService(IConfiguration config)
        {
            _config = config;
        }


        public async Task<bool> SendOtpSMS(SendSMS request)
        {
            var accountSID = _config["Twilio:AccountSId"];
            var authToken = _config["Twilio:Authen_token"];
            TwilioClient.Init(accountSID, authToken);

            try
            {
                string formattedPhone = FormatPhoneNumber(request.To);

                var message = await MessageResource.CreateAsync(
                    body: "Your OTP code is 123456", // Thay thế bằng nội dung OTP thực tế của bạn
                    from: new Twilio.Types.PhoneNumber("+12058832017"), // Thay thế bằng số điện thoại Twilio của bạn
                    to: new Twilio.Types.PhoneNumber(formattedPhone)
                );

                // Kiểm tra trạng thái tin nhắn
                if (message.Status == MessageResource.StatusEnum.Queued ||
                    message.Status == MessageResource.StatusEnum.Sent ||
                    message.Status == MessageResource.StatusEnum.Delivered)
                {
                    return true;
                }
                else
                {
                    // Có lỗi xảy ra khi gửi tin nhắn
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có lỗi xảy ra
                Console.WriteLine($"Error sending SMS: {ex.Message}");
                return false;
            }
        }

        private string FormatPhoneNumber(string phone)
        {
            // Kiểm tra xem số điện thoại đã có dấu '+' ở đầu hay chưa
            if (!phone.StartsWith("+"))
            {
                // Thêm mã quốc gia Việt Nam (+84) vào đầu số điện thoại nếu chưa có
                phone = "+84" + phone.TrimStart('0');
            }
            return phone;
        }

    }
}

