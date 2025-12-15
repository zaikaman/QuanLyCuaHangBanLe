# Customer, Category, Supplier, Promotion, User Management Flows

## 📋 Tổng Quan

Các module này có cấu trúc CRUD tương tự nhau, đều là quản lý master data của hệ thống.

---

# 1. Customer Management Flow - Quản Lý Khách Hàng

## 🏗️ Kiến Trúc

### **Components**
- **Controller**: `CustomersController`
- **Repository**: `IGenericRepository<Customer>`
- **Model**: `Customer`

### **Customer Properties**
```csharp
Customer {
    - CustomerId (int, PK, Auto)
    - Name (string, required)
    - Phone (string, nullable)
    - Email (string, nullable)
    - Address (string, nullable)
    - CreatedAt (DateTime)
    
    // Navigation
    - Orders (List<Order>)
}
```

## 🔄 CRUD Operations

### **Index - Danh Sách**
- Phân trang (10 items/page)
- Tìm kiếm: Name, Phone, Email, Address
- Hiển thị: Name, Phone, Email, số lượng đơn hàng

### **Create - Thêm Mới**
- **Fields**: Name (required), Phone, Email, Address
- **Validation**:
  - Name không được rỗng
  - Email phải unique (nếu nhập)
  - Phone phải unique (nếu nhập)
- **CreatedAt**: Tự động set DateTime.Now

### **Edit - Chỉnh Sửa**
- Validate tương tự Create
- Kiểm tra unique (loại trừ chính nó)

### **Delete - Xóa**
- **Ràng buộc**: Không thể xóa nếu khách hàng đã có đơn hàng
- Kiểm tra trước khi xóa

### **Details - Chi Tiết**
- Thông tin khách hàng
- Lịch sử mua hàng (orders)

---

# 2. Category Management Flow - Quản Lý Danh Mục

## 🏗️ Kiến Trúc

### **Components**
- **Controller**: `CategoriesController` [AdminOnly]
- **Repository**: `IGenericRepository<Category>`
- **Model**: `Category`

### **Category Properties**
```csharp
Category {
    - CategoryId (int, PK, Auto)
    - CategoryName (string, required, unique)
    
    // Navigation
    - Products (List<Product>)
}
```

## 🔄 CRUD Operations

### **Index - Danh Sách** [AdminOnly]
- Phân trang (10 items/page)
- Tìm kiếm: CategoryName
- Hiển thị: CategoryName, số lượng sản phẩm

### **Create - Thêm Mới** [AdminOnly]
- **Fields**: CategoryName (required)
- **Validation**:
  - CategoryName không được rỗng
  - CategoryName phải unique (case-insensitive)
  - Length: 2-100 ký tự

### **CreateAjax - Thêm Nhanh** [AdminOnly]
- AJAX endpoint cho modal trong trang Create Product
- Validate tương tự Create
- Return JSON { success, message }

### **Edit - Chỉnh Sửa** [AdminOnly]
- Validate tương tự Create
- Kiểm tra unique (loại trừ chính nó)

### **Delete - Xóa** [AdminOnly]
- **Ràng buộc**: Không thể xóa nếu danh mục đã có sản phẩm
- Confirm trước khi xóa

## 🎯 Special Features

### **Quick Add từ Product Form**
```
User đang ở trang Create Product
    ↓
Không tìm thấy Category mong muốn
    ↓
Click "Thêm nhanh danh mục"
    ↓
Modal popup
    ↓
Nhập tên danh mục
    ↓
AJAX POST /Categories/CreateAjax
    ↓
Success → Thêm vào dropdown, close modal
Error → Hiển thị lỗi
```

---

# 3. Supplier Management Flow - Quản Lý Nhà Cung Cấp

## 🏗️ Kiến Trúc

### **Components**
- **Controller**: `SuppliersController` [AdminOnly]
- **Repository**: `IGenericRepository<Supplier>`
- **Model**: `Supplier`

### **Supplier Properties**
```csharp
Supplier {
    - SupplierId (int, PK, Auto)
    - Name (string, required)
    - Phone (string, nullable)
    - Email (string, nullable)
    - Address (string, nullable)
    
    // Navigation
    - Products (List<Product>)
}
```

## 🔄 CRUD Operations

### **Index - Danh Sách** [AdminOnly]
- Phân trang (10 items/page)
- Tìm kiếm: Name, Phone, Email, Address
- Hiển thị: Name, Phone, Email, Address

### **Create - Thêm Mới** [AdminOnly]
- **Fields**: Name (required), Phone, Email, Address
- **Validation**:
  - Name không được rỗng
  - Email phải unique (nếu nhập)
  - Phone phải unique (nếu nhập)

### **Edit - Chỉnh Sửa** [AdminOnly]
- Validate tương tự Create
- Kiểm tra unique (loại trừ chính nó)

### **Delete - Xóa** [AdminOnly]
- **Ràng buộc**: Không thể xóa nếu nhà cung cấp đã có sản phẩm
- Confirm trước khi xóa

### **Details - Chi Tiết** [AdminOnly]
- Thông tin nhà cung cấp
- Danh sách sản phẩm từ nhà cung cấp này

---

# 4. Promotion Management Flow - Quản Lý Khuyến Mãi

## 🏗️ Kiến Trúc

### **Components**
- **Controller**: `PromotionsController` [AdminOnly]
- **Repository**: `IGenericRepository<Promotion>`
- **Model**: `Promotion`

### **Promotion Properties**
```csharp
Promotion {
    - PromoId (int, PK, Auto)
    - PromoCode (string, required, unique)
    - Description (string, nullable)
    - DiscountType (string: "percent" | "fixed")
    - DiscountValue (decimal)
    - StartDate (DateTime)
    - EndDate (DateTime)
    - MinOrderAmount (decimal, nullable)
    - UsageLimit (int, nullable)
    - UsedCount (int, default: 0)
    - Status (string: "active" | "inactive")
    
    // Navigation
    - Orders (List<Order>)
}
```

## 🔄 CRUD Operations

### **Index - Danh Sách** [AdminOnly]
- Phân trang (10 items/page)
- Tìm kiếm: PromoCode, Description, Status
- Hiển thị: PromoCode, DiscountType, DiscountValue, StartDate-EndDate, UsedCount/UsageLimit, Status

### **Create - Thêm Mới** [AdminOnly]
- **Fields**: PromoCode, Description, DiscountType, DiscountValue, StartDate, EndDate, MinOrderAmount, UsageLimit, Status
- **Validation**:
  - PromoCode không được rỗng, phải unique
  - EndDate phải sau StartDate
  - DiscountValue <= 100 nếu DiscountType = "percent"
  - PromoCode tự động uppercase

### **Edit - Chỉnh Sửa** [AdminOnly]
- Validate tương tự Create
- Không thay đổi UsedCount (chỉ system cập nhật)

### **Delete - Xóa** [AdminOnly]
- **Ràng buộc**: Không thể xóa nếu promotion đã được sử dụng (UsedCount > 0)
- Hoặc có thể chuyển Status = "inactive" thay vì xóa

### **Details - Chi Tiết** [AdminOnly]
- Thông tin promotion
- Danh sách đơn hàng đã sử dụng promotion này
- Thống kê hiệu quả

## 🎯 Business Logic

### **Validation trong OrderService**
```csharp
if (order.PromoId.HasValue) {
    var promo = await _context.Promotions.FindAsync(order.PromoId);
    
    // 1. Status phải "active"
    if (promo.Status != "active") → Error
    
    // 2. Trong thời gian hiệu lực
    if (DateTime.Now < promo.StartDate || DateTime.Now > promo.EndDate) → Error
    
    // 3. Còn lượt sử dụng
    if (promo.UsageLimit > 0 && promo.UsedCount >= promo.UsageLimit) → Error
    
    // 4. Đủ giá trị đơn hàng tối thiểu
    if (promo.MinOrderAmount > 0 && orderTotal < promo.MinOrderAmount) → Error
    
    // 5. Cập nhật UsedCount
    promo.UsedCount++;
}
```

### **Discount Calculation**
```csharp
if (promotion.DiscountType == "percent") {
    discountAmount = orderTotal * (promotion.DiscountValue / 100);
}
else if (promotion.DiscountType == "fixed") {
    discountAmount = promotion.DiscountValue;
}

finalAmount = orderTotal - discountAmount;
```

---

# 5. User Management Flow - Quản Lý Người Dùng

## 🏗️ Kiến Trúc

### **Components**
- **Controller**: `UsersController` [AdminOnly]
- **Repository**: `IGenericRepository<User>`
- **Model**: `User`

### **User Properties**
```csharp
User {
    - UserId (int, PK, Auto)
    - Username (string, required, unique)
    - Password (string, required)
    - FullName (string, required)
    - Role (string: "admin" | "staff")
    - CreatedAt (DateTime)
}
```

## 🔄 CRUD Operations

### **Index - Danh Sách** [AdminOnly]
- Phân trang (10 items/page)
- Tìm kiếm: Username, FullName, Role
- Hiển thị: Username, FullName, Role, CreatedAt

### **Create - Thêm Mới** [AdminOnly]
- **Fields**: Username, Password, FullName, Role
- **Validation**:
  - Username không được rỗng, phải unique
  - Password tối thiểu 6 ký tự
  - FullName không được rỗng
  - Role phải là "admin" hoặc "staff"
- **CreatedAt**: Tự động set DateTime.Now

### **Edit - Chỉnh Sửa** [AdminOnly]
- **Lưu ý**: Không cho phép sửa Password ở đây
- Để đổi password → Dùng ChangePassword trong AuthController
- Validate Username unique (loại trừ chính nó)

### **Delete - Xóa** [AdminOnly]
- **Ràng buộc**: Không thể xóa chính mình (user đang đăng nhập)
- Confirm trước khi xóa
- ⚠️ Cân nhắc: Không nên xóa user đã tạo đơn hàng (soft delete thay vì hard delete)

## 🔐 Security Notes

### **⚠️ Password Security**
```csharp
// HIỆN TẠI: Plain text (KHÔNG AN TOÀN)
user.Password = plainPassword;

// NÊN: Hash password
user.Password = BCrypt.HashPassword(plainPassword);
```

### **Role Management**
```csharp
// Role dropdown trong form
<select name="Role">
    <option value="admin">Admin</option>
    <option value="staff">Staff</option>
</select>
```

---

## 📊 Common Patterns

### **1. Pagination (Tất Cả Modules)**
```csharp
const int pageSize = 10;
var totalItems = allItems.Count();
var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

var items = allItems
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToList();

ViewBag.CurrentPage = page;
ViewBag.TotalPages = totalPages;
ViewBag.TotalItems = totalItems;
```

### **2. Search (Tất Cả Modules)**
```csharp
if (!string.IsNullOrWhiteSpace(searchTerm)) {
    searchTerm = searchTerm.Trim().ToLower();
    items = items.Where(/* search conditions */).ToList();
}
ViewBag.SearchTerm = searchTerm;
```

### **3. Unique Validation**
```csharp
// Khi tạo mới
var existing = await _repository.GetAllAsync();
if (existing.Any(x => x.Field.Equals(newValue, StringComparison.OrdinalIgnoreCase))) {
    ModelState.AddModelError("Field", "Giá trị này đã tồn tại");
}

// Khi chỉnh sửa (loại trừ chính nó)
if (existing.Any(x => x.Id != currentId && 
                      x.Field.Equals(newValue, StringComparison.OrdinalIgnoreCase))) {
    ModelState.AddModelError("Field", "Giá trị này đã tồn tại");
}
```

### **4. Delete với Ràng Buộc**
```csharp
// Kiểm tra foreign key constraints
var hasRelatedData = await _context.RelatedTable.AnyAsync(r => r.ForeignKey == id);
if (hasRelatedData) {
    TempData["Error"] = "Không thể xóa do còn dữ liệu liên quan";
    return RedirectToAction("Index");
}

await _repository.DeleteAsync(id);
TempData["Success"] = "Xóa thành công!";
```

---

## 🔗 Related Files

### **Customers**
- `Controllers/CustomersController.cs`
- `Models/Customer.cs`
- `Views/Customers/` (Index, Create, Edit, Delete, Details)

### **Categories**
- `Controllers/CategoriesController.cs`
- `Models/Category.cs`
- `Views/Categories/` (Index, Create, Edit)

### **Suppliers**
- `Controllers/SuppliersController.cs`
- `Models/Supplier.cs`
- `Views/Suppliers/` (Index, Create, Edit, Delete, Details)

### **Promotions**
- `Controllers/PromotionsController.cs`
- `Models/Promotion.cs`
- `Views/Promotions/` (Index, Create, Edit, Details)
- `Services/OrderService.cs` (Promotion validation)

### **Users**
- `Controllers/UsersController.cs`
- `Models/User.cs`
- `Views/Users/` (Index, Create, Edit, Delete)
- `Services/AuthService.cs`
