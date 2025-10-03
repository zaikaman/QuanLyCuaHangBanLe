# 🏪 Hệ Thống Quản Lý Cửa Hàng Bán Lẻ

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=for-the-badge&logo=dotnet)](https://docs.microsoft.com/en-us/aspnet/core/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?style=for-the-badge&logo=mysql&logoColor=white)](https://www.mysql.com/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?style=for-the-badge)](https://docs.microsoft.com/en-us/ef/core/)

Hệ thống quản lý cửa hàng bán lẻ toàn diện được xây dựng bằng ASP.NET Core MVC, cung cấp giải pháp quản lý hiện đại cho các cửa hàng vừa và nhỏ.

---

## 📋 Mục Lục

- [Tính Năng Chính](#-tính-năng-chính)
- [Công Nghệ Sử Dụng](#-công-nghệ-sử-dụng)
- [Yêu Cầu Hệ Thống](#-yêu-cầu-hệ-thống)
- [Hướng Dẫn Cài Đặt](#-hướng-dẫn-cài-đặt)
- [Cấu Hình Database](#-cấu-hình-database)
- [Chạy Ứng Dụng](#-chạy-ứng-dụng)
- [Cấu Trúc Dự Án](#-cấu-trúc-dự-án)
- [Tài Khoản Mặc Định](#-tài-khoản-mặc-định)
- [Hướng Dẫn Sử Dụng](#-hướng-dẫn-sử-dụng)
- [API Endpoints](#-api-endpoints)
- [Troubleshooting](#-troubleshooting)
- [Đóng Góp](#-đóng-góp)
- [License](#-license)

---

## ✨ Tính Năng Chính

### 🔐 Xác Thực & Phân Quyền
- Đăng nhập session-based an toàn
- Quản lý người dùng với roles khác nhau
- Bảo vệ routes với authentication middleware

### 📦 Quản Lý Sản Phẩm
- ✅ CRUD đầy đủ cho sản phẩm
- 🔍 Tìm kiếm real-time
- 📄 Phân trang thông minh (10 items/trang)
- 📊 Xem chi tiết sản phẩm với thông tin đầy đủ
- 🏷️ Phân loại theo danh mục
- 🔖 Quét mã vạch (barcode)

### 👥 Quản Lý Khách Hàng
- 📝 Thêm, sửa, xóa thông tin khách hàng
- 📞 Lưu trữ thông tin liên hệ đầy đủ
- 🔍 Tìm kiếm nhanh theo tên, SĐT, email
- 📄 Xem chi tiết khách hàng và lịch sử

### 🛒 Quản Lý Đơn Hàng
- 🧾 Tạo đơn hàng với nhiều sản phẩm
- 💰 Tính toán tự động tổng tiền, giảm giá
- 📊 Theo dõi trạng thái: Đã thanh toán, Chờ xử lý, Đã hủy
- 🔍 Lọc đơn hàng theo trạng thái
- 👁️ Xem chi tiết đơn hàng với thông tin đầy đủ

### 🎁 Quản Lý Khuyến Mãi
- 🎯 Tạo mã giảm giá linh hoạt
- 💯 Hỗ trợ giảm giá theo % hoặc số tiền cố định
- ⏰ Thiết lập thời gian hiệu lực
- 🎫 Giới hạn số lần sử dụng
- 📈 Theo dõi thống kê sử dụng real-time

### 📊 Quản Lý Tồn Kho
- 📦 Theo dõi số lượng tồn kho
- ⚠️ Cảnh báo hết hàng
- 🔄 Cập nhật số lượng nhanh chóng
- 🔍 Tìm kiếm sản phẩm trong kho

### 🏢 Quản Lý Nhà Cung Cấp
- 📇 Quản lý thông tin nhà cung cấp
- 📞 Lưu trữ thông tin liên hệ
- 🔗 Liên kết với sản phẩm

### 📁 Quản Lý Danh Mục
- 🗂️ Phân loại sản phẩm theo danh mục
- ✏️ CRUD đầy đủ cho danh mục
- 🎨 Giao diện card đẹp mắt

### 📈 Dashboard
- 📊 Tổng quan thống kê kinh doanh
- 📉 Biểu đồ trực quan
- 🎯 Số liệu quan trọng

---

## 🛠️ Công Nghệ Sử Dụng

### Backend
- **ASP.NET Core 9.0** - Framework web hiện đại
- **Entity Framework Core** - ORM mạnh mẽ
- **Pomelo.EntityFrameworkCore.MySql** - MySQL provider
- **ASP.NET Core MVC** - Pattern MVC chuẩn

### Frontend
- **Razor Views** - Template engine của .NET
- **Bootstrap 5** - Framework CSS responsive
- **jQuery** - JavaScript library
- **Vanilla JavaScript** - Cho real-time search & interactions

### Database
- **MySQL 8.0+** - Hệ quản trị CSDL quan hệ

### Design Pattern
- **Repository Pattern** - Truy xuất dữ liệu
- **Generic Repository** - Code reusability
- **Dependency Injection** - Loose coupling
- **MVC Pattern** - Separation of concerns

---

## 💻 Yêu Cầu Hệ Thống

### Phần Mềm Cần Thiết

1. **Visual Studio Code** (hoặc Visual Studio 2022)
   - Download: https://code.visualstudio.com/

2. **.NET 9.0 SDK**
   - Download: https://dotnet.microsoft.com/download/dotnet/9.0
   - Kiểm tra cài đặt: `dotnet --version`

3. **XAMPP** (cho MySQL)
   - Download: https://www.apachefriends.org/download.html
   - Phiên bản: 8.0.0 trở lên (MySQL 8.0+)

4. **Git** (tùy chọn)
   - Download: https://git-scm.com/downloads

### Extensions cho VS Code (Khuyến nghị)

- **C# Dev Kit** - Microsoft
- **C#** - Microsoft
- **.NET Install Tool** - Microsoft
- **MySQL** - Jun Han (để quản lý database)

---

## 🚀 Hướng Dẫn Cài Đặt

### Bước 1: Clone Dự Án

```bash
# Clone repository
git clone https://github.com/zaikaman/QuanLyCuaHangBanLe.git

# Di chuyển vào thư mục dự án
cd QuanLyCuaHangBanLe
```

### Bước 2: Cài Đặt XAMPP và Khởi Động MySQL

1. **Cài đặt XAMPP:**
   - Tải XAMPP từ https://www.apachefriends.org/
   - Chạy installer và cài đặt (khuyến nghị cài vào `C:\xampp`)
   - Chọn components: Apache, MySQL, phpMyAdmin

2. **Khởi động MySQL:**
   - Mở XAMPP Control Panel
   - Click nút **Start** ở dòng MySQL
   - Đợi đến khi status hiển thị màu xanh

3. **Kiểm tra MySQL đã chạy:**
   - Mở trình duyệt và truy cập: `http://localhost/phpmyadmin`
   - Bạn sẽ thấy giao diện phpMyAdmin

### Bước 3: Tạo Database

1. Truy cập `http://localhost/phpmyadmin`
2. Click tab **SQL** ở phía trên
3. Import file `sql.sql` trong project
4. Click **Go**
5. Database `store_management` sẽ được tạo với dữ liệu mẫu

### Bước 4: Cấu Hình Connection String

1. Mở file `appsettings.json` trong thư mục gốc của project

2. Kiểm tra connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=store_management;User=root;Password=;"
  }
}
```

3. **Quan trọng:** Nếu bạn đã đặt password cho MySQL root trong XAMPP:
   - Thay `Password=;` thành `Password=your_password;`
   - Ví dụ: `Password=123456;`

4. Nếu MySQL chạy trên port khác 3306:
   ```json
   "Server=localhost;Port=3307;Database=store_management;User=root;Password=;"
   ```

### Bước 5: Restore Dependencies

Mở Terminal trong VS Code (Ctrl + `) và chạy:

```bash
# Restore các NuGet packages
dotnet restore

# Build project để kiểm tra lỗi
dotnet build
```

---

## 🗄️ Cấu Hình Database

### Schema Overview

Database bao gồm các bảng chính:

```
📊 Database: store_management
├── 👤 users (Người dùng)
├── 👥 customers (Khách hàng)
├── 📦 products (Sản phẩm)
├── 📁 categories (Danh mục)
├── 🏢 suppliers (Nhà cung cấp)
├── 🛒 orders (Đơn hàng)
├── 📋 order_items (Chi tiết đơn hàng)
├── 💳 payments (Thanh toán)
├── 🎁 promotions (Khuyến mãi)
└── 📊 inventory (Tồn kho)
```

### Relationships

```
categories ──┐
             ├──< products >── inventory
suppliers ───┘           ├──< order_items >── orders ──< payments
                                                  │
customers ────────────────────────────────────────┘
promotions ───────────────────────────────────────┘
users ────────────────────────────────────────────┘
```

### Migration (Nếu cần)

Nếu bạn muốn tạo lại database từ code:

```bash
# Cài đặt EF Core CLI tools (nếu chưa có)
dotnet tool install --global dotnet-ef

# Tạo migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update
```

---

## ▶️ Chạy Ứng Dụng

### Cách 1: Sử dụng Visual Studio Code

1. **Mở project trong VS Code:**
   ```bash
   code .
   ```

2. **Chạy ứng dụng:**
   - Nhấn `F5` hoặc
   - Mở Terminal (Ctrl + `) và chạy:
     ```bash
     dotnet run
     ```

3. **Truy cập ứng dụng:**
   - Mở trình duyệt và truy cập: `http://localhost:5166`
   - Hoặc: `https://localhost:7049` (HTTPS)

### Cách 2: Sử dụng dotnet watch (Hot Reload)

```bash
# Chạy với hot reload - tự động restart khi có thay đổi
dotnet watch run
```

### Cách 3: Build và Run Production

```bash
# Build ở chế độ Release
dotnet build --configuration Release

# Run
dotnet run --configuration Release
```

---

## 📁 Cấu Trúc Dự Án

```
QuanLyCuaHangBanLe/
│
├── Controllers/                # MVC Controllers
│   ├── AuthController.cs       # Xác thực & đăng nhập
│   ├── DashboardController.cs  # Trang tổng quan
│   ├── ProductsController.cs   # Quản lý sản phẩm
│   ├── CustomersController.cs  # Quản lý khách hàng
│   ├── OrdersController.cs     # Quản lý đơn hàng
│   ├── PromotionsController.cs # Quản lý khuyến mãi
│   ├── CategoriesController.cs # Quản lý danh mục
│   ├── SuppliersController.cs  # Quản lý nhà cung cấp
│   ├── InventoryController.cs  # Quản lý tồn kho
│   └── UsersController.cs      # Quản lý người dùng
│
├── Models/                     # Data Models
│   ├── User.cs                 # Model người dùng
│   ├── Customer.cs             # Model khách hàng
│   ├── Product.cs              # Model sản phẩm
│   ├── Category.cs             # Model danh mục
│   ├── Supplier.cs             # Model nhà cung cấp
│   ├── Order.cs                # Model đơn hàng
│   ├── OrderItem.cs            # Model chi tiết đơn
│   ├── Payment.cs              # Model thanh toán
│   ├── Promotion.cs            # Model khuyến mãi
│   └── Inventory.cs            # Model tồn kho
│
├── Views/                      # Razor Views
│   ├── Shared/                 # Layout chung
│   │   ├── _Layout.cshtml      # Layout chính
│   │   └── Error.cshtml        # Trang lỗi
│   ├── Auth/                   # Views xác thực
│   ├── Dashboard/              # Views dashboard
│   ├── Products/               # Views sản phẩm
│   ├── Customers/              # Views khách hàng
│   ├── Orders/                 # Views đơn hàng
│   ├── Promotions/             # Views khuyến mãi
│   ├── Categories/             # Views danh mục
│   ├── Suppliers/              # Views nhà cung cấp
│   ├── Inventory/              # Views tồn kho
│   └── Users/                  # Views người dùng
│
├── Services/                   # Business Logic Layer
│   ├── IGenericRepository.cs   # Interface repository
│   ├── GenericRepository.cs    # Generic repository
│   ├── ProductService.cs       # Service sản phẩm
│   ├── OrderService.cs         # Service đơn hàng
│   ├── IAuthService.cs         # Interface xác thực
│   └── AuthService.cs          # Service xác thực
│
├── Data/                       # Data Access Layer
│   └── ApplicationDbContext.cs # EF Core DbContext
│
├── wwwroot/                    # Static files
│   ├── css/                    # Stylesheets
│   ├── js/                     # JavaScript files
│   └── lib/                    # Client libraries
│
├── Properties/
│   └── launchSettings.json     # Launch configuration
│
├── appsettings.json            # App configuration
├── appsettings.Development.json # Dev configuration
├── Program.cs                  # Entry point
├── sql.sql                     # Database script
└── README.md                   # This file
```

---

## 🔑 Tài Khoản Mặc Định

Sau khi import database, bạn có thể đăng nhập với các tài khoản sau:

### Admin Account
```
Username: admin
Password: admin123
```

### Manager Account
```
Username: manager
Password: manager123
```

### Staff Account
```
Username: staff
Password: staff123
```

> ⚠️ **Lưu ý bảo mật:** Đổi password ngay sau lần đăng nhập đầu tiên trong môi trường production!

---

## 📖 Hướng Dẫn Sử Dụng

### 1. Đăng Nhập

1. Truy cập `http://localhost:5166`
2. Hệ thống tự động redirect đến trang login
3. Nhập username và password
4. Click "Đăng nhập"

### 2. Dashboard

Sau khi đăng nhập thành công:
- Xem tổng quan doanh số, đơn hàng, sản phẩm
- Biểu đồ thống kê trực quan
- Truy cập nhanh các chức năng chính

### 3. Quản Lý Sản Phẩm

**Thêm sản phẩm mới:**
1. Menu → Sản phẩm → "Thêm sản phẩm mới"
2. Điền thông tin: Tên, Giá, Đơn vị, Danh mục, Nhà cung cấp
3. Click "Lưu"

**Tìm kiếm sản phẩm:**
- Gõ vào ô tìm kiếm (tìm theo tên, barcode, category)
- Kết quả hiển thị real-time

**Chỉnh sửa/Xóa:**
- Click nút "Sửa" hoặc "Xóa" trên từng sản phẩm
- Xác nhận thao tác

### 4. Tạo Đơn Hàng

1. Menu → Đơn hàng → "Tạo đơn mới"
2. Chọn khách hàng từ dropdown
3. Thêm sản phẩm:
   - Click "Thêm sản phẩm"
   - Chọn sản phẩm và nhập số lượng
   - Giá tự động hiển thị
4. Nhập giảm giá (nếu có)
5. Chọn trạng thái thanh toán
6. Click "Tạo đơn hàng"

### 5. Quản Lý Khuyến Mãi

**Tạo mã giảm giá:**
1. Menu → Khuyến mãi → "Tạo khuyến mãi mới"
2. Nhập mã khuyến mãi (VD: SUMMER2024)
3. Chọn loại giảm giá: % hoặc số tiền
4. Nhập giá trị giảm
5. Thiết lập thời gian và giới hạn sử dụng
6. Click "Lưu"

### 6. Quản Lý Tồn Kho

- Xem số lượng tồn kho của từng sản phẩm
- Cảnh báo sản phẩm sắp hết hàng
- Cập nhật số lượng nhanh chóng

---

## 🔌 API Endpoints

### Authentication
```
POST   /Auth/Login              # Đăng nhập
POST   /Auth/Logout             # Đăng xuất
```

### Products
```
GET    /Products                # Danh sách sản phẩm (có phân trang)
GET    /Products/Details/{id}   # Chi tiết sản phẩm
GET    /Products/Create         # Form thêm sản phẩm
POST   /Products/Create         # Tạo sản phẩm mới
GET    /Products/Edit/{id}      # Form sửa sản phẩm
POST   /Products/Edit/{id}      # Cập nhật sản phẩm
GET    /Products/Delete/{id}    # Xác nhận xóa
POST   /Products/Delete/{id}    # Xóa sản phẩm
```

### Orders
```
GET    /Orders                  # Danh sách đơn hàng (có phân trang)
GET    /Orders/Details/{id}     # Chi tiết đơn hàng
GET    /Orders/Create           # Form tạo đơn
POST   /Orders/Create           # Tạo đơn hàng mới
POST   /Orders/GetProductInfo   # Lấy thông tin sản phẩm (AJAX)
```

### Customers
```
GET    /Customers               # Danh sách khách hàng (có phân trang)
GET    /Customers/Details/{id}  # Chi tiết khách hàng
GET    /Customers/Create        # Form thêm khách hàng
POST   /Customers/Create        # Tạo khách hàng mới
GET    /Customers/Edit/{id}     # Form sửa khách hàng
POST   /Customers/Edit/{id}     # Cập nhật khách hàng
GET    /Customers/Delete/{id}   # Xác nhận xóa
POST   /Customers/Delete/{id}   # Xóa khách hàng
```

### Promotions
```
GET    /Promotions              # Danh sách khuyến mãi (có phân trang)
GET    /Promotions/Details/{id} # Chi tiết khuyến mãi
GET    /Promotions/Create       # Form thêm khuyến mãi
POST   /Promotions/Create       # Tạo khuyến mãi mới
GET    /Promotions/Edit/{id}    # Form sửa khuyến mãi
POST   /Promotions/Edit/{id}    # Cập nhật khuyến mãi
POST   /Promotions/Delete/{id}  # Xóa khuyến mãi
```

### Categories, Suppliers, Users, Inventory
Tương tự pattern trên với CRUD đầy đủ.

---

## 🔧 Troubleshooting

### Lỗi: "Unable to connect to any of the specified MySQL hosts"

**Nguyên nhân:** MySQL trong XAMPP chưa được khởi động.

**Giải pháp:**
1. Mở XAMPP Control Panel
2. Click "Start" ở dòng MySQL
3. Đợi status chuyển sang màu xanh
4. Restart ứng dụng

### Lỗi: "Access denied for user 'root'@'localhost'"

**Nguyên nhân:** Password MySQL không đúng.

**Giải pháp:**
1. Kiểm tra password MySQL trong XAMPP (mặc định để trống)
2. Cập nhật `appsettings.json`:
   ```json
   "Password=your_password;"
   ```
3. Restart ứng dụng

### Lỗi: "Unknown database 'store_management'"

**Nguyên nhân:** Database chưa được tạo.

**Giải pháp:**
1. Truy cập phpMyAdmin: `http://localhost/phpmyadmin`
2. Import file `sql.sql`
3. Hoặc chạy lệnh SQL để tạo database

### Lỗi: "The type or namespace name 'MySql' could not be found"

**Nguyên nhân:** NuGet packages chưa được restore.

**Giải pháp:**
```bash
dotnet restore
dotnet build
```

### Port 5166 đã được sử dụng

**Giải pháp:**
1. Thay đổi port trong `Properties/launchSettings.json`
2. Tìm và thay `5166` thành port khác (VD: 5167)
3. Hoặc tắt ứng dụng đang sử dụng port đó

### Real-time search không hoạt động

**Nguyên nhân:** JavaScript chưa được load.

**Giải pháp:**
1. Hard refresh trình duyệt (Ctrl + F5)
2. Kiểm tra Console trong Developer Tools (F12)
3. Xóa cache trình duyệt

### Lỗi 404 khi truy cập trang Details

**Nguyên nhân:** ID không tồn tại hoặc URL sai.

**Giải pháp:**
1. Kiểm tra ID có tồn tại trong database
2. Đảm bảo format URL đúng: `/Controller/Details/id`

---

## 🎨 Customization

### Thay đổi màu chủ đạo

Mở `wwwroot/css/site.css` và thay đổi:

```css
:root {
    --primary-color: #1a1a1a;    /* Màu đen chủ đạo */
    --secondary-color: #f5f5f5;   /* Màu xám nhạt */
    --background: #ffffff;        /* Màu nền trắng */
}
```

### Thay đổi số items per page

Mở controller tương ứng và thay đổi:

```csharp
const int pageSize = 10; // Đổi thành số khác
```

### Thêm fields vào model

1. Sửa model trong thư mục `Models/`
2. Tạo migration: `dotnet ef migrations add AddNewField`
3. Update database: `dotnet ef database update`
4. Cập nhật views và controllers tương ứng

---

## 🤝 Đóng Góp

Chúng tôi hoan nghênh mọi đóng góp! Để đóng góp:

1. **Fork repository**
2. **Tạo branch mới:**
   ```bash
   git checkout -b feature/TenTinhNang
   ```
3. **Commit changes:**
   ```bash
   git commit -m "Thêm tính năng XYZ"
   ```
4. **Push to branch:**
   ```bash
   git push origin feature/TenTinhNang
   ```
5. **Tạo Pull Request**

### Coding Standards

- Sử dụng PascalCase cho class names và method names
- Sử dụng camelCase cho variables
- Comment code khi cần thiết
- Tuân thủ SOLID principles
- Write clean, readable code

---

## 📝 Changelog

### Version 1.0.0 (2025-10-03)
- ✅ Phát hành phiên bản đầu tiên
- ✅ Đầy đủ CRUD cho tất cả entities
- ✅ Real-time search cho Products, Customers, Orders, Inventory
- ✅ Pagination cho tất cả danh sách
- ✅ Details pages cho Products, Customers, Promotions, Suppliers
- ✅ Light theme với màu trắng/đen
- ✅ Session-based authentication
- ✅ Responsive design

---

## 📄 License

Dự án này được phát hành dưới giấy phép MIT License. Xem file [LICENSE](LICENSE) để biết thêm chi tiết.

```
MIT License

Copyright (c) 2025 zaikaman

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
```

---

## 👨‍💻 Tác Giả

**zaikaman**
- GitHub: [@zaikaman](https://github.com/zaikaman)
- Repository: [QuanLyCuaHangBanLe](https://github.com/zaikaman/QuanLyCuaHangBanLe)

---

## 🙏 Lời Cảm Ơn

- Microsoft ASP.NET Core Team
- Entity Framework Core Team
- Bootstrap Team
- MySQL Team
- Cộng đồng .NET Việt Nam

---

## 📞 Hỗ Trợ

Nếu bạn gặp vấn đề hoặc có câu hỏi:

1. **Kiểm tra phần [Troubleshooting](#-troubleshooting)**
2. **Tạo Issue:** [GitHub Issues](https://github.com/zaikaman/QuanLyCuaHangBanLe/issues)
3. **Email:** [your-email@example.com]

---

## 🌟 Đánh Giá

Nếu dự án này hữu ích với bạn, hãy cho chúng tôi một ⭐ trên GitHub!

---

<div align="center">
    <p>Made with ❤️ by zaikaman</p>
    <p>© 2025 Hệ Thống Quản Lý Cửa Hàng Bán Lẻ. All rights reserved.</p>
</div>
