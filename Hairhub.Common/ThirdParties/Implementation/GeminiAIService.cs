using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Domain.Dtos.Requests.AI;
using Hairhub.Domain.Dtos.Responses.AI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Common.ThirdParties.Implementation
{
    public class GeminiAIService : IGeminiAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GeminiAIService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
        }

        public async Task<AIChatMessageResponse> ChatMessage(AIChatMessageRequest request)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = request.AskMessage }
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var apiKey = _configuration["AIGemini:Key"];
            var urlRequest = _configuration["AIGemini:Url"];
            var response = await _httpClient.PostAsync($"{urlRequest}{apiKey}", httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {errorContent}");
            }

            string responseJson = await response.Content.ReadAsStringAsync();
            if(responseJson == null)
            {
                throw new Exception("Lỗi không tìm thấy nội dung trả lời của AI");
            }

            var apiResponse = JsonConvert.DeserializeObject<AIChatMessageResponse>(responseJson);
            //var firstCandidate = apiResponse?.Candidates?.FirstOrDefault();
            //var firstPart = firstCandidate?.Content?.Parts?.FirstOrDefault();
            return apiResponse!;
        }
    }
}
