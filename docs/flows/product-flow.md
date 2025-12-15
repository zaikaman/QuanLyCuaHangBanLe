# Product Management Flow - Luồng Quản Lý Sản Phẩm

## 📋 Tổng Quan

Module Products quản lý toàn bộ sản phẩm trong cửa hàng, bao gồm thông tin chi tiết, hình ảnh, giá cả, mã vạch, và liên kết với danh mục và nhà cung cấp.

## 🏗️ Kiến Trúc

### **Components**
- **Controller**: `ProductsController`
- **Service**: `ProductService` (extends `GenericRepository<Product>`)
- **Cloud Service**: `CloudinaryService`
- **Model**: `Product`
- **Related Models**: `Category`, `Supplier`, `Inventory`

### **Product Properties**
```csharp
- ProductId (int, PK, Auto)
- ProductName (string, required)
- Barcode (string, unique, nullable)
- CategoryId (int, FK → Categories)
- SupplierId (int, FK → Suppliers)
- Price (decimal, required)
- Unit (string, VD: "Cái", "Hộp", "Kg")
- ImageUrl (string, nullable)
- CreatedAt (DateTime)

// Navigation Properties
- Category
- Supplier
- Inventory (1-1 relationship)
- OrderItems (1-many)
```

---

## 🔄 Luồng Hoạt Động

### **1. Xem Danh Sách Sản Phẩm (Index)**

#### **Flow Diagram**
```
GET /Products/Index?page=1&searchTerm=...
    ↓
Kiểm tra session (đã đăng nhập?)
    ↓
ProductService.GetAllAsync()
    ├─ Include Category
    ├─ Include Supplier
    └─ Include Inventory
    ↓
Áp dụng Search Filter (nếu có)
    ├─ ProductName contains searchTerm
    ├─ Barcode contains searchTerm
    ├─ Category.CategoryName contains searchTerm
    └─ Supplier.Name contains searchTerm
    ↓
Phân trang (pageSize = 10)
    ↓
Trả về View với:
    ├─ List<Product> (10 items)
    ├─ CurrentPage
    ├─ TotalPages
    ├─ TotalItems
    └─ SearchTerm
```

#### **Code Flow**
```csharp
public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
{
    // 1. Kiểm tra authentication
    var username = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(username)) {
        return RedirectToAction("Login", "Auth");
    }

    // 2. Lấy tất cả sản phẩm (với eager loading)
    const int pageSize = 10;
    var allProducts = await _productService.GetAllAsync();
    
    // 3. Tìm kiếm (nếu có)
    if (!string.IsNullOrWhiteSpace(searchTerm)) {
        searchTerm = searchTerm.Trim().ToLower();
        allProducts = allProducts.Where(p =>
            (p.ProductName != null && p.ProductName.ToLower().Contains(searchTerm)) ||
            (p.Barcode != null && p.Barcode.ToLower().Contains(searchTerm)) ||
            (p.Category?.CategoryName != null && 
             p.Category.CategoryName.ToLower().Contains(searchTerm)) ||
            (p.Supplier?.Name != null && 
             p.Supplier.Name.ToLower().Contains(searchTerm))
        ).ToList();
    }
    
    // 4. Tính toán phân trang
    var totalItems = allProducts.Count();
    var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

    // 5. Lấy dữ liệu cho trang hiện tại
    var products = allProducts
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToList();

    // 6. Truyền dữ liệu qua ViewBag
    ViewBag.CurrentPage = page;
    ViewBag.TotalPages = totalPages;
    ViewBag.TotalItems = totalItems;
    ViewBag.SearchTerm = searchTerm;

    return View(products);
}
```

#### **ProductService.GetAllAsync (Override)**
```csharp
public override async Task<IEnumerable<Product>> GetAllAsync()
{
    return await _dbSet
        .AsNoTracking()  // Không track để tránh conflict
        .Include(p => p.Category)   // Eager loading
        .Include(p => p.Supplier)
        .Include(p => p.Inventory)
        .ToListAsync();
}
```

---

### **2. Xem Chi Tiết Sản Phẩm (Details)**

#### **Flow Diagram**
```
GET /Products/Details/{id}
    ↓
Kiểm tra session
    ↓
ProductService.GetByIdAsync(id)
    ├─ Include Category
    ├─ Include Supplier
    └─ Include Inventory
    ↓
Sản phẩm tồn tại?
    ├─ KHÔNG → Return NotFound()
    └─ CÓ → Return View(product)
```

#### **Code Flow**
```csharp
public async Task<IActionResult> Details(int id)
{
    var username = HttpContext.Session.GetString("Username");
    if (string.IsNullOrEmpty(username)) {
        return RedirectToAction("Login", "Auth");
    }

    var product = await _productService.GetByIdAsync(id);
    if (product == null) {
        return NotFound();
    }
    return View(product);
}
```

---

### **3. Thêm Sản Phẩm Mới (Create)** - ADMIN ONLY

#### **Flow Diagram**
```
GET /Products/Create
    ↓
[AdminOnly] - Kiểm tra role
    ↓
LoadDropdownData()
    ├─ Categories → ViewBag.Categories
    └─ Suppliers → ViewBag.Suppliers
    ↓
Return View (empty Product)

====================

POST /Products/Create
    ↓
[AdminOnly] + [ValidateAntiForgeryToken]
    ↓
Kiểm tra Barcode trùng lặp (nếu có)
    ↓
ModelState.IsValid?
    ↓
Upload ảnh lên Cloudinary (nếu có)
    ├─ Validate file type
    ├─ Upload
    ├─ Lấy URL
    └─ Set product.ImageUrl
    ↓
product.CreatedAt = DateTime.Now
    ↓
ProductService.AddAsync(product)
    ↓
TempData["Success"] = "Thêm sản phẩm thành công!"
    ↓
Redirect → /Products/Index
```

#### **Code Flow - Create [GET]**
```csharp
[AdminOnly]
public async Task<IActionResult> Create()
{
    await LoadDropdownData();
    return View();
}

private async Task LoadDropdownData()
{
    var categories = await _categoryRepository.GetAllAsync();
    var suppliers = await _supplierRepository.GetAllAsync();
    
    ViewBag.Categories = new SelectList(
        categories, 
        "CategoryId", 
        "CategoryName"
    );
    ViewBag.Suppliers = new SelectList(
        suppliers, 
        "SupplierId", 
        "Name"
    );
}
```

#### **Code Flow - Create [POST]**
```csharp
[AdminOnly]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
{
    // 1. KIỂM TRA BARCODE TRÙNG LẶP
    if (!string.IsNullOrWhiteSpace(product.Barcode)) {
        var allProducts = await _productService.GetAllAsync();
        var existingProduct = allProducts.FirstOrDefault(p => 
            p.Barcode != null && 
            p.Barcode.Equals(product.Barcode, StringComparison.OrdinalIgnoreCase)
        );
        
        if (existingProduct != null) {
            ModelState.AddModelError("Barcode", 
                $"Mã vạch '{product.Barcode}' đã được sử dụng cho sản phẩm '{existingProduct.ProductName}'"
            );
        }
    }

    if (ModelState.IsValid) {
        try {
            // 2. UPLOAD ẢNH (NẾU CÓ)
            if (imageFile != null && imageFile.Length > 0) {
                try {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(
                        imageFile, 
                        "products"
                    );
                    product.ImageUrl = imageUrl;
                }
                catch (InvalidOperationException ex) {
                    ModelState.AddModelError("ImageFile", ex.Message);
                    await LoadDropdownData();
                    return View(product);
                }
            }

            // 3. THÊM SẢN PHẨM
            product.CreatedAt = DateTime.Now;
            await _productService.AddAsync(product);
            
            TempData["Success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("Index");
        }
        catch (Exception ex) {
            // Xử lý lỗi duplicate từ database
            if (ex.InnerException?.Message.Contains("Duplicate entry") == true) {
                if (ex.InnerException.Message.Contains("barcode")) {
                    ModelState.AddModelError("Barcode", 
                        "Mã vạch này đã tồn tại trong hệ thống"
                    );
                }
                else {
                    ModelState.AddModelError("", "Dữ liệu bị trùng lặp");
                }
            }
            else {
                ModelState.AddModelError("", "Lỗi khi thêm sản phẩm: " + ex.Message);
            }
        }
    }
    
    // Reload dropdown nếu có lỗi
    await LoadDropdownData();
    return View(product);
}
```

#### **CloudinaryService.UploadImageAsync**
```csharp
public async Task<string> UploadImageAsync(IFormFile file, string folder)
{
    // 1. Validate file type
    var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/gif" };
    if (!allowedTypes.Contains(file.ContentType.ToLower())) {
        throw new InvalidOperationException(
            "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif)"
        );
    }

    // 2. Upload lên Cloudinary
    using var stream = file.OpenReadStream();
    var uploadParams = new ImageUploadParams {
        File = new FileDescription(file.FileName, stream),
        Folder = folder
    };

    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
    
    // 3. Kiểm tra kết quả
    if (uploadResult.Error != null) {
        throw new Exception("Lỗi upload ảnh: " + uploadResult.Error.Message);
    }

    return uploadResult.SecureUrl.ToString();
}
```

#### **Validation Rules**
| Field | Rules |
|-------|-------|
| ProductName | - Required<br>- MaxLength: 255 |
| Barcode | - Unique<br>- Case-insensitive |
| CategoryId | - Required<br>- Must exist in Categories |
| SupplierId | - Required<br>- Must exist in Suppliers |
| Price | - Required<br>- Must be > 0 |
| Unit | - Required |
| ImageUrl | - Optional<br>- Valid image types only |

---

### **4. Sửa Sản Phẩm (Edit)** - ADMIN ONLY

#### **Flow Diagram**
```
GET /Products/Edit/{id}
    ↓
[AdminOnly]
    ↓
ProductService.GetByIdAsync(id)
    ↓
Sản phẩm tồn tại?
    ├─ KHÔNG → Return NotFound()
    └─ CÓ → LoadDropdownData() → Return View(product)

====================

POST /Products/Edit/{id}
    ↓
[AdminOnly] + [ValidateAntiForgeryToken]
    ↓
id == product.ProductId?
    ↓
Kiểm tra Barcode trùng (loại trừ chính nó)
    ↓
Upload ảnh mới (nếu có)
    ├─ Giữ ảnh cũ nếu không upload mới
    └─ Xóa ảnh cũ trên Cloudinary (nếu thay thế)
    ↓
ProductService.UpdateAsync(product)
    ↓
TempData["Success"] = "Cập nhật sản phẩm thành công!"
    ↓
Redirect → /Products/Index
```

#### **Code Flow - Edit [POST]**
```csharp
[AdminOnly]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
{
    // 1. KIỂM TRA ID KHỚP
    if (id != product.ProductId) {
        TempData["Error"] = "Dữ liệu không khớp";
        return RedirectToAction("Index");
    }

    // 2. LẤY SẢN PHẨM GỐC
    var existingProduct = await _productService.GetByIdAsync(id);
    if (existingProduct == null) {
        return NotFound();
    }

    // 3. KIỂM TRA BARCODE TRÙNG (LOẠI TRỪ CHÍNH NÓ)
    if (!string.IsNullOrWhiteSpace(product.Barcode)) {
        var allProducts = await _productService.GetAllAsync();
        var duplicateProduct = allProducts.FirstOrDefault(p =>
            p.ProductId != id &&
            p.Barcode != null &&
            p.Barcode.Equals(product.Barcode, StringComparison.OrdinalIgnoreCase)
        );

        if (duplicateProduct != null) {
            ModelState.AddModelError("Barcode", 
                $"Mã vạch '{product.Barcode}' đã được sử dụng"
            );
        }
    }

    if (ModelState.IsValid) {
        try {
            // 4. XỬ LÝ ẢNH
            if (imageFile != null && imageFile.Length > 0) {
                // Upload ảnh mới
                var imageUrl = await _cloudinaryService.UploadImageAsync(
                    imageFile, 
                    "products"
                );
                product.ImageUrl = imageUrl;
            }
            else {
                // Giữ ảnh cũ
                product.ImageUrl = existingProduct.ImageUrl;
            }

            // 5. GIỮ NGUYÊN CreatedAt
            product.CreatedAt = existingProduct.CreatedAt;

            // 6. CẬP NHẬT
            await _productService.UpdateAsync(product);
            
            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction("Index");
        }
        catch (Exception ex) {
            ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
        }
    }

    await LoadDropdownData();
    return View(product);
}
```

---

### **5. Xóa Sản Phẩm (Delete)** - ADMIN ONLY

#### **Flow Diagram**
```
GET /Products/Delete/{id}
    ↓
[AdminOnly]
    ↓
ProductService.GetByIdAsync(id)
    ↓
Sản phẩm tồn tại?
    ├─ KHÔNG → Return NotFound()
    └─ CÓ → Return View(product)

====================

POST /Products/Delete/{id}
    ↓
[AdminOnly] + [ValidateAntiForgeryToken]
    ↓
Kiểm tra sản phẩm có trong OrderItems?
    ├─ CÓ → TempData["Error"] = "Không thể xóa..."
    └─ KHÔNG → Xóa được
    ↓
ProductService.DeleteAsync(id)
    ↓
TempData["Success"] = "Xóa sản phẩm thành công!"
    ↓
Redirect → /Products/Index
```

#### **Code Flow - Delete [POST]**
```csharp
[AdminOnly]
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    try {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) {
            return NotFound();
        }

        // Kiểm tra ràng buộc dữ liệu
        var (canDelete, message) = await _productService.CanDeleteProductAsync(id);
        
        if (!canDelete) {
            TempData["Error"] = message;
            return RedirectToAction("Index");
        }

        await _productService.DeleteAsync(id);
        TempData["Success"] = "Xóa sản phẩm thành công!";
        return RedirectToAction("Index");
    }
    catch (Exception ex) {
        TempData["Error"] = "Lỗi khi xóa sản phẩm: " + ex.Message;
        return RedirectToAction("Index");
    }
}
```

#### **ProductService.CanDeleteProductAsync**
```csharp
public async Task<(bool CanDelete, string Message)> CanDeleteProductAsync(int productId)
{
    // Kiểm tra sản phẩm có trong order_items không
    var hasOrders = await _context.OrderItems
        .AnyAsync(oi => oi.ProductId == productId);
    
    if (hasOrders) {
        return (false, "Không thể xóa sản phẩm đã có trong đơn hàng");
    }

    return (true, "");
}
```

---

## 📊 Search & Filter

### **Search Implementation**
```csharp
// Tìm kiếm theo nhiều trường
searchTerm = searchTerm.Trim().ToLower();
allProducts = allProducts.Where(p =>
    // Tên sản phẩm
    (p.ProductName != null && 
     p.ProductName.ToLower().Contains(searchTerm)) ||
    
    // Mã vạch
    (p.Barcode != null && 
     p.Barcode.ToLower().Contains(searchTerm)) ||
    
    // Tên danh mục
    (p.Category?.CategoryName != null && 
     p.Category.CategoryName.ToLower().Contains(searchTerm)) ||
    
    // Tên nhà cung cấp
    (p.Supplier?.Name != null && 
     p.Supplier.Name.ToLower().Contains(searchTerm))
).ToList();
```

### **Pagination**
```csharp
const int pageSize = 10;
var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

var products = allProducts
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToList();
```

---

## 🖼️ Image Management

### **Upload Process**
```
1. User chọn file
    ↓
2. Validate file type (jpg, jpeg, png, gif)
    ↓
3. Upload lên Cloudinary
    ├─ Folder: "products"
    └─ Get secure URL
    ↓
4. Lưu URL vào database
```

### **Supported Image Types**
- `image/jpeg`
- `image/jpg`
- `image/png`
- `image/gif`

### **Error Handling**
```csharp
try {
    var imageUrl = await _cloudinaryService.UploadImageAsync(imageFile, "products");
    product.ImageUrl = imageUrl;
}
catch (InvalidOperationException ex) {
    // File type không hợp lệ
    ModelState.AddModelError("ImageFile", ex.Message);
}
catch (Exception ex) {
    // Lỗi upload
    ModelState.AddModelError("ImageFile", "Lỗi upload ảnh: " + ex.Message);
}
```

---

## 🔗 Related Entities

### **Product → Category (Many-to-One)**
```csharp
// Eager loading
.Include(p => p.Category)

// Usage
product.Category.CategoryName
```

### **Product → Supplier (Many-to-One)**
```csharp
// Eager loading
.Include(p => p.Supplier)

// Usage
product.Supplier.Name
```

### **Product → Inventory (One-to-One)**
```csharp
// Eager loading
.Include(p => p.Inventory)

// Usage
product.Inventory?.Quantity ?? 0
```

---

## 🐛 Error Handling

### **Common Errors**
```csharp
// Barcode trùng lặp
ModelState.AddModelError("Barcode", "Mã vạch này đã tồn tại trong hệ thống");

// Upload ảnh thất bại
ModelState.AddModelError("ImageFile", "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif)");

// Không thể xóa
TempData["Error"] = "Không thể xóa sản phẩm đã có trong đơn hàng";

// Cập nhật thành công
TempData["Success"] = "Cập nhật sản phẩm thành công!";
```

---

## 📝 Testing Checklist

- [ ] Xem danh sách sản phẩm với phân trang
- [ ] Tìm kiếm theo tên sản phẩm
- [ ] Tìm kiếm theo mã vạch
- [ ] Tìm kiếm theo tên danh mục
- [ ] Tìm kiếm theo tên nhà cung cấp
- [ ] Thêm sản phẩm mới với ảnh
- [ ] Thêm sản phẩm không có ảnh
- [ ] Thêm sản phẩm với mã vạch trùng → Hiện lỗi
- [ ] Sửa sản phẩm giữ nguyên ảnh cũ
- [ ] Sửa sản phẩm thay ảnh mới
- [ ] Xóa sản phẩm chưa có trong đơn hàng → Thành công
- [ ] Xóa sản phẩm đã có trong đơn hàng → Bị từ chối
- [ ] Staff truy cập Create/Edit/Delete → Bị từ chối

---

## 🔗 Related Files

- `Controllers/ProductsController.cs`
- `Services/ProductService.cs`
- `Services/CloudinaryService.cs`
- `Models/Product.cs`
- `Views/Products/Index.cshtml`
- `Views/Products/Create.cshtml`
- `Views/Products/Edit.cshtml`
- `Views/Products/Delete.cshtml`
- `Views/Products/Details.cshtml`
