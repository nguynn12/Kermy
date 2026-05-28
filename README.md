# Kermy: The Elemental Leap

**Kermy** là một trò chơi hành động nền tảng (2D Action-Platformer) đầy kịch tính, nơi người chơi điều khiển những chú ếch dũng cảm trong hành trình giải cứu vương quốc đầm lầy. Game kết hợp khéo léo giữa kỹ năng di chuyển lắt léo, cơ chế chiến đấu khắc chế nguyên tố và hệ thống thay đổi ngày đêm độc đáo để vượt qua những thử thách khắc nghiệt.

## 🎮 Hình ảnh Game (Gameplay)
![Gameplay Screenshot](https://via.placeholder.com/800x450.png?text=Kermy+Gameplay+Action)
*Hình ảnh minh họa chú ếch chiến đấu với Boss trong môi trường thay đổi ngày đêm.*

## 🚀 Hướng dẫn Cài đặt và Chạy
1. **Yêu cầu:** Đã cài đặt [Unity Hub](https://unity.com/download) và **Unity Editor 6** (hoặc phiên bản 2022.3 LTS trở lên).
2. **Tải dự án:** Clone repository này hoặc giải nén thư mục dự án `Kermy`.
3. **Mở dự án:**
   - Mở Unity Hub -> Chọn `Add` -> Trỏ đến thư mục `Kermy`.
   - Chọn phiên bản Editor phù hợp để mở.
4. **Chạy Game:**
   - Tìm đến thư mục `Assets/Scenes`.
   - Mở Scene `MenuGame` hoặc `Level05`.
   - Nhấn nút **Play** (biểu tượng hình tam giác) ở giữa phía trên Editor.

## 👥 Danh sách Thành viên & Phân công
| Họ và Tên | Vai trò | Công việc cụ thể |
|-----------|---------|------------------|
| **Thành viên A** | Team Leader & Programmer | Phát triển hệ thống di chuyển (Coop), Logic Boss và Đạn. |
| **Thành viên B** | Gameplay Designer | Thiết kế cơ chế khắc chế nguyên tố (Lửa/Nước) và Day/Night Lever. |
| **Thành viên C** | UI/UX Artist | Thiết kế Giao diện Victory, Thanh máu Boss và hệ thống Star Rating. |
| **Thành viên D** | Sound & VFX Specialist | Quản lý hiệu ứng hạt (Dirt Particle), âm thanh chiến thắng và nhạc nền. |

## 🛠 Công nghệ sử dụng
- **Engine:** Unity 6 - Tận dụng tính năng `linearVelocity` mới cho vật lý mượt mà.
- **Ngôn ngữ:** C# Scripting.
- **Đồ họa:** 2D Sprite với Universal Render Pipeline (URP) cho hệ thống ánh sáng 2D (Light2D).
- **UI:** TextMesh Pro (TMP) cho hiển thị văn bản chất lượng cao.
- **Hệ thống Input:** Keybinding Manager tùy chỉnh hỗ trợ chơi đơn và co-op (2 người chơi).

## ✨ Tính năng nổi bật
- **Cơ chế Nguyên tố:** Đạn hệ Lửa gây sát thương gấp đôi Boss hệ Nước và ngược lại.
- **Hệ thống Ngày/Đêm:** Sử dụng đòn bẩy (Lever) để thay đổi môi trường, kích hoạt các lối đi bí mật hoặc vật thể ẩn.
- **Hệ thống Xếp hạng:** Đánh giá chiến thắng dựa trên số lượng Ngọc (Gems) thu thập được theo thang điểm 5 sao.
- **Hiệu ứng Vật lý:** Hạt bụi (Dirt Particles) và hiệu ứng mờ dần (Fade out) khi Boss bị tiêu diệt.

## 💻 Yêu cầu hệ thống
### Tối thiểu (Minimum):
- **OS:** Windows 10 64-bit.
- **CPU:** Intel Core i3 hoặc AMD tương đương.
- **Memory:** 4 GB RAM.
- **Graphics:** Card đồ họa tích hợp hỗ trợ DirectX 11.
- **Storage:** 500 MB chỗ trống khả dụng.

### Khuyến khích (Recommended):
- **OS:** Windows 11.
- **CPU:** Intel Core i5 trở lên.
- **Memory:** 8 GB RAM.
- **Graphics:** NVIDIA GeForce GTX 1050 hoặc tốt hơn để trải nghiệm ánh sáng URP tốt nhất.

---
*© 2024 LTG Demo - Kermy Project. Phát triển cho mục đích học tập và giải trí.*
