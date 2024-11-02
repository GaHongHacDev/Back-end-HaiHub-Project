
using Hairhub.Domain.Dtos.Requests.AI;
using Hairhub.Domain.Dtos.Responses.AI;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Enums;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hairhub.Service.Services.Services
{
    public class GeminiAIService : IGeminiAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentService _appointmentService;

        public GeminiAIService(IConfiguration configuration, IUnitOfWork unitOfWork, IAppointmentService appointmentService)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _appointmentService = appointmentService;
        }

        private string ExtractValue(string line)
        {
            var parts = line.Split(new[] { ':' }, 2);
            if (parts.Length < 2)
                return "null";

            var valuePart = parts[1].Trim();
            if (valuePart.StartsWith("[") && valuePart.EndsWith("]"))
                valuePart = valuePart.Substring(1, valuePart.Length - 2).Trim();
            if (string.Equals(valuePart, "null", StringComparison.OrdinalIgnoreCase))
                return null;

            return valuePart;
        }

        private DateTime? ConvertToDateTime(string input)
        {
            if (string.IsNullOrEmpty(input) || (input.Trim().ToLower().Contains("null")&&!input.Trim().ToLower().Contains("[thời gian]")))
            {
                return null;
            }

            // Extract date and time part from the input using regex or substring
            string cleanedInput = input.Replace("[Thời gian]:", "").Trim();

            string[] formats = { "dd/MM/yyyy HH:mm", "dd/MM/yyyy" };

            if (DateTime.TryParseExact(cleanedInput, formats,
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None,
                                       out DateTime result))
            {
                return result;
            }

            return null;
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

        private async Task<string> CallGeminiAPI(string customerAsk)
        {
            var promptAsk = $@"Bạn là Hairhub Chatbot, một trợ lý hỗ trợ khách hàng cho ứng dụng đặt lịch HairHub. Tóm tắt câu hỏi của khách hàng trên hệ thống Hairhub: {customerAsk}.

                            Yêu cầu:
                            1. Tóm tắt và phân tích câu hỏi của khách hàng, chỉ tập trung vào các chủ đề: lịch hẹn, dịch vụ tóc, salon, barber shop, khuyến mãi, hướng dẫn sử dụng Hairhub.
                            2. Nếu câu hỏi là tìm kiếm salon (ví dụ: “Hairhub có những salon nào?”), hãy trả lời bằng danh sách salon hiện có.
                            3. Bỏ qua mọi thông tin không liên quan đến các chủ đề trên.
                            4. Định dạng [Thời gian] phải là (dd/MM/yyyy HH:mm). Ngày hôm nay là: {DateTime.Now.Date.ToString("dd/MM/yyyy HH:mm")}.
                            5. Chuyển các từ ngữ thời gian tự nhiên như ""hôm nay"", ""ngày mai"", ""cuối tuần"" thành ngày và giờ cụ thể dựa trên ngày hiện tại.
                            6. Nếu câu hỏi không đề cập đến trạng thái lịch hẹn, mặc định [Trạng thái lịch hẹn] là ""đang đặt"". Nếu có các cụm như (lịch hẹn bỏ lỡ) hoặc (lịch hẹn thất bại), đặt [Trạng thái lịch hẹn] là ""thất bại"".
                            7. Nếu thiếu thông tin cần thiết, trả lời: null.
                            8. Trả lời ngắn gọn dưới dạng text, không dùng text box.

                            Cấu trúc câu trả lời phải tuân thủ chính xác định dạng sau (chỉ trả về 6 dòng, không thêm thông tin khác):

                            [Loại câu hỏi]: [kiểm tra lịch hẹn, tìm khuyến mãi, Hướng dẫn sử dụng Hairhub, tìm kiếm thời gian trống để đặt lịch, salon/barber shop/nhân viên, null];
                            [Loại hướng dẫn sử dụng]: [đặt lịch hẹn, hủy lịch hẹn, quy trình check in, xem lịch sử lịch hẹn, xem trạng thái lịch hẹn, null];
                            [Trạng thái lịch hẹn]: [hủy, đang đặt, thành công, thất bại, tất cả, null];
                            [Vị trí]: [Gần tôi, Địa điểm, null];
                            [Tên Salon hoặc tên Barber shop]: [tên salon, tên barber shop, null];
                            [Thời gian]: [Ngày và giờ cụ thể trong câu hỏi khách hàng theo đúng định dạng (dd/MM/yyyy HH:mm); nếu không có thì trả lời là null].";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = promptAsk}
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
            return apiResponse!.Candidates!.FirstOrDefault()!.Content!.Parts!.FirstOrDefault()!.Text;
        }

        private ClassificationResult ClassifyPrompt(string classificationText)
        {
            var clasifyAskCustomer = new ClassificationResult();
            if (classificationText != null)
            {
                var lines = classificationText.Split(new[] { '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries);
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
                            clasifyAskCustomer.Time = ConvertToDateTime(s);
                            break;
                        default:
                            break;
                    }
                }
            }
            return clasifyAskCustomer;
        }

        private async Task<string> kiemTraLichHen(string statusAppointment, DateTime? dateTime, Guid customerId)
        {
            string status = ""; 
            switch (statusAppointment)
            {
                case "hủy":
                    status = AppointmentStatus.CancelByCustomer;
                    break;
                case "đang đặt":
                    status = AppointmentStatus.Booking;
                    break;
                case "thành công":
                    status = AppointmentStatus.Successed;
                    break;
                case "thất bại":
                    status = AppointmentStatus.Fail;
                    break;
                case "tất cả":
                    status = "";
                    break;
                case null:
                    status = "";
                    //Lấy hết
                    break;
                default:
                    //thua, không có trạng thái sao mà kiểm tra???
                    break;
            }
            if (dateTime == null)
            {
                dateTime = DateTime.Now;
            }
            var appointments = await _appointmentService.GetAppointmentCustomerByStatus(customerId, status, true, dateTime, 1, 10);
            if (appointments.Items == null || !appointments.Items.Any())
            {
                return "Không có lịch hẹn nào cho thời gian và trạng thái được chỉ định.";
            }

            var appointmentsString = new StringBuilder();
            appointmentsString.AppendLine($"Danh sách {appointments.Items.Count} lịch hẹn gần nhất ngày {dateTime.Value.Date:dd/MM/yyyy}:");
            foreach (var appointment in appointments.Items)
            {
                appointmentsString.Append(
                    $"- Salon: {appointment.SalonInformation.Name}, ");

                var appointmentDetails = appointment.AppointmentDetails;
                appointmentsString.Append($"Dịch vụ: {appointment!.AppointmentDetails[0]!.ServiceName!}, ");
                appointmentsString.Append($"Thời gian: {appointment.AppointmentDetails[0].StartTime:dd/MM/yyyy HH:mm}, ");
                for (int i = 1; i<appointmentDetails.Count; i++) 
                {
                    appointmentsString.Append($"Dịch vụ: {appointment!.AppointmentDetails[i]!.ServiceName!}, ");
                    appointmentsString.Append($"Thời gian: {appointment.AppointmentDetails[i].StartTime:dd/MM/yyyy HH:mm}, ");
                }
                string statusInVietnamese = appointment.Status switch
                {
                    AppointmentStatus.Booking => "Đang Đặt",
                    AppointmentStatus.CancelByCustomer => "Hủy bởi Khách hàng",
                    AppointmentStatus.Successed => "Thành Công",
                    AppointmentStatus.Fail => "Thất Bại",
                    AppointmentStatus.Fake => "Giả Mạo",
                    _ => appointment.Status // Giữ nguyên nếu trạng thái không được định nghĩa trong switch
                };

                appointmentsString.AppendLine($"Trạng thái: {statusInVietnamese}\n");
            }
            return appointmentsString.ToString();
        }

        public async Task<string> ChatMessage(AIChatMessageRequest request)
        {
            request.AskMessage = request.AskMessage == null ? "" : request.AskMessage.Trim().ToLower();
            string classificationText = await CallGeminiAPI(request.AskMessage);
            var clasifyAskCustomer = ClassifyPrompt(classificationText);


            switch (clasifyAskCustomer.Intent)
            {
                case "kiểm tra lịch hẹn":
                    return await kiemTraLichHen(clasifyAskCustomer!.StatusAppointment!, clasifyAskCustomer.Time, request.CustomerId);
                case "tìm khuyến mãi":
                    break;
                case "hướng dẫn sử dụng Hairhub":
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
                case "tìm thời gian đặt lịch":
                    break;
                case "tìm salon hoặc barber shop":
                    break;
                case null:
                    return await SendMessageDefault(request.AskMessage);
                default:
                    //Tôi không biết bạn đang hỏi qq dì cả???
                    break;
            }
            return "";
        }
    }
}
