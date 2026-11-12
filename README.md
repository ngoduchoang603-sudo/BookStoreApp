# 📚 BookStoreApp - Website Bán Sách Trực Tuyến (ASP.NET MVC)

Hệ thống e-commerce bán sách trực tuyến hoàn chỉnh được xây dựng trên nền tảng **ASP.NET Web Application (.NET Framework)** kết hợp với **Entity Framework (Database First)** và **SQL Server**. Dự án tích hợp đầy đủ tính năng dành cho Khách hàng và trang Quản trị (Admin Dashboard).

---

## 🚀 Công Nghệ Sử Dụng (Tech Stack)

* **Backend:** C# ASP.NET MVC 5, .NET Framework 4.8
* **Database:** SQL Server, Entity Framework 6 (Database First)
* **Frontend:** HTML5, CSS3, JavaScript, Bootstrap 5, FontAwesome 6
* **Authentication:** Forms Authentication + Hashing password (SHA-256)
* **Payment Gateways:**
  * COD (Thanh toán khi nhận hàng)
  * Chuyển khoản Ngân hàng tự động với mã VietQR (MBBank API)
  * Ví điện tử MoMo
  * Cổng thanh toán VNPay Sandbox

---

## ✨ Tính Năng Nổi Bật

### 🛒 Dành cho Khách hàng (Client Portal):
* **Trang chủ & Danh mục:** Giao diện hiện đại màu cam rực rỡ, tích hợp Slider banner, bộ lọc sách theo danh mục và thanh tìm kiếm từ khóa.
* **Chi tiết sản phẩm:** Xem thông tin sách, tác giả, mô tả và cụm nút tùy chọn **"Thêm vào giỏ hàng"** & **"Mua ngay"**.
* **Giỏ hàng & Thanh toán:** Quản lý số lượng sách, tính tổng tiền tự động và linh hoạt chọn 4 phương thức thanh toán (COD, VietQR, MoMo, VNPay).
* **Tài khoản & Đơn hàng:** Đăng ký, đăng nhập bảo mật và xem lại lịch sử các đơn hàng đã đặt.

### 🛡️ Dành cho Quản trị viên (Admin Area):
* **Dashboard Thống kê:** Báo cáo tổng doanh thu, tổng số đơn hàng, tổng đầu sách và số lượng khách hàng.
* **Quản lý Sách (CRUD):** Thêm sách mới, chỉnh sửa giá/số lượng tồn kho, cập nhật ảnh bìa và xóa sản phẩm.
* **Quản lý Đơn hàng:** Xem chi tiết đơn hàng, danh sách sách khách mua và cập nhật trạng thái giao hàng.
* **Quản lý Danh mục:** Thêm mới và quản lý các thể loại sách trong hệ thống.
* **Quản lý Người dùng:** Theo dõi danh sách tài khoản khách hàng và phân quyền hệ thống.

---

## 🛠️ Hướng Dẫn Cài Đặt & Chạy Dự Án (Getting Started)

### Yêu cầu hệ thống:
* Visual Studio 2019/2022 (đã cài gói web development)
* SQL Server Management Studio (SSMS)
* .NET Framework 4.8 trở lên

### Các bước khởi chạy:

1. **Clone repository về máy:**
   ```bash
   git clone [https://github.com/ngoduchoang603-sudo/BookStoreApp.git](https://github.com/ngoduchoang603-sudo/BookStoreApp.git)
2. Cấu hình Cơ sở dữ liệu (Database):
   Mở SQL Server Management Studio (SSMS).
   Tạo cơ sở dữ liệu mới với tên BookStoreDB.
   Chạy script tạo các bảng: Users, Categories, Books, Orders, OrderDetails.
3. Cập nhật Chuỗi kết nối (Connection String)
   Mở solution BookStoreApp.sln trong Visual Studio.
   Mở file Web.config và chỉnh sửa lại thuộc tính connectionString sao cho khớp với tên Server SQL trên máy của bạn:
   ```xml
   <add name="BookStoreDBEntities" connectionString="metadata=res://*/Models.BookStoreModel.csdl|...;provider connection string=&quot;data source=YOUR_SERVER_NAME;initial catalog=BookStoreDB;integrated security=True;...&quot;" providerName="System.Data.EntityClient" />
5. Chạy ứng dụng:
   Nhấn Ctrl + Shift + B để Rebuild Solution.
   Nhấn F5 (hoặc bấm Play IIS Express) để mở trang web.   
