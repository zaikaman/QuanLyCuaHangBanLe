# Tài Liệu Hệ Thống Quản Lý Cửa Hàng Bán Lẻ

## 📋 Tổng Quan

Hệ thống Quản Lý Cửa Hàng Bán Lẻ là một ứng dụng web được xây dựng bằng ASP.NET Core MVC, giúp quản lý toàn bộ hoạt động của cửa hàng bán lẻ bao gồm: sản phẩm, đơn hàng, khách hàng, tồn kho, nhà cung cấp, khuyến mãi và người dùng.

## 🏗️ Kiến Trúc Hệ Thống

### **Technology Stack**
- **Framework**: ASP.NET Core MVC (.NET 9.0)
- **Database**: MySQL
- **ORM**: Entity Framework Core
- **Cloud Storage**: Cloudinary (cho hình ảnh)
- **Authentication**: Session-based Authentication
- **UI**: Bootstrap 5, Chart.js

### **Mô Hình Kiến Trúc**
```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│    (Controllers + Views + wwwroot)      │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│         Business Logic Layer            │
│           (Services)                    │
│  - AuthService                          │
│  - OrderService                         │
│  - ProductService                       │
│  - CloudinaryService                    │
│  - GenericRepository                    │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│          Data Access Layer              │
│      (ApplicationDbContext)             │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│            Database                     │
│            (MySQL)                      │
└─────────────────────────────────────────┘
```

## 📚 Modules Chính

### 1. **Authentication (Xác Thực)**
- Đăng nhập/Đăng xuất
- Đổi mật khẩu
- Quản lý phiên làm việc
- Phân quyền (Admin/Staff)
- 📄 [Chi tiết →](flows/auth-flow.md)

### 2. **Dashboard (Bảng Điều Khiển)**
- Thống kê doanh thu theo thời gian
- Thống kê đơn hàng theo trạng thái
- Doanh thu theo danh mục sản phẩm
- Top sản phẩm bán chạy
- Lọc theo khoảng thời gian
- 📄 [Chi tiết →](flows/dashboard-flow.md)

### 3. **Products (Sản Phẩm)**
- Danh sách sản phẩm với phân trang
- Thêm/Sửa/Xóa sản phẩm
- Tìm kiếm sản phẩm
- Upload ảnh lên Cloudinary
- Quản lý mã vạch
- 📄 [Chi tiết →](flows/product-flow.md)

### 4. **Orders (Đơn Hàng)**
- Tạo đơn hàng mới
- Quản lý đơn hàng
- Xuất Excel đơn hàng
- Tính toán tự động
- Cập nhật tồn kho tự động
- Quản lý thanh toán
- 📄 [Chi tiết →](flows/order-flow.md)

### 5. **Inventory (Tồn Kho)**
- Xem tồn kho theo sản phẩm
- Cập nhật số lượng tồn kho (Admin)
- Tìm kiếm tồn kho
- 📄 [Chi tiết →](flows/inventory-flow.md)

### 6. **Customers (Khách Hàng)**
- Quản lý thông tin khách hàng
- Thêm/Sửa/Xóa khách hàng
- Tìm kiếm khách hàng
- Xem lịch sử mua hàng
- 📄 [Chi tiết →](flows/customer-flow.md)

### 7. **Categories (Danh Mục)**
- Quản lý loại sản phẩm
- Thêm/Sửa/Xóa danh mục (Admin)
- Tạo nhanh danh mục qua AJAX
- 📄 [Chi tiết →](flows/category-flow.md)

### 8. **Suppliers (Nhà Cung Cấp)**
- Quản lý nhà cung cấp
- Thêm/Sửa/Xóa nhà cung cấp (Admin)
- Tìm kiếm nhà cung cấp
- 📄 [Chi tiết →](flows/supplier-flow.md)

### 9. **Promotions (Khuyến Mãi)**
- Quản lý mã khuyến mãi
- Thêm/Sửa/Xóa khuyến mãi (Admin)
- Hỗ trợ giảm giá theo % hoặc số tiền
- Quản lý thời gian hiệu lực
- 📄 [Chi tiết →](flows/promotion-flow.md)

### 10. **Users (Người Dùng)**
- Quản lý tài khoản người dùng (Admin)
- Thêm/Sửa/Xóa người dùng
- Phân quyền Admin/Staff
- 📄 [Chi tiết →](flows/user-flow.md)

## 🔐 Phân Quyền

### **Admin**
- Toàn quyền truy cập tất cả chức năng
- Quản lý người dùng
- Quản lý danh mục, nhà cung cấp, khuyến mãi
- Cập nhật tồn kho
- Thêm/Sửa/Xóa sản phẩm

### **Staff**
- Xem dashboard
- Quản lý đơn hàng
- Xem sản phẩm, tồn kho
- Quản lý khách hàng
- Không thể thay đổi cấu hình hệ thống

## 🔄 Luồng Dữ Liệu Chính

### **Quy Trình Bán Hàng Tổng Quát**
```
1. Nhân viên đăng nhập
   ↓
2. Chọn khách hàng (hoặc tạo mới)
   ↓
3. Thêm sản phẩm vào đơn hàng
   ↓
4. Hệ thống kiểm tra tồn kho
   ↓
5. Áp dụng khuyến mãi (nếu có)
   ↓
6. Xác nhận thanh toán
   ↓
7. Hệ thống:
   - Tạo đơn hàng
   - Tạo payment record
   - Giảm tồn kho
   - Cập nhật thống kê
   ↓
8. Hoàn tất và in hóa đơn (xuất Excel)
```

## 📊 Database Schema

### **Các Bảng Chính**
- `users` - Người dùng hệ thống
- `customers` - Khách hàng
- `categories` - Danh mục sản phẩm
- `suppliers` - Nhà cung cấp
- `products` - Sản phẩm
- `inventory` - Tồn kho
- `promotions` - Khuyến mãi
- `orders` - Đơn hàng
- `order_items` - Chi tiết đơn hàng
- `payments` - Thanh toán

### **Mối Quan Hệ**
```
categories 1──────* products
suppliers  1──────* products
products   1──────1 inventory
products   1──────* order_items
customers  1──────* orders
orders     1──────* order_items
orders     1──────* payments
promotions *──────* orders
```

## 🛠️ Services

### **GenericRepository<T>**
Repository pattern chung cho tất cả entities với các phương thức:
- `GetAllAsync()` - Lấy tất cả
- `GetByIdAsync(id)` - Lấy theo ID
- `AddAsync(entity)` - Thêm mới
- `UpdateAsync(entity)` - Cập nhật
- `DeleteAsync(id)` - Xóa
- `CountAsync()` - Đếm số lượng

### **AuthService**
Xử lý xác thực và phân quyền:
- Đăng nhập
- Đổi mật khẩu
- Quản lý session

### **OrderService**
Xử lý business logic cho đơn hàng:
- Tạo đơn hàng với validation
- Cập nhật tồn kho tự động
- Tính toán tổng tiền
- Thống kê doanh thu
- Transaction handling

### **ProductService**
Xử lý business logic cho sản phẩm:
- CRUD với eager loading
- Tìm kiếm theo barcode
- Kiểm tra khả năng xóa

### **CloudinaryService**
Xử lý upload ảnh:
- Upload ảnh lên Cloudinary
- Validation file type
- Xử lý lỗi upload

## 🎨 UI/UX Features

- **Responsive Design**: Tương thích mọi thiết bị
- **Real-time Validation**: Validate form ngay lập tức
- **AJAX Operations**: Không reload trang cho một số thao tác
- **Toast Notifications**: Thông báo thân thiện
- **Data Tables**: Phân trang, tìm kiếm, sắp xếp
- **Charts**: Biểu đồ trực quan với Chart.js
- **Modal Dialogs**: Xác nhận trước khi xóa

## 📝 Coding Conventions

### **Naming Conventions**
- Controllers: `<Entity>Controller` (VD: `ProductsController`)
- Services: `<Entity>Service` (VD: `OrderService`)
- Models: Singular form (VD: `Product`, `Order`)
- Views: Trong thư mục theo controller name

### **Error Handling**
- Try-catch trong controllers
- Logging ra console với emoji icons (🔵 ✅ ❌)
- TempData để truyền thông báo giữa requests
- ModelState cho validation errors

### **Session Management**
- `Username`: Tên đăng nhập
- `FullName`: Họ tên đầy đủ
- `Role`: Vai trò (admin/staff)
- `UserId`: ID người dùng

## 🚀 Deployment Notes

### **Yêu Cầu Hệ Thống**
- .NET 9.0 SDK
- MySQL 8.0+
- Cloudinary account (cho upload ảnh)

### **Cấu Hình**
File `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=...;database=...;user=...;password=..."
  },
  "Cloudinary": {
    "CloudName": "...",
    "ApiKey": "...",
    "ApiSecret": "..."
  }
}
```

## 📖 Đọc Thêm

- [Authentication Flow](flows/auth-flow.md)
- [Dashboard Flow](flows/dashboard-flow.md)
- [Product Management Flow](flows/product-flow.md)
- [Order Management Flow](flows/order-flow.md)
- [Inventory Management Flow](flows/inventory-flow.md)
- [Customer Management Flow](flows/customer-flow.md)
- [Category Management Flow](flows/category-flow.md)
- [Supplier Management Flow](flows/supplier-flow.md)
- [Promotion Management Flow](flows/promotion-flow.md)
- [User Management Flow](flows/user-flow.md)

---

**Version**: 1.0.0  
**Last Updated**: December 2025
