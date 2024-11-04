using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Enums
{
    public class GeminiAIStr
    {
        public string searchSalonGuide = @"Để tìm kiếm salon trên hệ thống HairHub, bạn hãy làm các bước sau:
                                    Bước 1: Ấn vào “Hệ thống cửa hàng” trên thanh công cụ.
                                    Bước 2: Tại trang tìm kiếm, bạn có thể tìm kiếm các salon theo “Dịch vụ” hoặc “Tên salon”.
                                    Bước 3: Nếu bạn muốn tìm kiếm các salon gần mình, hãy ấn vào “Nhấn vào đây để tìm kiếm salon gần bạn” ở góc trái trên cùng. Sau đó nhập khoảng cách và địa điểm của bạn (bạn cũng có thể chọn vị trí của bạn trên bản đồ).";
        public string bookingGuide = @"Để có thể đặt lịch hẹn trên HairHub, hãy làm các bước sau:
                                Bước 1: Tìm kiếm và chọn salon mong muốn trong danh sách hoặc trên trang chủ.
                                Bước 2: Ấn “Đặt” để có thể đặt dịch vụ tại salon trên HairHub.
                                Bước 3: Trong cửa sổ đặt lịch hẹn, chọn theo thứ tự ngày và giờ bạn muốn đặt lịch hẹn.
                                Bước 4: Chọn nhân viên để phục vụ cho bạn. Nếu không chọn, hệ thống sẽ mặc định là “Ngẫu nhiên”.
                                Bước 5: Bạn có thể thêm voucher hoặc các dịch vụ khác. Cuối cùng, bấm “Đặt lịch” để hoàn tất việc đặt lịch hẹn tại salon trên hệ thống HairHub.";
        public string cancelAppointmentGuide = @"Để có thể hủy lịch hẹn, hãy làm theo các bước sau:
                                        Bước 1: Đầu tiên bạn hãy vào “Cuộc hẹn”, chọn “Đang đặt”.
                                        Bước 2: Ấn “Hủy cuộc hẹn” để hủy cuộc hẹn đang đặt.
                                        Bước 3: Nhập lý do hủy cuộc hẹn.
                                        Lưu ý: Đừng hủy cuộc hẹn quá nhiều nhé.";
        public string checkInGuide = @"Để có thể check-in, hãy làm các bước sau:
                                Bước 1: Sau khi đặt lịch hẹn, bạn hãy đến salon và ấn vào biểu tượng QR Code trên thanh công cụ.
                                Bước 2: Bạn hãy nhờ salon đưa QR Code và quét thôi.
                                Lưu ý: Chỉ khi quét QR Code thành công thì trạng thái lịch hẹn mới được chuyển sang “Thành công”. Hãy nhớ quét QR Code để trạng thái cuộc hẹn thành công nhé.";

        string viewAppointmentStatusGuide = @"Để xem các trạng thái lịch hẹn, bạn hãy vào “Cuộc hẹn”. Tại đây chứa danh sách các cuộc hẹn của bạn với các trạng thái: Đang đặt, Đã hủy, Thất bại, Thành công.";

        public string DanhGia { get; set; } = "Để có thể đánh giá, đầu tiên bạn phải check-in để cuộc hẹn chuyển sang trạng thái thành công. Khi đánh giá, bạn sẽ được đánh giá theo từng dịch vụ trong lịch hẹn. Hãy gửi hình và bình luận để giúp salon có nhiều đánh giá tốt hơn nhé.";

        public string BaoCaoViPham { get; set; } = "Báo cáo vi phạm: Truy cập trang chi tiết của salon hoặc dịch vụ bạn muốn báo cáo. Bạn chỉ có thể báo cáo các cuộc hẹn thành công nhé. Sau đó hãy nhấn vào nút 'Báo cáo' trên trang. Điền thông tin chi tiết về vi phạm và lý do báo cáo. Nhấn 'Gửi báo cáo' để hoàn tất.";

        public string NapTienVaoVi { get; set; } = "Nạp tiền vào ví: Đăng nhập vào tài khoản HairHub. Vào mục 'Nạp tiền vào ví' trong avatar tài khoản. Nhấn 'Nạp tiền' và nhập số tiền muốn nạp. Chọn số tiền muốn nạp và ấn 'Nạp tiền ngay'. Sau đó quét Qr thanh toán để nạp tiền.";

        public string RutTienTuVi { get; set; } = "Để có thể rút tiền từ ví trên Hairhub, hãy vào trang cá nhân và chọn 'yêu cầu rút tiền' để tạo đơn rút tiền. Sau đó nhập các thông tin cần thiết và đợi Hairhub duyệt và chuyển tiền vào tài khoản của bạn. Yêu cầu rút tiền sẽ được duyệt và thanh toán trong vòng 72h sau khi gửi đơn rút tiền.";

        public string XemLichSuGiaoDich { get; set; } = "Chọn vào avatar và ấn vào 'lịch sử giao dịch' để xem tất cả các lịch sử giao dịch của bạn.";

        public string XoaTaiKhoan { get; set; } = "Để xóa tài khoản, hãy vào 'trang cá nhâ'. Sau đó, ấn vào 'xóa tài khoản'. Đọc các lưu ý khi xóa tài khoản và bấm 'đồng ý' nếu bạn muốn xóa tài khoản." ;

        public static string allGuidesForGemini = @"Hướng dẫn sử dụng HairHub:
            1. Tìm kiếm salon:
                - Để tìm kiếm salon trên hệ thống HairHub, bạn hãy làm các bước sau:
                    Bước 1: Ấn vào “Hệ thống cửa hàng” trên thanh công cụ.
                    Bước 2: Tại trang tìm kiếm, bạn có thể tìm kiếm các salon theo “Dịch vụ” hoặc “Tên salon”.
                    Bước 3: Nếu bạn muốn tìm kiếm các salon gần mình, hãy ấn vào “Nhấn vào đây để tìm kiếm salon gần bạn” ở góc trái trên cùng. Sau đó nhập khoảng cách và địa điểm của bạn (bạn cũng có thể chọn vị trí của bạn trên bản đồ).

            2. Đặt lịch hẹn:
                - Để có thể đặt lịch hẹn trên HairHub, hãy làm các bước sau:
                    Bước 1: Tìm kiếm và chọn salon mong muốn trong danh sách hoặc trên trang chủ.
                    Bước 2: Ấn “Đặt” để có thể đặt dịch vụ tại salon trên HairHub.
                    Bước 3: Trong cửa sổ đặt lịch hẹn, chọn theo thứ tự ngày và giờ bạn muốn đặt lịch hẹn.
                    Bước 4: Chọn nhân viên để phục vụ cho bạn. Nếu không chọn, hệ thống sẽ mặc định là “Ngẫu nhiên”.
                    Bước 5: Bạn có thể thêm voucher hoặc các dịch vụ khác. Cuối cùng, bấm “Đặt lịch” để hoàn tất việc đặt lịch hẹn tại salon trên hệ thống HairHub.

            3. Hủy lịch hẹn:
                - Để có thể hủy lịch hẹn, hãy làm theo các bước sau:
                    Bước 1: Đầu tiên bạn hãy vào “Cuộc hẹn”, chọn “Đang đặt”.
                    Bước 2: Ấn “Hủy cuộc hẹn” để hủy cuộc hẹn đang đặt.
                    Bước 3: Nhập lý do hủy cuộc hẹn.
                    Lưu ý: Đừng hủy cuộc hẹn quá nhiều nhé.

            4. Check-in:
                - Để có thể check-in, hãy làm các bước sau:
                    Bước 1: Sau khi đặt lịch hẹn, bạn hãy đến salon và ấn vào biểu tượng QR Code trên thanh công cụ.
                    Bước 2: Bạn hãy nhờ salon đưa QR Code và quét thôi.
                    Lưu ý: Chỉ khi quét QR Code thành công thì trạng thái lịch hẹn mới được chuyển sang “Thành công”. Hãy nhớ quét QR Code để trạng thái cuộc hẹn thành công nhé.

            5. Xem trạng thái lịch hẹn:
                - Để xem các trạng thái lịch hẹn, bạn hãy vào “Cuộc hẹn”. Tại đây chứa danh sách các cuộc hẹn của bạn với các trạng thái: Đang đặt, Đã hủy, Thất bại, Thành công.

            6. Đánh giá:
                - Để có thể đánh giá, đầu tiên bạn phải check-in để cuộc hẹn chuyển sang trạng thái thành công. Khi đánh giá, bạn sẽ được đánh giá theo từng dịch vụ trong lịch hẹn. Hãy gửi hình và bình luận để giúp salon có nhiều đánh giá tốt hơn nhé.

            7. Báo cáo vi phạm:
                - Báo cáo vi phạm: Truy cập trang chi tiết của salon hoặc dịch vụ bạn muốn báo cáo. Bạn chỉ có thể báo cáo các cuộc hẹn thành công nhé. Sau đó hãy nhấn vào nút 'Báo cáo' trên trang. Điền thông tin chi tiết về vi phạm và lý do báo cáo. Nhấn 'Gửi báo cáo' để hoàn tất.

            8. Nạp tiền vào ví:
                - Nạp tiền vào ví: Đăng nhập vào tài khoản HairHub. Vào mục 'Nạp tiền vào ví' trong avatar tài khoản. Nhấn 'Nạp tiền' và nhập số tiền muốn nạp. Chọn số tiền muốn nạp và ấn 'Nạp tiền ngay'. Sau đó quét QR để thanh toán và nạp tiền.

            9. Rút tiền từ ví:
                - Để có thể rút tiền từ ví trên Hairhub, hãy vào trang cá nhân và chọn 'yêu cầu rút tiền' để tạo đơn rút tiền. Sau đó nhập các thông tin cần thiết và đợi Hairhub duyệt và chuyển tiền vào tài khoản của bạn. Yêu cầu rút tiền sẽ được duyệt và thanh toán trong vòng 72h sau khi gửi đơn rút tiền.

            10. Xem lịch sử giao dịch:
                - Chọn vào avatar và ấn vào 'lịch sử giao dịch' để xem tất cả các lịch sử giao dịch của bạn.

            11. Xóa tài khoản:
                - Để xóa tài khoản, hãy vào 'trang cá nhân'. Sau đó, ấn vào 'xóa tài khoản'. Đọc các lưu ý khi xóa tài khoản và bấm 'đồng ý' nếu bạn muốn xóa tài khoản.";
    }
}
