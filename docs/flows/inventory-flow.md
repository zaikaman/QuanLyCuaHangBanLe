# Inventory Management Flow - Luồng Quản Lý Tồn Kho

## 📋 Tổng Quan

Module Inventory theo dõi số lượng tồn kho của từng sản phẩm. Tồn kho được cập nhật tự động khi có đơn hàng mới, và Admin có thể cập nhật thủ công khi nhập hàng.

## 🏗️ Kiến Trúc

### **Components**
- **Controller**: `InventoryController`
- **Repository**: `IGenericRepository<Inventory>`
- **Service**: `ProductService`
- **Model**: `Inventory`

### **Inventory Properties**
```csharp
Inventory {
    - InventoryId (int, PK, Auto)
    - ProductId (int, FK → Products, Unique)
    - Quantity (int)
    - UpdatedAt (DateTime)
    
    // Navigation
    - Product (1-1 relationship)
}
```

---

## 🔄 Luồng Hoạt Động

### **1. Xem Danh Sách Tồn Kho (Index)**

#### **Flow Diagram**
```
GET /Inventory/Index?page=1&searchTerm=...
    ↓
Kiểm tra session
    ↓
Lấy tất cả Inventory
    ├─ Load thông tin Product
    └─ Tạo Dictionary ProductId → Product
    ↓
Áp dụng tìm kiếm (nếu có)
    ├─ ProductId
    ├─ Product.ProductName
    └─ Product.Barcode
    ↓
Phân trang (pageSize = 10)
    ↓
ViewBag.UserRole = Session["Role"]
    ↓
Return View với:
    ├─ List<Inventory>
    ├─ Products dictionary
    ├─ Pagination info
    └─ UserRole (để ẩn/hiện nút cập nhật)
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

    // 2. Lấy dữ liệu
    const int pageSize = 10;
    var allInventories = await _inventoryRepository.GetAllAsync();
    
    // 3. Load products để search và display
    var products = await _productService.GetAllAsync();
    var productDict = products.ToDictionary(p => p.ProductId, p => p);
    
    // 4. Tìm kiếm
    if (!string.IsNullOrWhiteSpace(searchTerm)) {
        searchTerm = searchTerm.Trim().ToLower();
        allInventories = allInventories.Where(i =>
            // Tìm theo ProductId
            i.ProductId.ToString().Contains(searchTerm) ||
            
            // Tìm theo tên sản phẩm
            (productDict.ContainsKey(i.ProductId) && 
             productDict[i.ProductId].ProductName != null && 
             productDict[i.ProductId].ProductName!.ToLower().Contains(searchTerm)) ||
            
            // Tìm theo barcode
            (productDict.ContainsKey(i.ProductId) && 
             productDict[i.ProductId].Barcode != null && 
             productDict[i.ProductId].Barcode!.ToLower().Contains(searchTerm))
        ).ToList();
    }
    
    // 5. Phân trang
    var totalItems = allInventories.Count();
    var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

    var inventories = allInventories
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();
    
    // 6. ViewBag
    ViewBag.Products = products;
    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = totalPages;
    ViewBag.TotalItems = totalItems;
    ViewBag.SearchTerm = searchTerm;
    ViewBag.UserRole = HttpContext.Session.GetString("Role") ?? "staff";

    return View(inventories);
}
```

#### **Display Information**
- ProductId
- Product Name
- Product Barcode
- Product Category
- Current Quantity
- Unit
- Last Updated
- **Update Button** (chỉ Admin mới thấy)

---

### **2. Cập Nhật Số Lượng (UpdateQuantity)** - ADMIN ONLY

#### **Flow Diagram**
```
User (Admin) click "Cập nhật" hoặc nhập số mới
    ↓
AJAX POST /Inventory/UpdateQuantity
    ├─ inventoryId
    └─ quantity (new value)
    ↓
[AdminOnly] - Kiểm tra role
    ↓
Tìm Inventory theo ID
    ├─ KHÔNG TÌM THẤY → Return JSON error
    └─ TÌM THẤY → Tiếp tục
    ↓
Cập nhật
    ├─ inventory.Quantity = quantity
    └─ inventory.UpdatedAt = DateTime.Now
    ↓
Repository.UpdateAsync()
    ↓
Return JSON { success: true }
```

#### **Code Flow**
```csharp
[AdminOnly]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UpdateQuantity(int inventoryId, int quantity)
{
    try {
        // 1. Tìm inventory
        var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);
        if (inventory == null) {
            return Json(new { 
                success = false, 
                message = "Không tìm thấy thông tin tồn kho" 
            });
        }

        // 2. Cập nhật
        inventory.Quantity = quantity;
        inventory.UpdatedAt = DateTime.Now;
        await _inventoryRepository.UpdateAsync(inventory);
        
        return Json(new { 
            success = true, 
            message = "Cập nhật số lượng thành công!" 
        });
    }
    catch (Exception ex) {
        return Json(new { 
            success = false, 
            message = "Lỗi: " + ex.Message 
        });
    }
}
```

#### **AJAX Implementation (Frontend)**
```javascript
function updateInventory(inventoryId) {
    var newQuantity = prompt("Nhập số lượng mới:");
    if (newQuantity === null) return;
    
    newQuantity = parseInt(newQuantity);
    if (isNaN(newQuantity) || newQuantity < 0) {
        alert("Số lượng không hợp lệ!");
        return;
    }
    
    $.ajax({
        url: '/Inventory/UpdateQuantity',
        type: 'POST',
        data: {
            inventoryId: inventoryId,
            quantity: newQuantity,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function(response) {
            if (response.success) {
                alert(response.message);
                location.reload();
            } else {
                alert(response.message);
            }
        },
        error: function() {
            alert("Có lỗi xảy ra khi cập nhật!");
        }
    });
}
```

---

## 🔄 Automatic Inventory Updates

### **Cập Nhật Tự Động Khi Tạo Đơn Hàng**

#### **Flow trong OrderService.CreateOrderAsync**
```csharp
// Sau khi lưu OrderItems
foreach (var item in validOrderItems) {
    // ... Lưu OrderItem
    
    // CẬP NHẬT TỒN KHO
    var inventory = await _context.Inventories
        .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);

    if (inventory != null) {
        // Inventory đã tồn tại → Giảm số lượng
        inventory.Quantity -= item.Quantity;
        inventory.UpdatedAt = DateTime.Now;
        _context.Inventories.Update(inventory);
    }
    else {
        // Inventory chưa tồn tại → Tạo mới với số âm
        var newInventory = new Inventory {
            ProductId = item.ProductId,
            Quantity = -item.Quantity,  // Số âm!
            UpdatedAt = DateTime.Now
        };
        await _context.Inventories.AddAsync(newInventory);
    }
}

await _context.SaveChangesAsync();
```

#### **Giải Thích Số Âm**
- Nếu tồn kho chưa được khởi tạo nhưng có đơn hàng
- Số âm = Đã bán nhưng chưa nhập kho
- Admin cần nhập hàng để bù đắp

### **Example Scenario**
```
1. Sản phẩm "Coca Cola" chưa có trong Inventory
2. Tạo đơn hàng bán 10 chai
3. Tạo Inventory với Quantity = -10
4. Admin nhập 50 chai → Cập nhật Quantity = -10 + 50 = 40
5. Bán thêm 15 chai → Quantity = 40 - 15 = 25
```

---

## 🚨 Inventory Warnings

### **Low Stock Alert**
```csharp
// Trong View có thể thêm logic hiển thị cảnh báo
@if (inventory.Quantity <= 10) {
    <span class="badge bg-danger">Sắp hết hàng</span>
}
else if (inventory.Quantity <= 20) {
    <span class="badge bg-warning">Tồn kho thấp</span>
}
else {
    <span class="badge bg-success">Còn hàng</span>
}
```

### **Out of Stock**
```csharp
@if (inventory.Quantity <= 0) {
    <span class="badge bg-dark">Hết hàng</span>
}
```

---

## 🔐 Authorization

### **Role-Based Access**
```csharp
// Tất cả user có thể XEM tồn kho
GET /Inventory/Index → Cho phép Staff & Admin

// Chỉ Admin mới có thể CẬP NHẬT
POST /Inventory/UpdateQuantity → [AdminOnly] filter
```

### **UI Conditional Rendering**
```cshtml
@if (ViewBag.UserRole == "admin") {
    <button onclick="updateInventory(@inventory.InventoryId)">
        Cập nhật
    </button>
}
else {
    <span class="text-muted">Chỉ Admin mới cập nhật được</span>
}
```

---

## 📊 Search Features

### **Search By**
- **ProductId**: Tìm theo ID sản phẩm
- **Product Name**: Tìm theo tên sản phẩm (case-insensitive)
- **Barcode**: Tìm theo mã vạch

### **Implementation**
```csharp
searchTerm = searchTerm.Trim().ToLower();

allInventories = allInventories.Where(i =>
    // ID
    i.ProductId.ToString().Contains(searchTerm) ||
    
    // Tên sản phẩm
    (productDict.ContainsKey(i.ProductId) && 
     productDict[i.ProductId].ProductName != null && 
     productDict[i.ProductId].ProductName!.ToLower().Contains(searchTerm)) ||
    
    // Barcode
    (productDict.ContainsKey(i.ProductId) && 
     productDict[i.ProductId].Barcode != null && 
     productDict[i.ProductId].Barcode!.ToLower().Contains(searchTerm))
).ToList();
```

---

## 🐛 Error Handling

### **Common Errors**
```csharp
// Inventory không tồn tại
return Json(new { 
    success: false, 
    message: "Không tìm thấy thông tin tồn kho" 
});

// Cập nhật thành công
return Json(new { 
    success: true, 
    message: "Cập nhật số lượng thành công!" 
});

// System error
return Json(new { 
    success: false, 
    message: "Lỗi: " + ex.Message 
});
```

---

## 🔄 Integration with Other Modules

### **Product Creation**
```csharp
// Khi tạo sản phẩm mới, có thể tự động tạo inventory
var newInventory = new Inventory {
    ProductId = product.ProductId,
    Quantity = 0,
    UpdatedAt = DateTime.Now
};
await _inventoryRepository.AddAsync(newInventory);
```

### **Order Creation**
```csharp
// Tự động giảm tồn kho
inventory.Quantity -= orderItem.Quantity;
```

### **Product Deletion**
```csharp
// Có thể cần xóa inventory trước khi xóa product
await _inventoryRepository.DeleteAsync(inventory.InventoryId);
```

---

## 📝 Testing Checklist

- [ ] Xem danh sách tồn kho → Hiển thị đúng với Product info
- [ ] Tìm kiếm theo tên sản phẩm → Tìm thấy
- [ ] Tìm kiếm theo mã vạch → Tìm thấy
- [ ] Admin cập nhật số lượng → Thành công, UpdatedAt được cập nhật
- [ ] Staff thấy nút "Cập nhật" → KHÔNG (bị ẩn)
- [ ] Staff gọi API UpdateQuantity → Bị từ chối (403)
- [ ] Tạo đơn hàng → Tồn kho tự động giảm
- [ ] Tạo đơn với số lượng > tồn kho → Bị từ chối
- [ ] Sản phẩm mới (chưa có inventory) được bán → Tạo inventory với số âm
- [ ] Phân trang hoạt động đúng

---

## 🔗 Related Files

- `Controllers/InventoryController.cs`
- `Models/Inventory.cs`
- `Views/Inventory/Index.cshtml`
- `Services/OrderService.cs` (Automatic updates)
- `Filters/AdminOnlyAttribute.cs`
