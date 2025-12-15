# Dashboard Flow - Luồng Bảng Điều Khiển

## 📋 Tổng Quan

Dashboard là trang chủ sau khi đăng nhập, cung cấp tổng quan về tình hình kinh doanh thông qua các chỉ số, biểu đồ và bảng thống kê.

## 🏗️ Kiến Trúc

### **Components**
- **Controller**: `DashboardController`
- **Services**: `OrderService`, `ProductService`, `IGenericRepository<Customer>`
- **Views**: `Dashboard/Index.cshtml`
- **Charts**: Chart.js (Line Chart, Donut Chart)

---

## 🔄 Luồng Hoạt Động

### **Flow Diagram**
```
GET /Dashboard/Index?dateRange=week&startDate=...&endDate=...
    ↓
Kiểm tra session (đã đăng nhập?)
    ├─ KHÔNG → Redirect /Auth/Login
    └─ CÓ → Tiếp tục
    ↓
Kiểm tra ErrorMessage từ session (từ chối quyền)
    ↓
Xác định khoảng thời gian lọc
    ├─ today: Hôm nay
    ├─ week: Tuần này (default)
    ├─ month: Tháng này
    └─ custom: startDate → endDate
    ↓
Tính toán các chỉ số
    ├─ [1] Tổng doanh thu (tất cả thời gian)
    ├─ [2] Doanh thu tháng này
    ├─ [3] Doanh thu tháng trước (để so sánh)
    ├─ [4] Doanh thu theo bộ lọc
    ├─ [5] Tổng số đơn hàng
    ├─ [6] Đơn hàng tháng này
    ├─ [7] Đơn hàng tháng trước
    ├─ [8] Tổng số khách hàng
    └─ [9] Tổng số sản phẩm
    ↓
Dữ liệu cho biểu đồ
    ├─ [10] Doanh thu 6 tháng gần nhất (Line Chart)
    └─ [11] Doanh thu theo danh mục (Donut Chart)
    ↓
Dữ liệu đơn hàng gần đây (theo bộ lọc)
    ├─ 5 đơn hàng mới nhất
    └─ Thống kê theo trạng thái (pending, paid, canceled)
    ↓
Return View với ViewBag chứa tất cả dữ liệu
```

---

## 📊 Chỉ Số & Thống Kê

### **1. Doanh Thu**

#### **Tổng Doanh Thu**
```csharp
ViewBag.TotalRevenue = await _orderService.GetTotalRevenueAsync();
```
- Tất cả đơn hàng có Status = "paid"
- Tính từ lúc bắt đầu hoạt động

#### **Doanh Thu Tháng Này**
```csharp
var startOfMonth = new DateTime(today.Year, today.Month, 1);
ViewBag.MonthRevenue = await _orderService.GetTotalRevenueAsync(
    startOfMonth, 
    today.AddDays(1)
);
```

#### **Doanh Thu Tháng Trước**
```csharp
var lastMonthStart = startOfMonth.AddMonths(-1);
var lastMonthEnd = startOfMonth;
ViewBag.LastMonthRevenue = await _orderService.GetTotalRevenueAsync(
    lastMonthStart, 
    lastMonthEnd
);
```

#### **Doanh Thu Theo Bộ Lọc**
```csharp
ViewBag.FilteredRevenue = await _orderService.GetTotalRevenueAsync(
    filterStart, 
    filterEnd
);
```

### **2. Đơn Hàng**

#### **Tổng Số Đơn Hàng**
```csharp
ViewBag.TotalOrders = await _orderService.GetTotalOrdersAsync();
```

#### **Đơn Hàng Tháng Này/Tháng Trước**
```csharp
ViewBag.OrdersThisMonth = await _orderService.GetTotalOrdersAsync(
    startOfMonth, 
    today.AddDays(1)
);
ViewBag.OrdersLastMonth = await _orderService.GetTotalOrdersAsync(
    lastMonthStart, 
    lastMonthEnd
);
```

### **3. Khách Hàng & Sản Phẩm**
```csharp
ViewBag.TotalCustomers = await _customerRepository.CountAsync();
ViewBag.TotalProducts = await _productService.CountAsync();
```

---

## 📈 Biểu Đồ

### **1. Doanh Thu 6 Tháng (Line Chart)**

#### **Data Structure**
```csharp
var monthlyRevenue = await _orderService.GetMonthlyRevenueAsync(6);
// Dictionary<string, decimal>
// Key: "2025-06", "2025-07", ...
// Value: Doanh thu của tháng đó

ViewBag.MonthlyRevenueLabels = monthlyRevenue.Keys.ToList();
ViewBag.MonthlyRevenueData = monthlyRevenue.Values.ToList();
```

#### **OrderService Implementation**
```csharp
public async Task<Dictionary<string, decimal>> GetMonthlyRevenueAsync(int months = 6)
{
    var result = new Dictionary<string, decimal>();
    var startDate = DateTime.Now.AddMonths(-months);
    
    var orders = await _dbSet
        .Where(o => o.Status == "paid" && o.OrderDate >= startDate)
        .ToListAsync();
    
    // Nhóm theo tháng
    var grouped = orders
        .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
        .OrderBy(g => g.Key.Year)
        .ThenBy(g => g.Key.Month);
    
    foreach (var group in grouped) {
        var key = $"{group.Key.Year}-{group.Key.Month:D2}";
        var revenue = group.Sum(o => o.TotalAmount - o.DiscountAmount);
        result[key] = revenue;
    }
    
    return result;
}
```

#### **Chart.js Configuration**
```javascript
new Chart(ctx, {
    type: 'line',
    data: {
        labels: @Html.Raw(Json.Serialize(ViewBag.MonthlyRevenueLabels)),
        datasets: [{
            label: 'Doanh thu',
            data: @Html.Raw(Json.Serialize(ViewBag.MonthlyRevenueData)),
            borderColor: 'rgb(75, 192, 192)',
            tension: 0.1
        }]
    }
});
```

### **2. Doanh Thu Theo Danh Mục (Donut Chart)**

#### **Data Structure**
```csharp
var categoryRevenue = await _orderService.GetRevenueByCategoryAsync();
// Dictionary<string, decimal>
// Key: Tên danh mục
// Value: Doanh thu của danh mục đó

ViewBag.CategoryLabels = categoryRevenue.Keys.ToList();
ViewBag.CategoryData = categoryRevenue.Values.ToList();

// Tính phần trăm
var totalCategoryRevenue = categoryRevenue.Values.Sum();
ViewBag.CategoryPercentages = categoryRevenue.ToDictionary(
    x => x.Key,
    x => totalCategoryRevenue > 0 
        ? Math.Round((x.Value / totalCategoryRevenue) * 100, 1) 
        : 0
);
```

#### **OrderService Implementation**
```csharp
public async Task<Dictionary<string, decimal>> GetRevenueByCategoryAsync()
{
    var result = new Dictionary<string, decimal>();
    
    var orderItems = await _context.OrderItems
        .Include(oi => oi.Order)
        .Include(oi => oi.Product)
            .ThenInclude(p => p.Category)
        .Where(oi => oi.Order.Status == "paid")
        .ToListAsync();
    
    var grouped = orderItems
        .GroupBy(oi => oi.Product?.Category?.CategoryName ?? "Khác");
    
    foreach (var group in grouped) {
        var revenue = group.Sum(oi => oi.Subtotal);
        var discount = group
            .Select(oi => oi.Order)
            .Distinct()
            .Sum(o => o.DiscountAmount);
        
        result[group.Key] = revenue - discount;
    }
    
    return result.OrderByDescending(x => x.Value)
                 .ToDictionary(x => x.Key, x => x.Value);
}
```

---

## 🔍 Bộ Lọc Thời Gian

### **Date Range Options**
```csharp
switch (dateRange.ToLower())
{
    case "today":
        filterStart = today;
        filterEnd = today.AddDays(1);
        break;
    
    case "week":  // DEFAULT
        filterStart = today.AddDays(-(int)today.DayOfWeek);  // Chủ nhật
        filterEnd = today.AddDays(1);
        break;
    
    case "month":
        filterStart = new DateTime(today.Year, today.Month, 1);
        filterEnd = today.AddDays(1);
        break;
    
    default:
        // Custom range
        if (startDate.HasValue && endDate.HasValue) {
            filterStart = startDate.Value;
            filterEnd = endDate.Value;
        }
        break;
}

ViewBag.DateRange = dateRange;
ViewBag.StartDate = filterStart;
ViewBag.EndDate = filterEnd;
```

### **URL Examples**
```
/Dashboard/Index?dateRange=today
/Dashboard/Index?dateRange=week
/Dashboard/Index?dateRange=month
/Dashboard/Index?startDate=2025-01-01&endDate=2025-01-31
```

---

## 📋 Đơn Hàng Gần Đây

### **Data**
```csharp
var allOrders = await _orderService.GetAllAsync();

// Lọc theo thời gian
var filteredOrders = allOrders
    .Where(o => o.OrderDate >= filterStart && o.OrderDate < filterEnd)
    .ToList();

// 5 đơn mới nhất
ViewBag.RecentOrders = filteredOrders
    .OrderByDescending(o => o.OrderDate)
    .Take(5);

// Thống kê theo trạng thái
ViewBag.PendingOrders = filteredOrders.Count(o => o.Status == "pending");
ViewBag.PaidOrders = filteredOrders.Count(o => o.Status == "paid");
ViewBag.CanceledOrders = filteredOrders.Count(o => o.Status == "canceled");
```

### **Display**
- OrderId
- Customer Name
- OrderDate
- TotalAmount - DiscountAmount
- Status (màu sắc khác nhau)

---

## 🎨 UI Components

### **1. Stat Cards (4 Cards)**
```
┌──────────────┬──────────────┬──────────────┬──────────────┐
│ Tổng Doanh Thu│ Đơn Hàng    │ Khách Hàng   │ Sản Phẩm     │
│ 50,000,000đ  │ 1,234       │ 567          │ 123          │
│ +15% ↑       │ +23 ↑       │              │              │
└──────────────┴──────────────┴──────────────┴──────────────┘
```

### **2. Charts (2 Charts)**
```
┌────────────────────────┬────────────────────────┐
│ Doanh Thu 6 Tháng      │ Doanh Thu Theo DM      │
│ (Line Chart)           │ (Donut Chart)          │
│                        │                        │
│      /\    /\         │      🍩                 │
│    /    \/    \        │    40% Đồ uống        │
│  /              \      │    30% Thực phẩm      │
│/                  \    │    20% Đồ gia dụng    │
│                        │    10% Khác           │
└────────────────────────┴────────────────────────┘
```

### **3. Recent Orders Table**
```
┌────────────────────────────────────────────────┐
│ Đơn Hàng Gần Đây                               │
├─────┬────────────┬─────────┬──────────┬────────┤
│ ID  │ Khách hàng │ Ngày    │ Tổng tiền│ Status │
├─────┼────────────┼─────────┼──────────┼────────┤
│ 123 │ Nguyễn A   │ 15/12   │ 500,000đ │ Paid   │
│ 122 │ Trần B     │ 15/12   │ 300,000đ │Pending │
└─────┴────────────┴─────────┴──────────┴────────┘
```

### **4. Status Overview**
```
Pending: 15 đơn
Paid: 100 đơn
Canceled: 5 đơn
```

---

## 🔄 API: Get Orders By Status

### **Purpose**
AJAX endpoint để lọc đơn hàng theo trạng thái

### **Flow**
```
GET /Dashboard/GetOrdersByStatus?status=paid
    ↓
Lấy tất cả orders
    ↓
Lọc theo status (nếu có)
    ↓
Return JSON với danh sách orders
```

### **Code**
```csharp
[HttpGet]
public async Task<IActionResult> GetOrdersByStatus(string status)
{
    var username = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(username)) {
        return Json(new { success = false, message = "Chưa đăng nhập" });
    }

    var allOrders = await _orderService.GetAllAsync();
    var filteredOrders = string.IsNullOrEmpty(status) 
        ? allOrders 
        : allOrders.Where(o => o.Status == status);

    var result = filteredOrders
        .OrderByDescending(o => o.OrderDate)
        .Take(10)
        .Select(o => new {
            orderId = o.OrderId,
            customerName = o.Customer?.Name,
            orderDate = o.OrderDate.ToString("dd/MM/yyyy"),
            totalAmount = o.TotalAmount - o.DiscountAmount,
            status = o.Status
        });

    return Json(new { success = true, orders = result });
}
```

---

## 📊 Code Example - Dashboard Index

```csharp
public async Task<IActionResult> Index(
    string dateRange = "week", 
    DateTime? startDate = null, 
    DateTime? endDate = null)
{
    // 1. AUTHENTICATION
    var username = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(username)) {
        return RedirectToAction("Login", "Auth");
    }

    // 2. KIỂM TRA ERROR MESSAGE (từ Authorization filter)
    var errorMessage = HttpContext.Session.GetString("ErrorMessage");
    if (!string.IsNullOrEmpty(errorMessage)) {
        TempData["Error"] = errorMessage;
        HttpContext.Session.Remove("ErrorMessage");
    }

    // 3. XÁC ĐỊNH KHOẢNG THỜI GIAN
    var today = DateTime.Today;
    var startOfMonth = new DateTime(today.Year, today.Month, 1);
    
    DateTime filterStart, filterEnd;
    // ... (logic xác định filterStart, filterEnd)

    // 4. TÍNH TOÁN DOANH THU
    ViewBag.TotalRevenue = await _orderService.GetTotalRevenueAsync();
    ViewBag.MonthRevenue = await _orderService.GetTotalRevenueAsync(
        startOfMonth, 
        today.AddDays(1)
    );
    
    var lastMonthStart = startOfMonth.AddMonths(-1);
    var lastMonthEnd = startOfMonth;
    ViewBag.LastMonthRevenue = await _orderService.GetTotalRevenueAsync(
        lastMonthStart, 
        lastMonthEnd
    );
    
    ViewBag.FilteredRevenue = await _orderService.GetTotalRevenueAsync(
        filterStart, 
        filterEnd
    );

    // 5. TÍNH TOÁN ĐƠN HÀNG
    ViewBag.TotalOrders = await _orderService.GetTotalOrdersAsync();
    ViewBag.OrdersThisMonth = await _orderService.GetTotalOrdersAsync(
        startOfMonth, 
        today.AddDays(1)
    );
    ViewBag.OrdersLastMonth = await _orderService.GetTotalOrdersAsync(
        lastMonthStart, 
        lastMonthEnd
    );

    // 6. KHÁCH HÀNG & SẢN PHẨM
    ViewBag.TotalCustomers = await _customerRepository.CountAsync();
    ViewBag.TotalProducts = await _productService.CountAsync();

    // 7. DỮ LIỆU BIỂU ĐỒ
    var monthlyRevenue = await _orderService.GetMonthlyRevenueAsync(6);
    ViewBag.MonthlyRevenueLabels = monthlyRevenue.Keys.ToList();
    ViewBag.MonthlyRevenueData = monthlyRevenue.Values.ToList();

    var categoryRevenue = await _orderService.GetRevenueByCategoryAsync();
    ViewBag.CategoryLabels = categoryRevenue.Keys.ToList();
    ViewBag.CategoryData = categoryRevenue.Values.ToList();
    var totalCategoryRevenue = categoryRevenue.Values.Sum();
    ViewBag.CategoryPercentages = categoryRevenue.ToDictionary(
        x => x.Key,
        x => totalCategoryRevenue > 0 
            ? Math.Round((x.Value / totalCategoryRevenue) * 100, 1) 
            : 0
    );

    // 8. ĐƠN HÀNG GẦN ĐÂY
    var allOrders = await _orderService.GetAllAsync();
    var filteredOrders = allOrders
        .Where(o => o.OrderDate >= filterStart && o.OrderDate < filterEnd)
        .ToList();
    
    ViewBag.RecentOrders = filteredOrders
        .OrderByDescending(o => o.OrderDate)
        .Take(5);
    
    ViewBag.PendingOrders = filteredOrders.Count(o => o.Status == "pending");
    ViewBag.PaidOrders = filteredOrders.Count(o => o.Status == "paid");
    ViewBag.CanceledOrders = filteredOrders.Count(o => o.Status == "canceled");
    
    // 9. VIEWBAG METADATA
    ViewBag.DateRange = dateRange;
    ViewBag.StartDate = filterStart;
    ViewBag.EndDate = filterEnd;

    return View();
}
```

---

## 📝 Testing Checklist

- [ ] Truy cập Dashboard sau khi đăng nhập → Hiển thị đúng
- [ ] Xem tổng doanh thu → Hiển thị đúng số liệu
- [ ] Xem doanh thu tháng này → Hiển thị đúng
- [ ] Lọc theo "Hôm nay" → Hiển thị doanh thu hôm nay
- [ ] Lọc theo "Tuần này" → Hiển thị doanh thu tuần này
- [ ] Lọc theo "Tháng này" → Hiển thị doanh thu tháng này
- [ ] Lọc theo khoảng tùy chỉnh → Hiển thị đúng
- [ ] Biểu đồ doanh thu 6 tháng → Render đúng
- [ ] Biểu đồ doanh thu theo danh mục → Render đúng, tổng 100%
- [ ] Xem 5 đơn hàng gần đây → Hiển thị đúng
- [ ] Thống kê đơn hàng theo trạng thái → Đếm đúng
- [ ] Staff truy cập Dashboard → Thành công
- [ ] Chưa đăng nhập truy cập → Redirect Login

---

## 🔗 Related Files

- `Controllers/DashboardController.cs`
- `Services/OrderService.cs`
- `Views/Dashboard/Index.cshtml`
- `wwwroot/js/chart.js` (Chart.js library)
