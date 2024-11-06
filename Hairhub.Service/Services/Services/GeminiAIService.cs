
using Hairhub.Domain.Dtos.Requests.AI;
using Hairhub.Domain.Dtos.Responses.AI;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Enums;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data.Entity;
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
            var promptStr = $@"Bạn là ChatBot Hairhub, trợ lý hỗ trợ khách hàng cho ứng dụng đặt lịch HairHub.. Hôm nay là {DateTime.Now.ToString("dd/MM/yyyy HH:mm")} - {DateTime.Now.DayOfWeek}. Câu hỏi của người dùng là: ""{askCustomer}"".

                            Vui lòng phản hồi bằng phong cách hài hước nhưng chuyên nghiệp và ngắn gọn. Chỉ trả lời duy nhất bằng tiếng Việt. Nếu câu hỏi nằm ngoài hệ thống, hướng dẫn khách hàng đặt câu hỏi rõ ràng hơn theo các chủ đề sau:
                            1. Kiểm tra lịch hẹn.
                            2. Tìm khuyến mãi hiện có.
                            3. Hướng dẫn sử dụng HairHub.
                            4. Tìm thời gian đặt lịch phù hợp.
                            5. Tìm salon hoặc barber shop gần bạn.";
                           // Lưu ý: Bắt đầu phản hồi với lời chào mừng đến HairHub và tự giới thiệu bản thân là Chatbot HairHub. Chatbot hân hạnh được hỗ trợ khách hàng.";
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

        private async Task<string> CallGeminiAPI(string prompt)
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
                            new { text = prompt}
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

        private async Task<string> FindVoucher()
        {
            var vouchers = await _unitOfWork.GetRepository<Voucher>().GetListAsync(predicate: x=>x.IsActive && x.ExpiryDate>=DateTime.Now);

            if (vouchers == null || !vouchers.Any())
            {
                return "Hiện tại không có voucher nào.";
            }
            var voucherString = new StringBuilder();
            voucherString.AppendLine("Hairhub có 2 loại voucher: voucher của hệ thống (áp dụng cho tất cả khách hàng sử dụng Hairhub), voucher của salon (chỉ áp dụng cho khách hàng đặt lịch tại salon này)." +
                                     "Hãy giới thiệu cho khách hàng tất cả những voucher mà họ có thể sử dụng.");
            voucherString.AppendLine("- Danh sách voucher hệ thống (áp dụng cho tất cả khách hàng đặt lịch trên Hairhub):");
            var voucherSystem = vouchers.Where(v => v.IsSystemCreated).ToList();
            if(voucherSystem!=null && voucherSystem!.Count != 0)
            {
                foreach (var voucher in voucherSystem) 
                {
                    //voucherString.Append($"   + Mã voucher: {voucher.Code}; ");
                    voucherString.Append($"   + Mô tả: {voucher.Description}; ");
                    voucherString.Append($"Số tiền đặt hàng tối thiểu: {voucher.MinimumOrderAmount:C}; ");
                    voucherString.Append($"Phần trăm giảm giá: {voucher.DiscountPercentage}%; ");
                    voucherString.Append($"Giảm giá tối đa: {voucher.MaximumDiscount:C}; ");
                    voucherString.Append($"Số lượng khả dụng: {voucher.Quantity}; ");
                    voucherString.Append($"Ngày bắt đầu: {voucher.StartDate:dd/MM/yyyy}; ");
                    voucherString.Append($"Ngày hết hạn: {voucher.ExpiryDate:dd/MM/yyyy}; ");
                    voucherString.Append($"Trạng thái: {(voucher.IsActive ? "Đang hoạt động" : "Không hoạt động")}\n");
                    voucherString.AppendLine();
                }
            }
            else
            {
                voucherString.AppendLine("Không có voucher của hệ thống.");
            }

            var voucherSalon = vouchers.Where(x => !x.IsSystemCreated && x.SalonInformationId != null)
                                           .GroupBy(v => v.SalonInformationId)
                                           .ToList();
            voucherString.AppendLine("- Danh sách voucher của salon (áp dụng cho khách hàng đặt lịch tại salon này):");
            if (vouchers!=null && voucherSalon.Count!=0)
            {

                foreach (var salonGroup in voucherSalon)
                {
                    var salon = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: x=>x.Id == salonGroup.Key);
                    voucherString.AppendLine($"  - Voucher của {salon.Name} (chỉ áp dụng khi đặt lịch tại salon {salon.Name})");

                    foreach (var voucher in salonGroup)
                    {
                        //voucherString.Append($"     + Mã voucher: {voucher.Code}; ");
                        voucherString.Append($"     + Mô tả: {voucher.Description}; ");
                        voucherString.Append($"Số tiền đặt hàng tối thiểu: {voucher.MinimumOrderAmount:C}; ");
                        voucherString.Append($"Phần trăm giảm giá: {voucher.DiscountPercentage}%; ");
                        voucherString.Append($"Giảm giá tối đa: {voucher.MaximumDiscount:C} VNĐ; ");
                        voucherString.Append($"Số lượng khả dụng: {voucher.Quantity}; ");
                        voucherString.Append($"Ngày bắt đầu: {voucher.StartDate:dd/MM/yyyy}; ");
                        voucherString.Append($"Ngày hết hạn: {voucher.ExpiryDate:dd/MM/yyyy}; ");
                        voucherString.AppendLine($"Trạng thái: {(voucher.IsActive ? "Đang hoạt động" : "Không hoạt động")}");
                    }
                    voucherString.AppendLine(); 
                }
            }
            else
            {
                voucherString.AppendLine("Không có voucher của salon.");
            }
            return voucherString.ToString();
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
                    status = AppointmentStatus.Booking;
                    //Lấy hết
                    break;
                default:
                    status = "";
                    break;
            }

            var appointments = await _appointmentService.GetAppointmentGemini(customerId, status, dateTime);
            if (appointments == null || appointments.Count == 0)
            {
                return "Không có lịch hẹn nào cho thời gian và trạng thái được chỉ định.";
            }

            if (appointments == null || appointments.Count == 0)
            {
                return "Không có lịch hẹn nào cho thời gian và trạng thái được chỉ định.";
            }

            var appointmentsString = new StringBuilder();
            appointmentsString.AppendLine($"Danh sách {appointments.Count} lịch hẹn:");

            int appointmentIndex = 1;
            foreach (var appointment in appointments)
            {
                appointmentsString.Append($"{appointmentIndex}. ");
                appointmentsString.Append($"Salon: {appointment.SalonInformation.Name}; ");
                appointmentsString.Append($"địa chỉ: {appointment.SalonInformation.Address}; ");
                appointmentsString.Append($"ngày hẹn: {appointment.StartDate:dd/MM/yyyy}; ");
                appointmentsString.Append($"giá tiền: {appointment.TotalPrice} VNĐ; ");

                string statusInVietnamese = appointment.Status switch
                {
                    AppointmentStatus.Booking => "Đang Đặt",
                    AppointmentStatus.CancelByCustomer => "Đã Hủy",
                    AppointmentStatus.Successed => "Thành Công",
                    AppointmentStatus.Fail => "Thất Bại",
                    AppointmentStatus.Fake => "",
                    _ => appointment.Status
                };

                appointmentsString.AppendLine($"trạng thái: {statusInVietnamese};");
                appointmentsString.AppendLine("    Chi tiết cuộc hẹn:");

                foreach (var detail in appointment.AppointmentDetails)
                {
                    appointmentsString.AppendLine($"  + Tên dịch vụ: {detail.ServiceName}; tên nhân viên: {detail.SalonEmployee.FullName}; thời gian bắt đầu: {detail.StartTime:dd/MM/yyyy HH:mm}; thời gian kết thúc: {detail.EndTime:dd/MM/yyyy HH:mm}");
                }
                appointmentIndex++;
                appointmentsString.AppendLine();
            }
            return appointmentsString.ToString();
        }

        private async Task<string> FindSalon(string? salonName)
        {
            if (salonName == null)
            {
                salonName = "";
            }
            else
            {
                salonName = salonName.Trim();
            }
            var salons = await _unitOfWork.GetRepository<SalonInformation>().GetListAsync(predicate: x=>x.Name.Contains(salonName) && x.Status.Equals(SalonStatus.Approved));
            var infoString = new StringBuilder();

            foreach(var salon in salons)
            {
                infoString.AppendLine($"- Thông tin salon: {salon.Name}; Địa chỉ: {salon.Address}; Đánh giá: {salon.Rate} / 5 ({salon.TotalReviewer} đánh giá); Trạng thái: {salon.Status}");
                infoString.AppendLine($"  + Lịch làm việc của {salon.Name}:");
                //schedule cua Salon
                var schedules = await _unitOfWork.GetRepository<Schedule>().GetListAsync(predicate: x => x.SalonId == salon.Id);
                foreach (var schedule in schedules)
                {
                    infoString.AppendLine($"    - Ngày: {schedule.DayOfWeek}, Bắt đầu: {schedule.StartTime}, Kết thúc: {schedule.EndTime}, Trạng thái: {(schedule.IsActive ? "Đang hoạt động" : "Không hoạt động")}");
                }
                //Service cua Salon
                infoString.AppendLine("  + Dịch vụ của salon:");
                var services = await _unitOfWork.GetRepository<ServiceHair>().GetListAsync(predicate: x=>x.IsActive && x.SalonInformationId == salon.Id);
                foreach (var service in services)
                {
                    infoString.AppendLine($"    - Tên dịch vụ: {service.ServiceName}; Giá: {(int)service.Price} VNĐ; Thời gian: {(int)(service.Time*60)} phút");
                }
                // Nhân viên của salon
                infoString.AppendLine("  + Nhân viên của salon:");
                var employees = await _unitOfWork.GetRepository<SalonEmployee>().GetListAsync(predicate: x=>x.SalonInformationId == salon.Id && x.IsActive);
                foreach (var employee in employees)
                {
                    infoString.AppendLine($"    - Tên: {employee.FullName}; Giới tính: {employee.Gender}; Đánh giá: {employee.Rating} / 5 ({employee.RatingCount} đánh giá)");

                    // Thêm lịch làm việc của nhân viên
                    var employeeSchedules = await _unitOfWork.GetRepository<Schedule>().GetListAsync(predicate: x=>x.EmployeeId == employee.Id);
                    if (employeeSchedules.Any())
                    {
                        infoString.AppendLine("      Thời gian làm việc:");
                        foreach (var empSchedule in employeeSchedules)
                        {
                            infoString.AppendLine($"        + Ngày: {empSchedule.DayOfWeek}, Bắt đầu: {empSchedule.StartTime}, Kết thúc: {empSchedule.EndTime}");
                        }
                    }
                    else
                    {
                        infoString.AppendLine("      Thời gian làm việc: Không có lịch làm việc");
                    }
                    infoString.Append($"      Dịch vụ mà nhân viên {employee.FullName} thực hiện: ");
                    var employeeServices = await _unitOfWork.GetRepository<ServiceHair>()
                                                            .GetListAsync(
                                                                predicate: x => x.ServiceEmployees.Any(s=>s.SalonEmployeeId == employee.Id) && x.IsActive
                                                            );

                    if (employeeServices.Any())
                    {
                        foreach(var service in employeeServices)
                        {
                            infoString.Append($"{service.ServiceName}, ");
                        }
                        infoString.AppendLine();
                    }
                    else
                    {
                        infoString.AppendLine("Không có dịch vụ nào.");
                    }
                }
            }
            return infoString.ToString();
        }

        private async Task<Customer?> GetCustomerInfo(Guid customerId)
        {
            var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x=>x.Id == customerId);
            return customer;
        }
        public async Task<string> ChatMessage(AIChatMessageRequest request)
        {
            string customerAsk = request.AskMessage == null ? "" : request.AskMessage.Trim().ToLower();
            string preQuestion = request.PreQuesion == null ? "Không có" : request!.PreQuesion.Trim();
            var promptAsk = $@"Bạn là Hairhub Chatbot, một trợ lý hỗ trợ khách hàng cho ứng dụng đặt lịch HairHub. 
                                Câu hỏi trước đó của khách hàng: ""{preQuestion}""
                                Câu hỏi hiện tại của khách hàng: ""{customerAsk}""
                            Yêu cầu:
                            1. Tóm tắt và phân tích câu hỏi của khách hàng, chỉ tập trung vào các chủ đề: lịch hẹn, dịch vụ tóc, salon, barber shop, khuyến mãi, hướng dẫn sử dụng Hairhub.
                            2. Nếu câu hỏi là tìm kiếm salon (ví dụ: “Hairhub có những salon nào?”), hãy trả lời bằng danh sách salon hiện có.
                            3. Bỏ qua mọi thông tin không liên quan đến các chủ đề trên.
                            4. Định dạng [Thời gian] phải là (dd/MM/yyyy HH:mm). Ngày hôm nay là: {DateTime.Now.Date.ToString("dd/MM/yyyy HH:mm")} - {DateTime.Now.DayOfWeek}.
                            5. Chuyển các từ ngữ thời gian tự nhiên như ""hôm nay"", ""ngày mai"", ""cuối tuần"" thành ngày và giờ cụ thể dựa trên ngày hiện tại.
                            6. Nếu câu hỏi không tìm thấy trạng thái lịch hẹn, đặt [Trạng thái lịch hẹn] là ""đang đặt"". Nếu có các cụm như (lịch hẹn bỏ lỡ) hoặc (lịch hẹn thất bại), đặt [Trạng thái lịch hẹn] là ""thất bại"".
                            7. Nếu thiếu thông tin cần thiết, trả lời: null.
                            8. Trả lời ngắn gọn dưới dạng text, không dùng text box.
                            9. Tên [Tên Salon hoặc tên Barber shop] cần lược bỏ các từ như (salon), (barber shop), (tiệm tóc), (tiệm cắt tóc).
                            10. Mọi câu hỏi về nhân viên, salon, hoặc barber shop đều phải được phân loại là `salon/barber shop/nhân viên` trong mục [Loại câu hỏi].

                            Hãy tóm tắt câu hỏi của khách hàng và trả lời theo tuân thủ chính xác định dạng 6 dòng sau (chỉ trả về 6 dòng, không thêm thông tin khác):
                            [Loại câu hỏi]: [kiểm tra lịch hẹn, tìm khuyến mãi, hướng dẫn sử dụng Hairhub, tìm kiếm thời gian trống để đặt lịch, salon/barber shop/nhân viên, null];
                            [Loại hướng dẫn sử dụng]: [đặt lịch hẹn, hủy lịch hẹn, quy trình check in, xem lịch sử lịch hẹn, xem trạng thái lịch hẹn, null];
                            [Trạng thái lịch hẹn]: [hủy, đang đặt, thành công, thất bại, tất cả, null];
                            [Vị trí]: [Gần tôi, Địa điểm, null];
                            [Tên Salon hoặc tên Barber shop]: [tên salon, tên barber shop, null];
                            [Thời gian]: [Ngày và giờ cụ thể trong câu hỏi khách hàng theo đúng định dạng (dd/MM/yyyy HH:mm); nếu không có thì trả lời là null].";
            string classificationText = await CallGeminiAPI(promptAsk);
            var clasifyAskCustomer = ClassifyPrompt(classificationText);

            string dataHairhub = "";
            switch (clasifyAskCustomer.Intent)
            {
                case "kiểm tra lịch hẹn":
                    dataHairhub = await kiemTraLichHen(clasifyAskCustomer!.StatusAppointment!, clasifyAskCustomer.Time, request.CustomerId);
                    break;
                case "tìm khuyến mãi":
                    dataHairhub = await FindVoucher();
                    break;
                case "hướng dẫn sử dụng Hairhub":
                    dataHairhub = GeminiAIStr.allGuidesForGemini;
                    break;
                case "tìm kiếm thời gian trống để đặt lịch":
                    break;
                case "salon/barber shop/nhân viên":
                    dataHairhub = await FindSalon(clasifyAskCustomer.SalonName);
                    break;
                case null:
                    return await SendMessageDefault(request.AskMessage);
                default:
                    //Tôi không biết bạn đang hỏi qq dì cả???
                    break;
            }
            string customerPrompt = "";
            var customerInfo = await GetCustomerInfo(request.CustomerId);
            if (customerInfo == null)
            {
                customerPrompt = "Không có thông tin.";
            }
            else
            {
                customerPrompt = $@"Tên: {customerInfo.FullName ?? "không có"}, giới tính: {customerInfo.Gender ?? "không có"}, ngày sinh: {customerInfo.DayOfBirth?.ToString("dd/MM/yyyy") ?? "không có"}, địa chỉ: {customerInfo.Address ?? "không có"}.";
            }
            var promptAnswer = $@"Bạn là ChatBot Hairhub, trợ lý hỗ trợ khách hàng cho ứng dụng đặt lịch HairHub. Hãy trả lời câu hỏi của khách hàng dựa trên dữ liệu có sẵn từ hệ thống HairHub.
                                Thông tin của khách hàng: {customerPrompt}
                                Lưu ý:
                                1. Ngày hiện tại là: {DateTime.Now.ToString("dd/MM/yyyy HH:mm")} - {DateTime.Now.DayOfWeek}.
                                2. Nếu câu hỏi của khách hàng là về “lịch hẹn sắp tới” hoặc “đang có lịch hẹn nào không”, kiểm tra các lịch hẹn có trạng thái “đang đặt”.
                                3. Nếu câu hỏi của khách hàng là về “lịch hẹn bỏ lỡ” hoặc “lịch hẹn thất bại”, kiểm tra các lịch hẹn có trạng thái “thất bại”.
                                4. Chỉ trả lời dựa trên dữ liệu đã cung cấp. Không thêm thông tin ngoài dữ liệu này.
                                5. Nếu khách hàng tìm salon/barber shop để đặt lịch thì hãy gợi ý những salon/barber shop có số lượt đánh giá cao.
                                6. Phản hồi bằng phong cách hài hước nhưng chuyên nghiệp.
                                7. Chỉ trả lời duy nhất bằng tiếng Việt.
                                
                                Câu hỏi trước đó của khách hàng: ""{preQuestion}""
                                Câu hỏi hiện tại của khách hàng: ""{customerAsk}""
                                Dữ liệu từ HairHub: ""{dataHairhub}""
                                ";
            string result = await CallGeminiAPI(promptAnswer);
            return result;
        }
    }
}
