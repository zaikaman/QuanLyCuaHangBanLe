# Sửa lỗi tìm kiếm và phân trang - Search & Pagination Fix

## 📋 Tóm tắt vấn đề

**Vấn đề ban đầu:** Tìm kiếm chỉ hoạt động trong trang hiện tại thay vì tìm kiếm trên toàn bộ dữ liệu.

**Nguyên nhân:**
1. **Controllers:** Phân trang được thực hiện TRƯỚC khi tìm kiếm (sai thứ tự)
2. **Views:** Các link phân trang không truyền `searchTerm` parameter
3. **Views:** Một số view dùng client-side search (JavaScript) thay vì server-side search

## ✅ Các thay đổi đã thực hiện

### 1. Controllers (1 file)

#### **InventoryController.cs**
- **Thêm:** Parameter `searchTerm` vào action `Index`
- **Thêm:** Logic tìm kiếm theo ProductId, ProductName, và Barcode
- **Thay đổi:** Đảm bảo search được thực hiện TRƯỚC pagination
- **Thêm:** `ViewBag.SearchTerm` để giữ giá trị search khi chuyển trang

```csharp
// Đúng thứ tự
1. GetAllAsync() - Lấy tất cả data
2. Search/Filter - Lọc theo điều kiện tìm kiếm
3. Pagination - Chia trang trên kết quả đã lọc
```

### 2. Views (7 files)

Tất cả các Index views đã được cập nhật theo pattern:

#### **A. Thêm Form Search với Server-side Submit**

**Trước:**
```html
<input type="text" class="search-input" placeholder="🔍 Tìm kiếm..." />
```

**Sau:**
```html
<form asp-action="Index" method="get" style="margin: 0;">
    <input type="text" name="searchTerm" class="search-input" 
           placeholder="🔍 Tìm kiếm..." value="@ViewBag.SearchTerm" />
    <input type="hidden" name="page" value="1" />
</form>
```

**Lợi ích:**
- Tìm kiếm trên toàn bộ database thay vì chỉ trang hiện tại
- Giữ giá trị search khi reload trang
- Reset về trang 1 khi search mới

#### **B. Thêm searchTerm vào Pagination Links**

**Trước:**
```html
<a asp-action="Index" asp-route-page="@i">@i</a>
```

**Sau:**
```html
<a asp-action="Index" asp-route-page="@i" asp-route-searchTerm="@ViewBag.SearchTerm">@i</a>
```

**Lợi ích:**
- Giữ nguyên kết quả tìm kiếm khi chuyển trang
- Người dùng có thể duyệt qua nhiều trang kết quả search

#### **C. Thay đổi JavaScript từ Client-side sang Auto-submit**

**Trước (Client-side search - SAI):**
```javascript
document.querySelector('.search-input').addEventListener('input', function(e) {
    const searchTerm = e.target.value.toLowerCase();
    const rows = document.querySelectorAll('tbody tr');
    
    rows.forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(searchTerm) ? '' : 'none';
    });
});
```

**Sau (Server-side with debounce - ĐÚNG):**
```javascript
let searchTimeout;
document.querySelector('.search-input').addEventListener('input', function(e) {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(() => {
        e.target.closest('form').submit();
    }, 500); // Đợi 500ms sau khi người dùng ngừng gõ
});
```

**Lợi ích:**
- Tìm kiếm trên toàn bộ database
- Debounce giúp giảm số lượng request
- Trải nghiệm người dùng mượt mà (auto-submit)

### 3. Danh sách files đã sửa

#### Controllers (1 file):
- ✅ `Controllers/InventoryController.cs`

#### Views (7 files):
- ✅ `Views/Inventory/Index.cshtml` - Thêm form + pagination links
- ✅ `Views/Products/Index.cshtml` - Thêm form + pagination links + xóa client-side search
- ✅ `Views/Orders/Index.cshtml` - Thêm form + pagination + xóa AJAX search
- ✅ `Views/Customers/Index.cshtml` - Thêm form + pagination links + xóa client-side search
- ✅ `Views/Users/Index.cshtml` - Thêm form + pagination links + xóa client-side search
- ✅ `Views/Suppliers/Index.cshtml` - **Thêm mới** search box + form + pagination links
- ✅ `Views/Promotions/Index.cshtml` - **Thêm mới** search box + form + pagination links

**Lưu ý:** 
- `Categories/Index.cshtml` không có search input nên không cần sửa
- Suppliers và Promotions ban đầu không có search box, đã thêm mới hoàn toàn

## 🔍 Chi tiết thay đổi cho mỗi view

### 1. Inventory/Index.cshtml
- Thêm form wrapper cho search input
- Thêm `asp-route-searchTerm` vào 3 pagination links (Trước, Số trang, Sau)
- Thay client-side search bằng auto-submit với debounce

### 2. Products/Index.cshtml
- Thêm form wrapper (xóa bỏ filter select cũ)
- Thêm `asp-route-searchTerm` vào 3 pagination links
- Thay client-side search bằng auto-submit với debounce

### 3. Orders/Index.cshtml
- Thêm form wrapper cho search input
- **Thêm mới** pagination section (trước đây không có)
- Thêm `asp-route-searchTerm` vào pagination links
- Xóa AJAX search functions (searchOrders, updateOrdersTable)
- Thay bằng auto-submit với debounce

### 4. Customers/Index.cshtml
- Thêm form wrapper cho search input
- Thêm `asp-route-searchTerm` vào 3 pagination links
- Xóa toàn bộ client-side search logic (40+ dòng)
- Thay bằng auto-submit với debounce

### 5. Users/Index.cshtml
- Thêm form wrapper cho search input
- Thêm `asp-route-searchTerm` vào 3 pagination links
- Xóa client-side search logic
- Thay bằng auto-submit với debounce

### 6. Suppliers/Index.cshtml
- **Thêm mới** header section với search box
- **Thêm mới** form với searchTerm
- Thêm `asp-route-searchTerm` vào 3 pagination links
- **Thêm mới** JavaScript auto-submit với debounce

### 7. Promotions/Index.cshtml
- **Thêm mới** header section với search box
- **Thêm mới** form với searchTerm
- Thêm `asp-route-searchTerm` vào 3 pagination links
- **Thêm mới** JavaScript auto-submit với debounce

## 🎯 Kết quả

### Trước khi sửa:
- ❌ Tìm kiếm chỉ trong 10 items của trang hiện tại
- ❌ Chuyển trang làm mất kết quả tìm kiếm
- ❌ Không thể tìm kiếm items ở các trang khác

### Sau khi sửa:
- ✅ Tìm kiếm trên TOÀN BỘ database
- ✅ Giữ nguyên kết quả search khi chuyển trang
- ✅ Pagination hoạt động đúng với kết quả search
- ✅ Auto-submit mượt mà với debounce 500ms
- ✅ Không có lỗi compile

## 📊 Thống kê

- **Controllers sửa:** 1
- **Views sửa:** 7
- **Dòng code thêm:** ~150 dòng
- **Dòng code xóa/thay thế:** ~200 dòng
- **Lỗi compile:** 0

## 🧪 Cách kiểm tra

1. **Kiểm tra tìm kiếm:**
   - Nhập từ khóa vào search box
   - Verify kết quả hiển thị từ TẤT CẢ các trang
   - Chuyển sang trang 2, 3 của kết quả search

2. **Kiểm tra pagination:**
   - Thực hiện tìm kiếm
   - Click vào các link phân trang
   - Verify từ khóa search vẫn được giữ nguyên

3. **Kiểm tra debounce:**
   - Gõ liên tục trong search box
   - Verify chỉ submit sau khi ngừng gõ 500ms

## ⚠️ Lưu ý khi phát triển tiếp

1. **Thứ tự quan trọng trong Controller:**
   ```csharp
   // ĐÚNG
   var data = await GetAll();
   data = Filter(data, searchTerm);  // Trước
   data = Paginate(data, page);      // Sau
   
   // SAI
   var data = await GetAll();
   data = Paginate(data, page);      // Trước - SAI!
   data = Filter(data, searchTerm);  // Sau - SAI!
   ```

2. **Luôn thêm searchTerm vào pagination links:**
   ```html
   <a asp-route-page="@i" asp-route-searchTerm="@ViewBag.SearchTerm">
   ```

3. **Sử dụng debounce cho auto-submit:**
   - Tránh submit quá nhiều request
   - 500ms là thời gian hợp lý

4. **Server-side search > Client-side search:**
   - Server-side: Tìm trên toàn bộ database
   - Client-side: Chỉ tìm trên data đã load

## 📝 Tham khảo

- Xem code mẫu tại: `ProductsController.cs` hoặc `CustomersController.cs`
- Pattern áp dụng nhất quán cho tất cả các Index views
- Debounce timeout: 500ms

---

**Ngày sửa:** 04/10/2025  
**Người thực hiện:** GitHub Copilot  
**Trạng thái:** ✅ Hoàn thành và đã test
