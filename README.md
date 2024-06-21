# WebAPI
Website bán thức ăn nhanh sử dụng webAPI và gọi API với MVC
1. Giới thiệu
Chào mừng bạn đến với dự án Pfood. Đây là một ứng dụng web phát triển bằng ASP.NET Core. File hướng dẫn này sẽ giúp bạn thiết lập và chạy ứng dụng trên máy tính của bạn.
2. Yêu cầu hệ thống
Để chạy ứng dụng này, bạn cần có các công cụ và phần mềm sau:
•	.NET SDK: Phiên bản 8.0 hoặc các phiên bản khác
•	Visual Studio: Phiên bản 2022, các phiên bản khác hoặc Visual Studio Code
•	SQL Server
3. Cấu hình dự án
Dự án bao gồm 2 project là ASP.NET CORE MVC và ASP.NET CORE API
+ Project API: Chứa các API controller, models, services…
+ Project MVC: Chứa các MVC controller, views, models…
Bước 1: Mở project trong Visual Studio 
Bước 2: Cấu hình “appsetting.json”
 + Project API:
-	Cấu hình chuỗi kết nối phù hợp với Sql Server của bạn trong đối tượng ConnectionStrings.
-	Cấu hình các thuộc tính của “Smtp” như Username, Password (password là mật khẩu ứng dụng của bạn), FromEmail…
-	Cấu hình các thuộc tính “Jwt” tùy chỉnh.
+ Project MVC:
-	Cấu hình chuỗi kết nối phù hợp với Sql Server của bạn trong đối tượng ConnectionStrings.
-	Đăng kí tài khoản merchant trên môi trường test qua link https://sandbox.vnpayment.vn/devreg
-	Kiểm tra mail đăng kí được gửi về hộp thư có các thông tin như TmnCode, SecretKey
-	Cấu hình các thông tin của “VnPay” tương ứng với thông tin trên.
Bước 3: Tạo database với EF
+ Mở Package Manager Console
+ Chọn project để migration là PFood
+ Chạy lệnh: add-migration “Tên migration”
+ Chạy lệnh update-database để tạo cơ sở dữ liệu
4. Chạy ứng dụng
4.1. Thiết lập Startup Project
•	Nhấp chuột phải vào Solution chọn Configure Startup Projects
•	Trong Startup Project chọn “Multiple startup projects” 
•	Trong action của 2 project chọn Start hoặc Start without debugging và nhấn ok
4.2. Chạy ứng dụng
•	Nhấn vào Nút Start hoặc nhấn F5…
•	Tài khoản Admin mặc định là admin@gmail.com password: Admin@1234
5. Thông tin liên hệ
Nếu bạn gặp vấn đề hoặc có câu hỏi, vui lòng liên hệ với tôi qua email nguyenphuc14112003@gmail.com hoặc zalo: 0898827656.
