using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.SMS;
using Hairhub.Service.Helpers;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Vonage.Messages.Sms;
using Vonage.Messaging;
using Vonage.Request;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.SMS.SMSEndpoint + "/[action]")]
    [ApiController]
    public class SMSController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.stringee.com/v1/sms";
        private readonly string _apiKeySid;
        private readonly string _apiKeySecret;

        public SMSController()
        {
            _httpClient = new HttpClient();
            _apiKeySid = "SK.0.OoEuNo8nugx9XIlJxV7r5w2O1vnjRTOI";
            _apiKeySecret = "M3hhZkc5VVNOWjBNaVZkcHpOWHhCNUNOOWhVQ01Vb2I=";
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendSms([FromBody] SmsRequest request)
        {
            try
            {
                // Prepare the request payload
                var smsData = new
                {
                    sms = new[]
                    {
                        new { from = request.From, to = request.To, text = request.Text }
                    }
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(smsData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Set Basic Authentication header
                var base64Auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_apiKeySid}:{_apiKeySecret}"));
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Auth);

                // Send POST request to Stringee API
                var response = await _httpClient.PostAsync(ApiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Failed to send SMS: {response.StatusCode}");
                }

                // Read response content
                var responseContent = await response.Content.ReadAsStringAsync();

                return Ok(responseContent);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to send SMS: {ex.Message}");
            }
        }
    }
}
