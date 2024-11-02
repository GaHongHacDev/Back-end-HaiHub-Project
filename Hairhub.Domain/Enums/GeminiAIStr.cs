using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Enums
{
    public class GeminiAIStr
    {
        public string TimKiemSalon { get; set; } = "Tìm kiếm salon: Truy cập trang chủ HairHub. Tìm thanh tìm kiếm ở đầu trang. Nhập tên salon, dịch vụ hoặc địa điểm mong muốn. Nhấn vào nút 'Tìm kiếm'. Xem danh sách các salon hiện ra theo kết quả tìm kiếm của bạn.";

        public string DatLichHen { get; set; } = "Đặt lịch hẹn: Đăng nhập vào tài khoản HairHub. Tìm kiếm và chọn salon mong muốn trong danh sách hoặc trang chủ. Chọn dịch vụ bạn cần và thời gian muốn đặt lịch. Nhấn nút 'Đặt lịch hẹn' hoặc 'Xác nhận'. Kiểm tra lại thông tin và nhấn 'Hoàn tất' để xác nhận đặt lịch hẹn.";

        public string HuyLichHen { get; set; } = "Hủy lịch hẹn: Đăng nhập vào tài khoản HairHub. Vào mục 'Lịch hẹn của tôi' trong tài khoản của bạn. Tìm lịch hẹn mà bạn muốn hủy. Nhấn vào lịch hẹn đó, sau đó nhấn 'Hủy lịch hẹn'. Xác nhận việc hủy lịch hẹn khi hệ thống yêu cầu.";

        public string QuyTrinhCheckIn { get; set; } = "Quy trình check-in: Đến salon theo đúng lịch hẹn đã đặt trên hệ thống. Tại quầy lễ tân của salon, thông báo với nhân viên về lịch hẹn của bạn. Nhân viên sẽ kiểm tra lịch hẹn và hướng dẫn bạn đến khu vực chờ hoặc khu vực làm dịch vụ.";

        public string XemTrangThaiLichHen { get; set; } = "Xem trạng thái lịch hẹn: Đăng nhập vào tài khoản HairHub. Vào mục 'Lịch hẹn của tôi'. Xem trạng thái của từng lịch hẹn, bao gồm các trạng thái như: 'Đã đặt', 'Đã hoàn thành', 'Đã hủy', v.v.";

        public string DanhGia { get; set; } = "Đánh giá: Đăng nhập vào tài khoản và vào 'Lịch hẹn của tôi' sau khi đã hoàn thành dịch vụ. Tìm lịch hẹn mà bạn muốn đánh giá. Nhấn vào 'Đánh giá', nhập nhận xét và chấm điểm theo mức độ hài lòng. Nhấn 'Gửi' để hoàn tất đánh giá của bạn.";

        public string BaoCaoViPham { get; set; } = "Báo cáo vi phạm: Truy cập trang chi tiết của salon hoặc dịch vụ bạn muốn báo cáo. Tìm và nhấn vào nút 'Báo cáo vi phạm' trên trang. Điền thông tin chi tiết về vi phạm và lý do báo cáo. Nhấn 'Gửi báo cáo' để hoàn tất.";

        public string NapTienVaoVi { get; set; } = "Nạp tiền vào ví: Đăng nhập vào tài khoản HairHub. Vào mục 'Ví của tôi' trong trang tài khoản. Nhấn 'Nạp tiền' và nhập số tiền muốn nạp. Chọn phương thức thanh toán phù hợp và nhấn 'Xác nhận'. Làm theo các hướng dẫn thanh toán để hoàn tất nạp tiền.";

        public string RutTienTuVi { get; set; } = "Rút tiền từ ví: Đăng nhập vào tài khoản HairHub. Vào mục 'Ví của tôi'. Nhấn vào 'Rút tiền' và nhập số tiền bạn muốn rút. Xác nhận thông tin tài khoản ngân hàng nhận tiền và nhấn 'Xác nhận'. Chờ hệ thống xử lý rút tiền.";

        public string XemLichSuGiaoDich { get; set; } = "Xem lịch sử giao dịch: Đăng nhập vào tài khoản HairHub. Vào mục 'Ví của tôi'. Tìm và nhấn vào 'Lịch sử giao dịch'. Xem chi tiết các giao dịch đã thực hiện, bao gồm nạp tiền, rút tiền và thanh toán dịch vụ.";

        public string XoaTaiKhoan { get; set; } = "Xóa tài khoản: Đăng nhập vào tài khoản HairHub. Vào mục 'Cài đặt tài khoản'. Nhấn vào 'Xóa tài khoản'. Xác nhận quyết định xóa tài khoản khi hệ thống yêu cầu.";
    }
}
