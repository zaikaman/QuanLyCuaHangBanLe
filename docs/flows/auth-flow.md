# Authentication Flow - Luồng Xác Thực

## 📋 Tổng Quan

Module Authentication quản lý toàn bộ quy trình đăng nhập, đăng xuất và đổi mật khẩu của hệ thống. Sử dụng Session-based authentication để lưu trạng thái người dùng.

## 🏗️ Kiến Trúc

### **Components**
- **Controller**: `AuthController`
- **Service**: `AuthService` (implements `IAuthService`)
- **Model**: `User`
- **Views**: 
  - `Login.cshtml`
  - `ChangePassword.cshtml`

### **Session Keys**
```csharp
- "Username"    // Tên đăng nhập
- "FullName"    // Họ tên đầy đủ
- "Role"        // Vai trò: "admin" hoặc "staff"
- "UserId"      // ID người dùng (int)
```

## 🔄 Luồng Hoạt Động

### **1. Đăng Nhập (Login)**

#### **Flow Diagram**
```
User nhập thông tin
    ↓
Validation phía client (HTML5)
    ↓
POST /Auth/Login
    ↓
Validation phía server
    ├─ Username: rỗng? < 3 ký tự? > 50 ký tự?
    ├─ Password: rỗng? < 6 ký tự?
    └─ Có lỗi? → Hiện thông báo lỗi
    ↓
AuthService.AuthenticateAsync()
    ↓
Query database: username & password khớp?
    ├─ KHÔNG → Thông báo "Sai tên đăng nhập hoặc mật khẩu"
    └─ CÓ → Lấy thông tin User
    ↓
Lưu thông tin vào Session
    ├─ Username
    ├─ FullName
    ├─ Role
    └─ UserId
    ↓
Redirect → /Dashboard/Index
```

#### **Code Flow - AuthController.Login [POST]**

```csharp
// 1. VALIDATION ĐẦU VÀO
var errors = new List<string>();

if (string.IsNullOrWhiteSpace(username))
    errors.Add("Tên đăng nhập không được để trống");
else if (username.Length < 3)
    errors.Add("Tên đăng nhập phải có ít nhất 3 ký tự");
else if (username.Length > 50)
    errors.Add("Tên đăng nhập không được vượt quá 50 ký tự");

if (string.IsNullOrWhiteSpace(password))
    errors.Add("Mật khẩu không được để trống");
else if (password.Length < 6)
    errors.Add("Mật khẩu phải có ít nhất 6 ký tự");

// 2. TRẢ VỀ LỖI NẾU CÓ
if (errors.Any()) {
    ViewBag.Error = string.Join(", ", errors);
    return View();
}

// 3. XÁC THỰC
var user = await _authService.AuthenticateAsync(username.Trim(), password);

// 4. KIỂM TRA KẾT QUẢ
if (user != null) {
    // Lưu session
    HttpContext.Session.SetString("Username", user.Username);
    HttpContext.Session.SetString("FullName", user.FullName ?? "");
    HttpContext.Session.SetString("Role", user.Role);
    HttpContext.Session.SetInt32("UserId", user.UserId);
    
    // Redirect đến Dashboard
    return RedirectToAction("Index", "Dashboard");
}

// 5. THẤT BẠI
ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
return View();
```

#### **Code Flow - AuthService.AuthenticateAsync**

```csharp
public async Task<User?> AuthenticateAsync(string username, string password)
{
    // Query database tìm user với username và password khớp
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
    
    return user;  // null nếu không tìm thấy
}
```

#### **Validation Rules**
| Field | Rules |
|-------|-------|
| Username | - Không được rỗng<br>- Tối thiểu 3 ký tự<br>- Tối đa 50 ký tự<br>- Trim whitespace |
| Password | - Không được rỗng<br>- Tối thiểu 6 ký tự |

---

### **2. Đăng Xuất (Logout)**

#### **Flow Diagram**
```
User click "Đăng xuất"
    ↓
GET /Auth/Logout
    ↓
HttpContext.Session.Clear()
    ↓
Redirect → /Auth/Login
```

#### **Code Flow**
```csharp
public IActionResult Logout()
{
    // Xóa toàn bộ session
    HttpContext.Session.Clear();
    
    // Redirect về trang login
    return RedirectToAction("Login");
}
```

---

### **3. Đổi Mật Khẩu (Change Password)**

#### **Flow Diagram**
```
User đã đăng nhập
    ↓
GET /Auth/ChangePassword
    ↓
Kiểm tra session có Username?
    ├─ KHÔNG → Redirect /Auth/Login
    └─ CÓ → Hiển thị form đổi mật khẩu
    ↓
User nhập thông tin
    ├─ Mật khẩu cũ
    ├─ Mật khẩu mới
    └─ Xác nhận mật khẩu mới
    ↓
POST /Auth/ChangePassword
    ↓
Validation
    ├─ Các trường có rỗng?
    ├─ Mật khẩu mới >= 6 ký tự?
    └─ Mật khẩu mới == Xác nhận?
    ↓
AuthService.ChangePasswordAsync()
    ├─ Tìm user theo UserId
    ├─ Kiểm tra mật khẩu cũ đúng?
    ├─ Cập nhật mật khẩu mới
    └─ SaveChanges
    ↓
Thành công
    ├─ TempData["Success"]
    └─ Redirect → /Dashboard/Index
```

#### **Code Flow - AuthController.ChangePassword [POST]**

```csharp
// 1. KIỂM TRA SESSION
var username = HttpContext.Session.GetString("Username");
var userId = HttpContext.Session.GetInt32("UserId");

if (string.IsNullOrEmpty(username) || userId == null) {
    return RedirectToAction("Login");
}

// 2. VALIDATION
var errors = new List<string>();

if (string.IsNullOrWhiteSpace(oldPassword))
    errors.Add("Mật khẩu cũ không được để trống");

if (string.IsNullOrWhiteSpace(newPassword))
    errors.Add("Mật khẩu mới không được để trống");
else if (newPassword.Length < 6)
    errors.Add("Mật khẩu mới phải có ít nhất 6 ký tự");

if (string.IsNullOrWhiteSpace(confirmPassword))
    errors.Add("Xác nhận mật khẩu không được để trống");

if (!string.IsNullOrWhiteSpace(newPassword) && 
    !string.IsNullOrWhiteSpace(confirmPassword)) {
    if (newPassword != confirmPassword)
        errors.Add("Mật khẩu mới và xác nhận mật khẩu không khớp");
}

if (errors.Any()) {
    TempData["Error"] = string.Join(", ", errors);
    return View();
}

// 3. ĐỔI MẬT KHẨU
var result = await _authService.ChangePasswordAsync(
    userId.Value, 
    oldPassword, 
    newPassword
);

// 4. XỬ LÝ KẾT QUẢ
if (result) {
    TempData["Success"] = "Đổi mật khẩu thành công!";
    return RedirectToAction("Index", "Dashboard");
} else {
    TempData["Error"] = "Mật khẩu cũ không đúng";
    return View();
}
```

#### **Code Flow - AuthService.ChangePasswordAsync**

```csharp
public async Task<bool> ChangePasswordAsync(
    int userId, 
    string oldPassword, 
    string newPassword)
{
    try {
        // 1. Tìm user
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        // 2. Kiểm tra mật khẩu cũ
        if (user.Password != oldPassword) return false;

        // 3. Cập nhật mật khẩu mới
        user.Password = newPassword;
        await _context.SaveChangesAsync();
        
        return true;
    }
    catch {
        return false;
    }
}
```

#### **Validation Rules**
| Field | Rules |
|-------|-------|
| Mật khẩu cũ | - Không được rỗng<br>- Phải khớp với DB |
| Mật khẩu mới | - Không được rỗng<br>- Tối thiểu 6 ký tự |
| Xác nhận mật khẩu | - Không được rỗng<br>- Phải khớp với mật khẩu mới |

---

## 🔒 Authorization - Phân Quyền

### **SessionAuthorizationFilter**
Filter tự động áp dụng cho tất cả controllers (trừ AuthController)

```csharp
public override void OnActionExecuting(ActionExecutingContext context)
{
    var username = context.HttpContext.Session.GetString("Username");
    
    if (string.IsNullOrEmpty(username)) {
        // Chưa đăng nhập → Redirect về Login
        context.Result = new RedirectToActionResult("Login", "Auth", null);
    }
}
```

### **AdminOnlyAttribute**
Filter áp dụng cho các action chỉ Admin mới được truy cập

```csharp
public override void OnActionExecuting(ActionExecutingContext context)
{
    var role = context.HttpContext.Session.GetString("Role");
    
    if (role != "admin") {
        // Không phải Admin → Từ chối truy cập
        context.HttpContext.Session.SetString(
            "ErrorMessage", 
            "Bạn không có quyền truy cập trang này"
        );
        context.Result = new RedirectToActionResult("Index", "Dashboard", null);
    }
}
```

### **Áp Dụng Authorization**

```csharp
// Tất cả controllers đều yêu cầu đăng nhập (tự động)
[ServiceFilter(typeof(SessionAuthorizationFilter))]
public class ProductsController : Controller { }

// Controller chỉ Admin mới truy cập được
[AdminOnly]
public class UsersController : Controller { }

// Action cụ thể chỉ Admin mới truy cập được
public class ProductsController : Controller {
    [AdminOnly]
    public IActionResult Create() { }
}
```

---

## 🎯 Security Notes

### **⚠️ Security Issues (Cần Cải Thiện)**
1. **Password Storage**: Hiện tại lưu plain text → Nên hash với BCrypt/PBKDF2
2. **CSRF Protection**: Đã có `[ValidateAntiForgeryToken]` cho POST
3. **SQL Injection**: An toàn do dùng Entity Framework
4. **Session Hijacking**: Nên thêm SSL/HTTPS và secure cookies

### **✅ Security Best Practices Đã Áp Dụng**
- ✅ Input validation (client + server)
- ✅ Anti-forgery token cho POST requests
- ✅ Session timeout tự động
- ✅ Error messages không tiết lộ thông tin nhạy cảm
- ✅ Authorization filter tự động

### **🔐 Recommendations**
```csharp
// TODO: Implement password hashing
using BCrypt.Net;

// Khi tạo user mới
user.Password = BCrypt.HashPassword(plainPassword);

// Khi xác thực
bool isValid = BCrypt.Verify(plainPassword, user.Password);
```

---

## 📊 Session Lifecycle

```
User Login
    ↓
Session Created (20 minutes idle timeout)
    ↓
User hoạt động → Session renewed
    ↓
User Logout / Timeout → Session Clear
    ↓
Redirect → Login Page
```

---

## 🐛 Error Handling

### **Login Errors**
```csharp
// Validation errors
ViewBag.Error = "Tên đăng nhập phải có ít nhất 3 ký tự"

// Authentication failed
ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng"

// System error
ViewBag.Error = "Có lỗi xảy ra trong quá trình đăng nhập. Vui lòng thử lại sau."
```

### **Change Password Errors**
```csharp
// Validation errors
TempData["Error"] = "Mật khẩu mới và xác nhận mật khẩu không khớp"

// Wrong old password
TempData["Error"] = "Mật khẩu cũ không đúng"

// Success
TempData["Success"] = "Đổi mật khẩu thành công!"
```

---

## 📝 Testing Checklist

- [ ] Login với thông tin hợp lệ → Chuyển đến Dashboard
- [ ] Login với username sai → Hiện lỗi
- [ ] Login với password sai → Hiện lỗi
- [ ] Login với username < 3 ký tự → Hiện lỗi validation
- [ ] Login với password < 6 ký tự → Hiện lỗi validation
- [ ] Logout → Xóa session và chuyển về Login
- [ ] Truy cập trang khi chưa login → Redirect về Login
- [ ] Đổi mật khẩu với mật khẩu cũ đúng → Thành công
- [ ] Đổi mật khẩu với mật khẩu cũ sai → Hiện lỗi
- [ ] Đổi mật khẩu với xác nhận không khớp → Hiện lỗi validation
- [ ] Staff truy cập trang Admin only → Bị từ chối

---

## 🔗 Related Files

- `Controllers/AuthController.cs`
- `Services/AuthService.cs`
- `Services/IAuthService.cs`
- `Filters/SessionAuthorizationFilter.cs`
- `Filters/AdminOnlyAttribute.cs`
- `Models/User.cs`
- `Views/Auth/Login.cshtml`
- `Views/Auth/ChangePassword.cshtml`
