# Hướng Dẫn Deploy Lên Heroku

## Mục Lục
1. [Yêu Cầu](#yêu-cầu)
2. [Chi Phí](#chi-phí)
3. [Bước 1: Chuẩn Bị Project](#bước-1-chuẩn-bị-project)
4. [Bước 2: Tạo Tài Khoản Heroku](#bước-2-tạo-tài-khoản-heroku)
5. [Bước 3: Cài Đặt Heroku CLI](#bước-3-cài-đặt-heroku-cli)
6. [Bước 4: Chuyển Đổi Database sang PostgreSQL](#bước-4-chuyển-đổi-database-sang-postgresql)
7. [Bước 5: Cấu Hình Project cho Heroku](#bước-5-cấu-hình-project-cho-heroku)
8. [Bước 6: Deploy Lên Heroku](#bước-6-deploy-lên-heroku)
9. [Bước 7: Cấu Hình Database](#bước-7-cấu-hình-database)
10. [Xử Lý Sự Cố](#xử-lý-sự-cố)

---

## Yêu Cầu

- **Git** đã được cài đặt
- **.NET SDK 8.0+** (Heroku hỗ trợ .NET 8.0 trở lên)
- **Tài khoản Heroku** (đã xác minh bằng thẻ tín dụng/ghi nợ)
- **Heroku CLI** đã được cài đặt

> ⚠️ **Lưu ý quan trọng**: Heroku **KHÔNG CÒN MIỄN PHÍ** từ tháng 11/2022. Bạn cần đăng ký gói Eco dynos ($5/tháng) hoặc Basic dynos ($7/tháng).

---

## Chi Phí

| Dịch vụ | Gói | Chi phí |
|---------|-----|---------|
| Dyno (Server) | Eco | $5/tháng (1000 dyno hours) |
| Dyno (Server) | Basic | $7/tháng |
| PostgreSQL | Essential-0 | $5/tháng |
| **Tổng tối thiểu** | | **~$10/tháng** |

> 💡 Nếu bạn là sinh viên, có thể đăng ký [Heroku for GitHub Students](https://blog.heroku.com/github-student-developer-program) để nhận credits miễn phí.

---

## Bước 1: Chuẩn Bị Project

### 1.1. Cập nhật Target Framework

Project hiện tại đang sử dụng .NET 9.0. Heroku hỗ trợ .NET 8.0+ nên có thể giữ nguyên hoặc hạ xuống .NET 8.0 để ổn định hơn.

**Nếu muốn dùng .NET 8.0**, sửa file `QuanLyCuaHangBanLe.csproj`:

```xml
<TargetFramework>net8.0</TargetFramework>
```

### 1.2. Tạo file `Procfile`

Tạo file `Procfile` (không có phần mở rộng) ở thư mục gốc:

```
web: cd $HOME/heroku_output && exec ./QuanLyCuaHangBanLe --urls http://0.0.0.0:$PORT
```

### 1.3. Tạo file `global.json` (tùy chọn)

Nếu muốn chỉ định phiên bản SDK cụ thể:

```json
{
  "sdk": {
    "version": "8.0.0",
    "rollForward": "latestFeature"
  }
}
```

---

## Bước 2: Tạo Tài Khoản Heroku

1. Truy cập [https://signup.heroku.com/](https://signup.heroku.com/)
2. Đăng ký tài khoản mới
3. Xác minh email
4. Thêm thẻ tín dụng/ghi nợ để xác minh tài khoản (bắt buộc để sử dụng add-ons như PostgreSQL)

---

## Bước 3: Cài Đặt Heroku CLI

### Windows (PowerShell)

```powershell
# Cách 1: Sử dụng Chocolatey
choco install heroku-cli

# Cách 2: Tải installer từ trang chủ
## Bước 4: Database — Giữ MySQL hoặc dùng PostgreSQL (tuỳ bạn)

Project hiện tại sử dụng **MySQL** (Pomelo). Trên Heroku bạn có hai cách chính để chạy database:

- Dùng MySQL add-on từ Heroku Marketplace (ví dụ `JawsDB` hoặc `ClearDB`).
- Dùng PostgreSQL (Heroku Postgres add-on) — lựa chọn phổ biến trên Heroku.

Tài liệu gốc của bạn đã hướng dẫn chuyển về PostgreSQL; tuy nhiên nếu bạn muốn giữ MySQL (ít thay đổi code), phần này hướng dẫn cách làm.

### 4.1. Nếu muốn GIỮ MySQL (khuyến nghị dùng Pomelo như hiện tại)

1. Giữ package `Pomelo.EntityFrameworkCore.MySql` trong `*.csproj` (không cần remove).

2. Trên Heroku, thêm một MySQL add-on (ví dụ `JawsDB` hoặc `ClearDB`):

```powershell
# JawsDB (ví dụ) - tên gói có thể khác theo Marketplace
heroku addons:create jawsdb:kite

# Hoặc ClearDB
heroku addons:create cleardb:ignite
```

3. Sau khi add-on được cấp, Heroku sẽ set một biến config như `JAWSDB_URL` hoặc `CLEARDB_DATABASE_URL` chứa URL dạng:

```
mysql://user:password@host:port/database_name
```

4. Cập nhật `Program.cs` (hoặc giữ code hiện tại) để khi chạy trên Heroku đọc biến đó và cấu hình Pomelo:

Ví dụ nhỏ (chỉ đoạn liên quan tới DbContext):

```csharp
var isHeroku = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DYNO"));
if (isHeroku)
{
    // Hỗ trợ JAWSDB_URL hoặc CLEARDB_DATABASE_URL
    var mysqlUrl = Environment.GetEnvironmentVariable("JAWSDB_URL") ?? Environment.GetEnvironmentVariable("CLEARDB_DATABASE_URL");
    if (!string.IsNullOrEmpty(mysqlUrl))
    {
        // mysqlUrl: mysql://user:pass@host:port/db
        var uri = new Uri(mysqlUrl);
        var userInfo = uri.UserInfo.Split(':');
        var cs = $"Server={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')}" +
                 $";User={userInfo[0]};Password={userInfo[1]};";
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(cs, ServerVersion.AutoDetect(cs)));
    }
    else
    {
        // fallback: nếu bạn vẫn set DATABASE_URL (không phổ biến cho MySQL)
        var connection = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connection, ServerVersion.AutoDetect(connection)));
    }
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
}
```

5. Khi dùng MySQL add-on, truy cập biến cấu hình:

```powershell
heroku config:get JAWSDB_URL
# hoặc
heroku config:get CLEARDB_DATABASE_URL
```

6. Tạo schema: bạn có thể chạy `heroku run "dotnet ef database update"` nếu dùng EF Migrations, hoặc vào MySQL CLI của add-on (tuỳ add-on cung cấp) và chạy nội dung `sql.sql` hiện có (MySQL syntax) — file `sql.sql` của repo là cú pháp MySQL, dùng được trực tiếp.

### 4.2. Nếu muốn CHUYỂN sang PostgreSQL (tùy chọn)

Phần hướng dẫn chuyển sang PostgreSQL vẫn giữ nguyên như trước (nếu bạn muốn tôi giữ lại phần chi tiết chuyển đổi sang Npgsql, tôi sẽ chèn lại). Lưu ý: nếu chuyển sang Postgres bạn cần thay đổi package và schema (ví dụ `sql_postgresql.sql`).

---

> Ghi chú: Heroku hỗ trợ cả MySQL add-ons trong Marketplace; tuy nhiên Heroku chính thức khuyến nghị Postgres và cung cấp nhiều tính năng quản lý cho Postgres. Nếu bạn muốn giữ MySQL để giảm thay đổi code, dùng `JawsDB`/`ClearDB` là cách nhanh nhất.
    }
}
```

### 4.3. Cập nhật SQL Schema cho PostgreSQL

Tạo file `sql_postgresql.sql` với cú pháp PostgreSQL:

```sql
-- DATABASE STORE MANAGEMENT FOR POSTGRESQL

-- Bảng người dùng
CREATE TABLE users (
    user_id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    full_name VARCHAR(100),
    role VARCHAR(20) DEFAULT 'staff' CHECK (role IN ('admin', 'staff')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng khách hàng
CREATE TABLE customers (
    customer_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    email VARCHAR(100),
    address TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng loại sản phẩm
CREATE TABLE categories (
    category_id SERIAL PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL
);

-- Bảng nhà cung cấp
CREATE TABLE suppliers (
    supplier_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    email VARCHAR(100),
    address TEXT
);

-- Bảng sản phẩm
CREATE TABLE products (
    product_id SERIAL PRIMARY KEY,
    category_id INT,
    supplier_id INT,
    product_name VARCHAR(100) NOT NULL,
    barcode VARCHAR(50) UNIQUE,
    price DECIMAL(10,2) NOT NULL,
    unit VARCHAR(20) DEFAULT 'pcs',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng tồn kho
CREATE TABLE inventory (
    inventory_id SERIAL PRIMARY KEY,
    product_id INT NOT NULL,
    quantity INT DEFAULT 0,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Bảng khuyến mãi
CREATE TABLE promotions (
    promo_id SERIAL PRIMARY KEY,
    promo_code VARCHAR(50) UNIQUE NOT NULL,
    description VARCHAR(255),
    discount_type VARCHAR(20) NOT NULL CHECK (discount_type IN ('percent', 'fixed')),
    discount_value DECIMAL(10,2) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    min_order_amount DECIMAL(10,2) DEFAULT 0,
    usage_limit INT DEFAULT 0,
    used_count INT DEFAULT 0,
    status VARCHAR(20) DEFAULT 'active' CHECK (status IN ('active', 'inactive'))
);

-- Bảng đơn hàng
CREATE TABLE orders (
    order_id SERIAL PRIMARY KEY,
    customer_id INT,
    user_id INT,
    promo_id INT NULL,
    order_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status VARCHAR(20) DEFAULT 'pending' CHECK (status IN ('pending', 'paid', 'canceled')),
    total_amount DECIMAL(10,2),
    discount_amount DECIMAL(10,2) DEFAULT 0
);

-- Bảng chi tiết đơn hàng
CREATE TABLE order_items (
    order_item_id SERIAL PRIMARY KEY,
    order_id INT,
    product_id INT,
    quantity INT NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    subtotal DECIMAL(10,2) NOT NULL
);

-- Bảng thanh toán
CREATE TABLE payments (
    payment_id SERIAL PRIMARY KEY,
    order_id INT,
    payment_method VARCHAR(50),
    amount DECIMAL(10,2),
    payment_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Thêm dữ liệu mẫu
INSERT INTO users (username, password, full_name, role) 
VALUES ('admin', 'admin123', 'Administrator', 'admin');
```

---

## Bước 5: Cấu Hình Project cho Heroku

### 5.1. Tạo file `.gitignore` (nếu chưa có)

```
bin/
obj/
.vs/
*.user
appsettings.Development.json
```

### 5.2. Khởi tạo Git Repository

```powershell
git init
git add .
git commit -m "Initial commit for Heroku deployment"
```

---

## Bước 6: Deploy Lên Heroku

### 6.1. Tạo Heroku App

```powershell
heroku create ten-app-cua-ban
```

Ví dụ:
```powershell
heroku create quanlycuahangbanle
```

> Heroku sẽ tự động tạo URL dạng: `https://quanlycuahangbanle.herokuapp.com`

### 6.2. Deploy Code

```powershell
git push heroku main
```

Hoặc nếu branch của bạn là `master`:
```powershell
git push heroku master
```

---

## Bước 7: Cấu Hình Database

### 7.1. Thêm PostgreSQL Add-on

```powershell
heroku addons:create heroku-postgresql:essential-0
```

> Add-on này có phí $5/tháng.

### 7.2. Kiểm tra thông tin Database

```powershell
heroku config
```

Bạn sẽ thấy biến `DATABASE_URL` được tự động thiết lập.

### 7.3. Chạy Migration hoặc Tạo Tables

Kết nối vào database và chạy SQL:

```powershell
heroku pg:psql
```

Sau đó copy và paste nội dung file `sql_postgresql.sql` để tạo các bảng.

Hoặc sử dụng Entity Framework Migrations:

```powershell
# Cài đặt dotnet-ef tool
dotnet tool install --global dotnet-ef

# Tạo migration
dotnet ef migrations add InitialCreate

# Push code và chạy migration
heroku run "dotnet ef database update"
```

---

## Xử Lý Sự Cố

### Xem Logs

```powershell
heroku logs --tail
```

### Khởi động lại App

```powershell
heroku restart
```

### Mở App trong trình duyệt

```powershell
heroku open
```

### Kiểm tra trạng thái Dynos

```powershell
heroku ps
```

### Chạy lệnh trên Heroku

```powershell
heroku run "dotnet --version"
```

---

## Các Lệnh Heroku Thường Dùng

| Lệnh | Mô tả |
|------|-------|
| `heroku login` | Đăng nhập |
| `heroku create` | Tạo app mới |
| `heroku logs --tail` | Xem logs realtime |
| `heroku config` | Xem biến môi trường |
| `heroku config:set KEY=VALUE` | Đặt biến môi trường |
| `heroku addons` | Xem các add-ons |
| `heroku pg:info` | Thông tin PostgreSQL |
| `heroku pg:psql` | Kết nối PostgreSQL CLI |
| `heroku restart` | Khởi động lại app |
| `heroku ps:scale web=1` | Scale dynos |

---

## Lưu Ý Quan Trọng

1. **Chi phí**: Heroku không còn miễn phí. Tối thiểu ~$10/tháng cho app + database.

2. **Sleep**: Gói Eco dynos sẽ sleep sau 30 phút không hoạt động.

3. **Storage**: Heroku sử dụng ephemeral filesystem - files upload sẽ bị xóa khi restart. Cần sử dụng dịch vụ lưu trữ bên ngoài như AWS S3, Cloudinary cho hình ảnh.

4. **Database Size**: Gói Essential-0 giới hạn 1GB data và 20 connections.

5. **SSL**: Heroku cung cấp SSL miễn phí cho tất cả apps.

---

## Tài Liệu Tham Khảo

- [Heroku .NET Documentation](https://devcenter.heroku.com/categories/dotnet-support)
- [Getting Started with .NET on Heroku](https://devcenter.heroku.com/articles/getting-started-with-dotnet)
- [Configuring ASP.NET Core Apps for Heroku](https://devcenter.heroku.com/articles/aspnetcore-app-configuration)
- [Heroku Postgres](https://devcenter.heroku.com/articles/heroku-postgresql)
