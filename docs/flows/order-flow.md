# Order Management Flow - Luồng Quản Lý Đơn Hàng

## 📋 Tổng Quan

Module Orders là trung tâm của hệ thống, quản lý toàn bộ quy trình bán hàng từ tạo đơn, thanh toán đến xuất báo cáo. Đây là module phức tạp nhất với nhiều business logic và validation.

## 🏗️ Kiến Trúc

### **Components**
- **Controller**: `OrdersController`
- **Service**: `OrderService` (extends `GenericRepository<Order>`)
- **Models**: `Order`, `OrderItem`, `Payment`
- **Related**: `Product`, `Customer`, `Promotion`, `Inventory`

### **Order Structure**
```csharp
Order {
    - OrderId (int, PK, Auto)
    - CustomerId (int, FK → Customers)
    - OrderDate (DateTime)
    - TotalAmount (decimal)
    - DiscountAmount (decimal)
    - PromoId (int?, FK → Promotions)
    - Status (string: "pending", "paid", "canceled")
    
    // Navigation
    - Customer
    - Promotion
    - OrderItems (List<OrderItem>)
    - Payments (List<Payment>)
}

OrderItem {
    - OrderItemId (int, PK, Auto)
    - OrderId (int, FK → Orders)
    - ProductId (int, FK → Products)
    - Quantity (int)
    - Price (decimal)
    - Subtotal (decimal)
    
    // Navigation
    - Order
    - Product
}

Payment {
    - PaymentId (int, PK, Auto)
    - OrderId (int, FK → Orders)
    - Amount (decimal)
    - PaymentMethod (string: "cash", "card", "bank_transfer", "e_wallet")
    - PaymentDate (DateTime)
    
    // Navigation
    - Order
}
```

---

## 🔄 Luồng Hoạt Động

### **1. Xem Danh Sách Đơn Hàng (Index)**

#### **Flow Diagram**
```
GET /Orders/Index?page=1&searchTerm=...
    ↓
Kiểm tra session
    ↓
OrderService.GetAllAsync()
    ├─ Include Customer
    ├─ Include OrderItems → Include Product
    └─ Include Payments
    ↓
Thống kê theo trạng thái
    ├─ PendingCount
    ├─ PaidCount
    └─ CanceledCount
    ↓
Áp dụng Search Filter
    ├─ OrderId
    ├─ Customer.Name
    ├─ Customer.Phone
    └─ Status
    ↓
Phân trang (pageSize = 10)
    ↓
Return View với thống kê và danh sách
```

#### **Code Flow**
```csharp
public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
{
    // 1. Authentication
    var username = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(username)) {
        return RedirectToAction("Login", "Auth");
    }

    // 2. Lấy tất cả đơn hàng
    const int pageSize = 10;
    var allOrders = await _orderService.GetAllAsync();
    
    // 3. Thống kê theo trạng thái
    ViewBag.PendingCount = allOrders.Count(o => o.Status == "pending");
    ViewBag.PaidCount = allOrders.Count(o => o.Status == "paid");
    ViewBag.CanceledCount = allOrders.Count(o => o.Status == "canceled");
    
    // 4. Tìm kiếm
    if (!string.IsNullOrWhiteSpace(searchTerm)) {
        searchTerm = searchTerm.Trim().ToLower();
        allOrders = allOrders.Where(o =>
            o.OrderId.ToString().Contains(searchTerm) ||
            (o.Customer?.Name != null && o.Customer.Name.ToLower().Contains(searchTerm)) ||
            (o.Customer?.Phone != null && o.Customer.Phone.Contains(searchTerm)) ||
            (o.Status != null && o.Status.ToLower().Contains(searchTerm))
        ).ToList();
    }
    
    // 5. Phân trang
    var totalItems = allOrders.Count();
    var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

    var orders = allOrders
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = totalPages;
    ViewBag.TotalItems = totalItems;
    ViewBag.SearchTerm = searchTerm;

    return View(orders);
}
```

---

### **2. Tạo Đơn Hàng Mới (Create)** - CORE FEATURE

#### **Flow Diagram - Part 1: Load Form**
```
GET /Orders/Create
    ↓
Kiểm tra session
    ↓
LoadDropdownData()
    ├─ Customers → ViewBag
    └─ Products → ViewBag
    ↓
Return View (empty form)
```

#### **Flow Diagram - Part 2: Submit Order**
```
User điền thông tin và thêm sản phẩm
    ↓
POST /Orders/Create
    ├─ Order object
    ├─ List<OrderItem>
    └─ paymentMethod
    ↓
Validate Session
    ├─ Username tồn tại?
    └─ UserId > 0?
    ↓
OrderService.CreateOrderAsync()
    ↓
[TRANSACTION BEGIN]
    ↓
    [1] VALIDATE ĐƠN HÀNG CƠ BẢN
        ├─ Order != null?
        ├─ OrderItems có ít nhất 1 item?
        └─ CustomerId hợp lệ?
    ↓
    [2] KIỂM TRA KHÁCH HÀNG TỒN TẠI
    ↓
    [3] VALIDATE & TÍNH TOÁN SẢN PHẨM
        Foreach OrderItem:
        ├─ Product tồn tại?
        ├─ Quantity > 0?
        ├─ Kiểm tra tồn kho đủ?
        ├─ Tính Price từ Product.Price
        ├─ Tính Subtotal = Quantity × Price
        └─ Cộng vào calculatedTotal
    ↓
    [4] VALIDATE GIẢM GIÁ
        ├─ DiscountAmount >= 0?
        └─ DiscountAmount <= TotalAmount?
    ↓
    [5] VALIDATE PROMOTION (nếu có)
        ├─ Promotion tồn tại?
        ├─ Status = "active"?
        ├─ Trong thời gian hiệu lực?
        ├─ Còn lượt sử dụng?
        ├─ Đủ giá trị đơn hàng tối thiểu?
        └─ Cập nhật UsedCount++
    ↓
    [6] THIẾT LẬP THÔNG TIN ĐƠN HÀNG
        ├─ OrderDate = DateTime.Now
        ├─ TotalAmount = calculatedTotal
        ├─ Status = "pending" hoặc "paid"
        └─ Validate FinalAmount >= 0
    ↓
    [7] LƯU ĐƠN HÀNG VÀO DB
        ├─ Insert Order
        └─ Get OrderId
    ↓
    [8] LƯU ORDER ITEMS & CẬP NHẬT TỒN KHO
        Foreach OrderItem:
        ├─ Set OrderId
        ├─ Insert OrderItem
        └─ Cập nhật Inventory.Quantity -= item.Quantity
    ↓
    [9] TẠO PAYMENT RECORD (nếu Status = "paid")
        ├─ Validate PaymentMethod
        ├─ Amount = FinalAmount
        └─ Insert Payment
    ↓
    [10] COMMIT TRANSACTION
    ↓
[TRANSACTION END]
    ↓
Success → Redirect /Orders/Details/{OrderId}
Error → TempData["Error"] + Reload form
```

#### **Code Flow - OrdersController.Create [POST]**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(
    Order order, 
    List<OrderItem> orderItems, 
    string paymentMethod = "cash")
{
    Console.WriteLine("🔵 ========== BẮT ĐẦU TẠO ĐƠN HÀNG ==========");
    
    // 1. KIỂM TRA SESSION
    var username = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(username)) {
        return RedirectToAction("Login", "Auth");
    }

    var userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null || userId <= 0) {
        TempData["Error"] = "Không tìm thấy thông tin người dùng";
        return RedirectToAction("Index");
    }

    // 2. GỌI SERVICE ĐỂ TẠO ĐƠN HÀNG
    var (success, message, createdOrder) = await _orderService.CreateOrderAsync(
        order, 
        orderItems, 
        paymentMethod
    );

    // 3. XỬ LÝ KẾT QUẢ
    if (success && createdOrder != null) {
        TempData["Success"] = "Tạo đơn hàng thành công!";
        return RedirectToAction("Details", new { id = createdOrder.OrderId });
    }
    else {
        TempData["Error"] = message;
        await LoadDropdownData();
        return View(order);
    }
}
```

#### **Code Flow - OrderService.CreateOrderAsync (Chi Tiết)**

```csharp
public async Task<(bool Success, string Message, Order? Order)> CreateOrderAsync(
    Order order, 
    List<OrderItem> orderItems, 
    string paymentMethod = "cash")
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    try {
        // ============ [1] VALIDATE CƠ BẢN ============
        if (order == null) {
            return (false, "Thông tin đơn hàng không hợp lệ", null);
        }

        if (orderItems == null || !orderItems.Any(i => i.ProductId > 0)) {
            return (false, "Đơn hàng phải có ít nhất một sản phẩm", null);
        }

        if (order.CustomerId == null || order.CustomerId <= 0) {
            return (false, "Vui lòng chọn khách hàng", null);
        }

        // ============ [2] KIỂM TRA KHÁCH HÀNG ============
        var customerExists = await _context.Customers
            .AnyAsync(c => c.CustomerId == order.CustomerId);
        if (!customerExists) {
            return (false, "Khách hàng không tồn tại", null);
        }

        // ============ [3] VALIDATE & TÍNH TOÁN SẢN PHẨM ============
        decimal calculatedTotal = 0;
        var validOrderItems = new List<OrderItem>();

        foreach (var item in orderItems.Where(i => i.ProductId > 0)) {
            // Lấy sản phẩm với inventory
            var product = await _context.Products
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.ProductId == item.ProductId);

            if (product == null) {
                return (false, $"Sản phẩm ID {item.ProductId} không tồn tại", null);
            }

            // Kiểm tra số lượng
            if (item.Quantity <= 0) {
                return (false, 
                    $"Số lượng sản phẩm '{product.ProductName}' phải lớn hơn 0", 
                    null);
            }

            // Kiểm tra tồn kho
            var currentStock = product.Inventory?.Quantity ?? 0;
            if (currentStock < item.Quantity) {
                return (false, 
                    $"Sản phẩm '{product.ProductName}' chỉ còn {currentStock} {product.Unit} trong kho", 
                    null);
            }

            // Tính giá
            item.Price = product.Price;
            item.Subtotal = item.Quantity * item.Price;
            calculatedTotal += item.Subtotal;

            validOrderItems.Add(item);
        }

        // ============ [4] VALIDATE GIẢM GIÁ ============
        if (order.DiscountAmount < 0) {
            return (false, "Giá trị giảm giá không hợp lệ", null);
        }

        if (order.DiscountAmount > calculatedTotal) {
            return (false, 
                "Giá trị giảm giá không được vượt quá tổng tiền hàng", 
                null);
        }

        // ============ [5] VALIDATE PROMOTION ============
        if (order.PromoId.HasValue && order.PromoId > 0) {
            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.PromoId == order.PromoId);

            if (promotion == null) {
                return (false, "Mã khuyến mãi không tồn tại", null);
            }

            if (promotion.Status != "active") {
                return (false, "Mã khuyến mãi không còn hiệu lực", null);
            }

            if (promotion.StartDate > DateTime.Now || 
                promotion.EndDate < DateTime.Now) {
                return (false, 
                    "Mã khuyến mãi đã hết hạn hoặc chưa bắt đầu", 
                    null);
            }

            if (promotion.UsageLimit > 0 && 
                promotion.UsedCount >= promotion.UsageLimit) {
                return (false, "Mã khuyến mãi đã hết lượt sử dụng", null);
            }

            if (promotion.MinOrderAmount > 0 && 
                calculatedTotal < promotion.MinOrderAmount) {
                return (false, 
                    $"Đơn hàng phải có giá trị tối thiểu {promotion.MinOrderAmount:N0}đ", 
                    null);
            }

            // Cập nhật số lần sử dụng
            promotion.UsedCount++;
            _context.Promotions.Update(promotion);
        }

        // ============ [6] THIẾT LẬP ĐƠN HÀNG ============
        order.OrderDate = DateTime.Now;
        order.TotalAmount = calculatedTotal;
        order.Status = string.IsNullOrEmpty(order.Status) ? "pending" : order.Status;

        var finalAmount = order.TotalAmount - order.DiscountAmount;
        if (finalAmount < 0) {
            return (false, "Tổng tiền đơn hàng không hợp lệ", null);
        }

        // ============ [7] LƯU ĐƠN HÀNG ============
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        // ============ [8] LƯU ORDER ITEMS & CẬP NHẬT TỒN KHO ============
        foreach (var item in validOrderItems) {
            item.OrderId = order.OrderId;
            await _context.OrderItems.AddAsync(item);

            // Cập nhật tồn kho
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);

            if (inventory != null) {
                inventory.Quantity -= item.Quantity;
                inventory.UpdatedAt = DateTime.Now;
                _context.Inventories.Update(inventory);
            }
            else {
                // Tạo mới inventory với số âm (nếu chưa có)
                var newInventory = new Inventory {
                    ProductId = item.ProductId,
                    Quantity = -item.Quantity,
                    UpdatedAt = DateTime.Now
                };
                await _context.Inventories.AddAsync(newInventory);
            }
        }

        await _context.SaveChangesAsync();

        // ============ [9] TẠO PAYMENT (nếu đã thanh toán) ============
        if (order.Status == "paid") {
            var validPaymentMethods = new[] { 
                "cash", "card", "bank_transfer", "e_wallet" 
            };
            if (!validPaymentMethods.Contains(paymentMethod.ToLower())) {
                paymentMethod = "cash";
            }
            
            var payment = new Payment {
                OrderId = order.OrderId,
                Amount = finalAmount,
                PaymentMethod = paymentMethod,
                PaymentDate = DateTime.Now
            };
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        // ============ [10] COMMIT TRANSACTION ============
        await transaction.CommitAsync();

        Console.WriteLine($"✅ ĐƠN HÀNG #{order.OrderId} TẠO THÀNH CÔNG");
        return (true, "Thành công", order);
    }
    catch (Exception ex) {
        await transaction.RollbackAsync();
        Console.WriteLine($"❌ LỖI TẠO ĐƠN HÀNG: {ex.Message}");
        return (false, "Lỗi hệ thống: " + ex.Message, null);
    }
}
```

---

### **3. Xem Chi Tiết Đơn Hàng (Details)**

#### **Flow Diagram**
```
GET /Orders/Details/{id}
    ↓
OrderService.GetByIdAsync(id)
    ├─ Include Customer
    ├─ Include OrderItems → Include Product
    └─ Include Payments
    ↓
Order tồn tại?
    ├─ KHÔNG → Return NotFound()
    └─ CÓ → Return View(order)
```

#### **View Features**
- Thông tin khách hàng
- Danh sách sản phẩm trong đơn
- Chi tiết tính toán: Tổng tiền, giảm giá, thành tiền
- Thông tin thanh toán
- Trạng thái đơn hàng
- Nút xuất Excel

---

### **4. Xuất Excel Đơn Hàng**

#### **Flow Diagram**
```
GET /Orders/ExportToExcel/{id}
    ↓
OrderService.GetByIdAsync(id)
    ↓
Tạo Excel với ClosedXML
    ├─ Thông tin cửa hàng
    ├─ Thông tin khách hàng
    ├─ Bảng chi tiết sản phẩm
    ├─ Tổng tiền, giảm giá, thành tiền
    └─ Format styling
    ↓
Return FileContentResult
    ├─ ContentType: application/vnd.openxmlformats...
    └─ FileName: DonHang_{OrderId}_{DateTime}.xlsx
```

---

### **5. API: Lấy Thông Tin Sản Phẩm (GetProductInfo)**

#### **Purpose**
AJAX endpoint để lấy thông tin sản phẩm khi người dùng chọn sản phẩm trong form tạo đơn hàng

#### **Flow**
```
POST /Orders/GetProductInfo
    ├─ productId (from AJAX)
    ↓
ProductService.GetByIdAsync(productId)
    ↓
Return JSON {
    success: true,
    productId: ...,
    productName: ...,
    price: ...,
    unit: ...
}
```

#### **Code**
```csharp
[HttpPost]
public async Task<IActionResult> GetProductInfo(int productId)
{
    var product = await _productService.GetByIdAsync(productId);
    if (product == null) {
        return Json(new { 
            success = false, 
            message = "Không tìm thấy sản phẩm" 
        });
    }
    
    return Json(new {
        success = true,
        productId = product.ProductId,
        productName = product.ProductName,
        price = product.Price,
        unit = product.Unit
    });
}
```

---

## ⚙️ Business Rules

### **Validation Rules**
| Rule | Description |
|------|-------------|
| **Khách hàng** | Phải chọn khách hàng hợp lệ |
| **Sản phẩm** | Ít nhất 1 sản phẩm, quantity > 0 |
| **Tồn kho** | Phải đủ số lượng trong kho |
| **Giá** | Lấy từ Product.Price (không cho phép sửa) |
| **Giảm giá** | 0 <= DiscountAmount <= TotalAmount |
| **Promotion** | Phải active, trong thời gian, đủ điều kiện |
| **Tổng tiền** | FinalAmount >= 0 |
| **Payment** | Chỉ tạo khi Status = "paid" |

### **Order Status**
```csharp
- "pending"  : Chưa thanh toán
- "paid"     : Đã thanh toán
- "canceled" : Đã hủy
```

### **Payment Methods**
```csharp
- "cash"          : Tiền mặt
- "card"          : Thẻ
- "bank_transfer" : Chuyển khoản
- "e_wallet"      : Ví điện tử
```

---

## 🔄 Transaction Handling

### **ACID Properties**
```csharp
using var transaction = await _context.Database.BeginTransactionAsync();
try {
    // 1. Thêm Order
    // 2. Thêm OrderItems
    // 3. Cập nhật Inventory
    // 4. Cập nhật Promotion.UsedCount
    // 5. Thêm Payment
    
    await transaction.CommitAsync();
}
catch {
    await transaction.RollbackAsync();
    // Tất cả thay đổi bị hủy
}
```

### **Rollback Scenarios**
- Lỗi validation giữa chừng
- Không đủ tồn kho
- Lỗi database
- Exception bất kỳ

---

## 📊 Statistics & Reports

### **Doanh Thu**
```csharp
// Tổng doanh thu
public async Task<decimal> GetTotalRevenueAsync(
    DateTime? startDate = null, 
    DateTime? endDate = null)
{
    var query = _dbSet.Where(o => o.Status == "paid");
    
    if (startDate.HasValue)
        query = query.Where(o => o.OrderDate >= startDate.Value);
    
    if (endDate.HasValue)
        query = query.Where(o => o.OrderDate < endDate.Value);
    
    return await query.SumAsync(o => o.TotalAmount - o.DiscountAmount);
}

// Doanh thu theo tháng (6 tháng gần nhất)
public async Task<Dictionary<string, decimal>> GetMonthlyRevenueAsync(int months = 6)

// Doanh thu theo danh mục
public async Task<Dictionary<string, decimal>> GetRevenueByCategoryAsync()
```

### **Đơn Hàng**
```csharp
// Tổng số đơn hàng
public async Task<int> GetTotalOrdersAsync(
    DateTime? startDate = null, 
    DateTime? endDate = null)

// Thống kê theo trạng thái
ViewBag.PendingCount = orders.Count(o => o.Status == "pending");
ViewBag.PaidCount = orders.Count(o => o.Status == "paid");
ViewBag.CanceledCount = orders.Count(o => o.Status == "canceled");
```

---

## 🐛 Error Handling

### **Common Errors**
```csharp
// Không có sản phẩm
"Đơn hàng phải có ít nhất một sản phẩm"

// Không đủ tồn kho
"Sản phẩm 'Coca Cola' chỉ còn 5 Chai trong kho"

// Giảm giá quá lớn
"Giá trị giảm giá không được vượt quá tổng tiền hàng"

// Promotion không hợp lệ
"Mã khuyến mãi đã hết hạn hoặc chưa bắt đầu"
"Mã khuyến mãi đã hết lượt sử dụng"
"Đơn hàng phải có giá trị tối thiểu 100,000đ để sử dụng mã này"

// Success
"Tạo đơn hàng thành công!"
```

---

## 📝 Testing Checklist

- [ ] Tạo đơn với 1 sản phẩm → Thành công
- [ ] Tạo đơn với nhiều sản phẩm → Thành công
- [ ] Tạo đơn không có sản phẩm → Hiện lỗi
- [ ] Tạo đơn với số lượng > tồn kho → Hiện lỗi
- [ ] Tạo đơn với giảm giá hợp lệ → Thành công
- [ ] Tạo đơn với giảm giá > tổng tiền → Hiện lỗi
- [ ] Áp dụng mã khuyến mãi hợp lệ → Thành công, UsedCount++
- [ ] Áp dụng mã khuyến mãi hết hạn → Hiện lỗi
- [ ] Áp dụng mã khuyến mãi không đủ giá trị tối thiểu → Hiện lỗi
- [ ] Tạo đơn Status = "paid" → Tạo Payment record
- [ ] Tạo đơn Status = "pending" → Không tạo Payment
- [ ] Kiểm tra tồn kho sau khi tạo đơn → Đã giảm
- [ ] Xuất Excel đơn hàng → Download file thành công
- [ ] Tìm kiếm đơn hàng theo ID → Tìm thấy
- [ ] Tìm kiếm đơn hàng theo tên khách → Tìm thấy
- [ ] Xem thống kê theo trạng thái → Hiển thị đúng

---

## 🔗 Related Files

- `Controllers/OrdersController.cs`
- `Services/OrderService.cs`
- `Models/Order.cs`
- `Models/OrderItem.cs`
- `Models/Payment.cs`
- `Views/Orders/Index.cshtml`
- `Views/Orders/Create.cshtml`
- `Views/Orders/Details.cshtml`
