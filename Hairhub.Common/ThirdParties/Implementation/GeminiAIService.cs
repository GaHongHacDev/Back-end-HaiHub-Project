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

        private async Task<string> SendMessageDefault(string askCustomer)
        {
            var promptStr = $@"Câu hỏi của người dùng trên hệ thống Hairhub như sau: ""{askCustomer}"". Hãy giúp Hairhub soạn 1 nội dung trả lời lại 
                                khách hàng với nội dung chính như sau: ""Chào mừng bạn đến với Hairhub – Hệ thống kết nối giữa Salon/Barber Shop và khách hàng! 
                                Tôi là Hairhub chatbot. Để tôi có thể hỗ trợ tốt hơn, vui lòng cung cấp câu hỏi liên quan đến một trong các chủ đề sau:
                                1️. Kiểm tra lịch hẹn.
                                2️. Tìm khuyến mãi hiện có.
                                3️. Hướng dẫn sử dụng Hairhub.
                                4️. Tìm thời gian đặt lịch phù hợp.
                                5️. Tìm salon hoặc barber shop gần bạn.
                                Hairhub Chatbot rất hân hạnh được hỗ trợ bạn"" ";
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = promptStr}
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
            if (responseJson == null)
            {
                throw new Exception("Lỗi không tìm thấy nội dung trả lời của AI");
            }

            var apiResponse = JsonConvert.DeserializeObject<AIChatMessageResponse>(responseJson);
            var classificationText = apiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "";
            return classificationText;
        }
        public async Task<string> ChatMessage(AIChatMessageRequest request)
        {
            var promptStr = $@"Tóm tắt câu hỏi sau của một khách hàng đặt lịch cắt tóc trên hệ thống Hairhub, câu hỏi như sau: ""{request.AskMessage}""

Lưu ý: 
1. Kết quả trả về được viết dưới dạng text, tập trung vào phân tích và tóm tắt câu hỏi của khách hàng về **lịch hẹn**, **dịch vụ tóc**, **salon**, **barber shop**, **khuyến mãi**, **hướng dẫn sử dụng Hairhub**. 
2. Bỏ qua các thông tin không liên quan đến các chủ đề trên. 
3. **[Thời gian]** phải được viết dưới dạng **""dd/MM/yyyy HH:mm:ss""**. Ngày hôm nay là: ""{DateTime.Now.Date.ToString("dd/MM/yyyy HH:mm:ss")}"". 
4. Các từ ngữ thời gian tự nhiên như ""hôm nay"", ""ngày mai"", ""cuối tuần"" cần được chuyển thành thời gian cụ thể dựa trên ngày hiện tại. **Ví dụ**, nếu thời gian hiện tại là **""24/10/2023 15:00:00""** và khách hàng nói ""hôm nay"", thì **[Thời gian]** sẽ là **""24/10/2023 15:00:00""**.
5.Nếu không có thông tin thì trả lời ""null"".
6.Câu trả lời cần ngắn gọn, chỉ trả về dữ liệu dưới dạng text và** không được trình bày dưới dạng text box * *.

Kết quả trả lời cần tuân thủ theo định dạng 6 cột sau(Không trả lời thêm ngoài 6 dòng bên dưới):

[Loại câu hỏi]: [Kiểm tra lịch hẹn, Tìm khuyến mãi, Hướng dẫn sử dụng Hairhub, Tìm thời gian đặt lịch, Tìm salon hoặc barber shop, null];

            [Loại hướng dẫn sử dụng]: [Đặt lịch hẹn, Hủy lịch hẹn, Quy trình check in, Xem lịch sử lịch hẹn, Xem trạng thái lịch hẹn, null];

            [Trạng thái lịch hẹn]: [Hủy, Đang đặt, Đang, Thành công, Hoàn thành, Thất bại, null];

            [Vị trí]: [Gần tôi, Địa điểm, null];

            [Tên Salon hoặc tên Barber shop]: [tên salon, tên barber shop, null];

            [Thời gian]: [Ngày và giờ cụ thể trong câu hỏi khách hàng, nếu có, theo định dạng ""dd / MM / yyyy HH: mm: ss""; nếu không có thì trả lời ""null""]";



            var promptStr1 = $@"Tóm tắt câu hỏi sau của 1 khách hàng đặt lịch cắt tóc trên hệ thống Hairhub, câu hỏi như sau: ""{request.AskMessage}""

                            Lưu ý: Kết quả trả về được viết dưới dạng text, tập trung vào phân tích và tóm tắt câu hỏi của khách hàng về 
                            lịch hẹn, dịch vụ tóc, salon, barber shop, khuyến mãi, hướng dẫn sử dụng Hairhub. 
                            Bỏ qua các thông khác trong câu hỏi không liên quan đến lịch hẹn, dịch vụ tóc, salon, barber shop, khuyến mãi, hướng dẫn sử dụng Hairhub. 
                            [Thời gian] phải được viết dưới dạng ""MM/dd/yyyy HH:mm:ss"", thời gian hiện tại đang là: ""{DateTime.Now}"". Phân biệt rõ ràng về ý định của câu hỏi về [Loại hướng dẫn sử dụng] hay những ý định khác.
                            Nếu không có thông tin thì trả lời ""null"". 
                            Câu trả lời cần ngắn gọn, chỉ trả về dữ liệu dưới dạng text và KHÔNG được dưới dạng text box. 
                            Kết quả trả lời ứng với cột dữ liệu sau, chỉ trả lời với 6 cột dữ liệu bên dưới (Không trả lời thêm ngoài 6 dòng bên dưới):

                            [Loại câu hỏi]: [Kiểm tra lịch hẹn, Tìm khuyến mãi, Hướng dẫn sử dụng Hairhub, Tìm thời gian đặt lịch, Tìm salon hoặc barber shop, null];
                            [Loại hướng dẫn sử dụng]: [Đặt lịch hẹn, Hủy lịch hẹn, Quy trình check in, Xem lịch sử lịch hẹn, Xem trạng thái lịch hẹn]; 
                            [Trạng thái lịch hẹn]: [Hủy, Đang đặt, Đang, Thành công, Hoàn thành, Thất bại];
                            [Vị trí]: [Gần tôi, Địa điểm, null];
                            [Tên Salon hoặc tên Barber shop]: [tên salon, tên barber shop, null];
                            [Thời gian]: [Thời gian trong câu hỏi lời của khách hàng theo định dạng ""MM/dd/yyyy HH:mm:ss"", null];";
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = promptStr}
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
            if (responseJson == null)
            {
                throw new Exception("Lỗi không tìm thấy nội dung trả lời của AI");
            }

            var apiResponse = JsonConvert.DeserializeObject<AIChatMessageResponse>(responseJson);
            var classificationText = apiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
            return classificationText;
            if (classificationText != null)
            {
                var lines = classificationText.Split(new[] { '\n', '\r', ';'}, StringSplitOptions.RemoveEmptyEntries);
                var clasifyAskCustomer = new ClassificationResult();
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    switch (trimmedLine)
                    {
                        case string s when s.StartsWith("[Loại câu hỏi]:"):
                            clasifyAskCustomer.Intent = ExtractValue(s);
                            break;
                        case string s when s.StartsWith("[Loại hướng dẫn sử dụng]:"):
                            clasifyAskCustomer.TyleGuid = ExtractValue(s);
                            break;
                        case string s when s.StartsWith("[Trạng thái lịch hẹn]:"):
                            clasifyAskCustomer.StatusAppointment = ExtractValue(s);
                            break;
                        case string s when s.StartsWith("[Vị trí]:"):
                            clasifyAskCustomer.Position = ExtractValue(s);
                            break;
                        case string s when s.StartsWith("[Tên Salon hoặc tên Barber shop]:"):
                            clasifyAskCustomer.SalonName = ExtractValue(s);
                            break;
                        case string s when s.StartsWith("[Thời gian]:"):
                            clasifyAskCustomer.Time = ExtractValue(s);
                            break;
                        default:
                            break;
                    }
                }

                switch (clasifyAskCustomer.Intent)
                {
                    case "Kiểm tra lịch hẹn":
                        switch (clasifyAskCustomer.StatusAppointment)
                        {
                            case "Hủy":
                                break;
                            case "Đang đặt" or "Đang":
                                break;
                            case "Thành công" or "Hoàn thành":
                                break;
                            case "Thất bại":
                                break;
                            case null:
                                //thua, không có trạng thái sao mà kiểm tra???
                                break;
                            default:
                                //thua, không có trạng thái sao mà kiểm tra???
                                break;
                        }
                        break;
                    //Chưa nghĩ ra
                    case "Tìm khuyến mãi":
                        break;
                    case "Hướng dẫn sử dụng Hairhub":
                        switch (clasifyAskCustomer.TyleGuid)
                        {
                            case "Đặt lịch hẹn":
                                break;
                            case "Hủy lịch hẹn":
                                break;
                            case "Quy trình check in":
                                break;
                            case "Xem lịch sử lịch hẹn":
                                break;
                            case "Xem trạng thái lịch hẹn":
                                break;
                            case null:
                                //Cần cung cấp thêm thông tin là hướng dẫn sử dụng về gì?
                                break;
                            default:
                                //Chưa có thông tin cho hướng dẫn Hairhub này
                                break;
                        }
                        break;
                    case "Tìm thời gian đặt lịch":
                        break;
                    case "Tìm salon hoặc barber shop":
                        break;
                    case null:
                        return await SendMessageDefault(request.AskMessage);
                    default:
                        //Tôi không biết bạn đang hỏi qq dì cả???
                        break;
                }

                Console.WriteLine("*************************" + clasifyAskCustomer);

            }
            //return apiResponse!;
            return "";
        }

        private string ExtractValue(string line)
        {
            var parts = line.Split(new[] { ':' }, 2);
            if (parts.Length < 2)
                return "null";

            var valuePart = parts[1].Trim();
            // Remove square brackets if present
            if (valuePart.StartsWith("[") && valuePart.EndsWith("]"))
                valuePart = valuePart.Substring(1, valuePart.Length - 2).Trim();

            // Handle "null" case
            if (string.Equals(valuePart, "null", StringComparison.OrdinalIgnoreCase))
                return null;

            return valuePart;
        }
    }
}
