# Sửa lỗi 404 - Promotions Delete

## 🐛 Vấn đề

**Lỗi:** `No webpage was found for the web address: http://localhost:5166/Promotions/Delete/6`

**Nguyên nhân:**
- View `Promotions/Index.cshtml` sử dụng `<a asp-action="Delete">` tạo GET request
- Controller `PromotionsController.cs` chỉ có action Delete với `[HttpPost]`
- Không có GET version của Delete action
- Không có view `Delete.cshtml`

## ✅ Giải pháp

Thay đổi cách xóa từ link GET sang button với AJAX POST request.

## 🔧 Các thay đổi

### 1. Sửa HTML button trong `Views/Promotions/Index.cshtml`

**Trước (SAI - dùng GET link):**
```html
<div class="promo-actions">
    <a asp-action="Delete" asp-route-id="@promo.PromoId" 
       class="btn-action btn-delete" 
       onclick="return confirm('Bạn có chắc chắn muốn xóa khuyến mãi này?')">
        🗑️ Xóa
    </a>
</div>
```

**Sau (ĐÚNG - dùng button với JavaScript):**
```html
<div class="promo-actions">
    <a asp-action="Details" asp-route-id="@promo.PromoId" 
       class="btn-action">
        👁️ Xem
    </a>
    <a asp-action="Edit" asp-route-id="@promo.PromoId" 
       class="btn-action">
        ✏️ Sửa
    </a>
    <button type="button" 
            class="btn-action btn-delete" 
            onclick="deletePromotion(@promo.PromoId, '@promo.PromoCode')">
        🗑️ Xóa
    </button>
</div>
```

**Thay đổi:**
- ✅ Thêm nút "Xem" để xem chi tiết
- ✅ Thêm nút "Sửa" để chỉnh sửa
- ✅ Đổi `<a>` thành `<button>` cho nút Xóa
- ✅ Gọi JavaScript function `deletePromotion()` thay vì navigate trực tiếp
- ✅ Truyền cả ID và PromoCode vào function

### 2. Thêm JavaScript function trong `Views/Promotions/Index.cshtml`

**Thêm function xử lý xóa:**
```javascript
// Delete promotion function
async function deletePromotion(id, promoCode) {
    if (!confirm(`Bạn có chắc chắn muốn xóa khuyến mãi "${promoCode}" không?`)) {
        return;
    }

    try {
        // Get anti-forgery token
        const token = document.querySelector('input[name="__RequestVerificationToken"]');
        
        const formData = new FormData();
        formData.append('id', id);
        if (token) {
            formData.append('__RequestVerificationToken', token.value);
        }

        const response = await fetch('@Url.Action("Delete", "Promotions")', {
            method: 'POST',
            body: formData
        });

        const result = await response.json();

        if (result.success) {
            alert(result.message);
            location.reload(); // Reload để cập nhật danh sách
        } else {
            alert('Lỗi: ' + result.message);
        }
    } catch (error) {
        alert('Lỗi khi xóa: ' + error.message);
    }
}
```

**Tính năng:**
- ✅ Hiển thị confirm dialog với tên khuyến mãi
- ✅ Gửi POST request với anti-forgery token
- ✅ Xử lý response JSON từ server
- ✅ Hiển thị thông báo thành công/lỗi
- ✅ Reload trang sau khi xóa thành công

## 📋 Controller không cần thay đổi

Controller `PromotionsController.cs` đã có action Delete đúng:

```csharp
[HttpPost]
public async Task<IActionResult> Delete(int id)
{
    try
    {
        await _promotionRepository.DeleteAsync(id);
        return Json(new { success = true, message = "Xóa khuyến mãi thành công!" });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = "Lỗi: " + ex.Message });
    }
}
```

**Lưu ý:** 
- Action chỉ accept POST request
- Trả về JSON response (không phải View)
- Phù hợp với AJAX call

## 🎨 CSS

CSS cho các button đã có sẵn trong view:

```css
.promo-actions {
    display: flex;
    gap: 10px;
    margin-top: 15px;
    padding-top: 15px;
    border-top: 1px solid #e2e8f0;
}

.btn-action {
    padding: 8px 16px;
    background: #f7fafc;
    color: #2d3748;
    border-radius: 8px;
    border: none;
    font-size: 0.85rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s ease;
    text-decoration: none;
    text-align: center;
}

.btn-delete {
    background: #fed7d7 !important;
    color: #c53030 !important;
    border: 2px solid transparent !important;
}

.btn-delete:hover {
    background: #ffffff !important;
    border: 2px solid #c53030 !important;
    color: #c53030 !important;
}
```

## ✨ Cải tiến

### 1. User Experience
- Hiển thị tên khuyến mãi trong confirm dialog
- Alert thông báo rõ ràng về kết quả
- Auto reload trang sau khi xóa thành công

### 2. Thêm nút Xem & Sửa
- Người dùng giờ có thể xem chi tiết và chỉnh sửa từ Index
- Không cần navigate riêng

### 3. Security
- Sử dụng anti-forgery token
- POST request thay vì GET (đúng RESTful convention)

## 🧪 Cách test

1. **Navigate đến trang Promotions:**
   ```
   http://localhost:5166/Promotions
   ```

2. **Click nút "🗑️ Xóa" trên một promotion:**
   - Verify confirm dialog hiển thị đúng tên
   - Click OK

3. **Verify kết quả:**
   - Alert hiển thị "Xóa khuyến mãi thành công!"
   - Trang reload và promotion đã bị xóa
   - Không có lỗi 404

4. **Test các button khác:**
   - Click "👁️ Xem" - navigate đến Details
   - Click "✏️ Sửa" - navigate đến Edit

## 📝 Pattern áp dụng

Pattern này có thể áp dụng cho các controller/view khác:

**Khi nào dùng DELETE với POST + AJAX:**
- ✅ Khi controller action chỉ có `[HttpPost]`
- ✅ Khi action trả về JSON
- ✅ Khi không có view riêng cho Delete
- ✅ Khi muốn xóa mà không rời khỏi trang

**Khi nào dùng DELETE với GET + View:**
- ❌ Khi cần trang confirm riêng với nhiều thông tin
- ❌ Khi có logic phức tạp cần hiển thị trước khi xóa

## 🔍 Tương tự các files khác

Kiểm tra các controller/view khác xem có vấn đề tương tự:
- `Products/Index.cshtml` - OK (có view Delete.cshtml)
- `Customers/Index.cshtml` - OK (có view Delete.cshtml)
- `Users/Index.cshtml` - Cần kiểm tra
- `Suppliers/Index.cshtml` - Cần kiểm tra

---

**Ngày sửa:** 04/10/2025  
**Lỗi:** 404 Not Found khi Delete Promotion  
**Trạng thái:** ✅ Đã sửa xong
