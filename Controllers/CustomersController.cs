using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;
using QuanLyCuaHangBanLe.Services;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class CustomersController : Controller
    {
        private readonly IGenericRepository<Customer> _customerRepository;

        public CustomersController(IGenericRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            const int pageSize = 10;
            var allCustomers = await _customerRepository.GetAllAsync();
            
            // Áp dụng tìm kiếm TRƯỚC KHI phân trang
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                allCustomers = allCustomers.Where(c =>
                    (c.Name != null && c.Name.ToLower().Contains(searchTerm)) ||
                    (c.Phone != null && c.Phone.Contains(searchTerm)) ||
                    (c.Email != null && c.Email.ToLower().Contains(searchTerm)) ||
                    (c.Address != null && c.Address.ToLower().Contains(searchTerm))
                ).ToList();
            }
            
            var totalItems = allCustomers.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var customers = allCustomers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchTerm = searchTerm;

            return View(customers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (customer == null)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction("Index");
            }

            // Validation bổ sung
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                ModelState.AddModelError("Name", "Tên khách hàng không được để trống");
            }

            // Kiểm tra email và phone nếu có
            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                var existingCustomers = await _customerRepository.GetAllAsync();
                if (existingCustomers.Any(c => c.Email != null && c.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng");
                }
            }

            if (!string.IsNullOrWhiteSpace(customer.Phone))
            {
                var existingCustomers = await _customerRepository.GetAllAsync();
                if (existingCustomers.Any(c => c.Phone != null && c.Phone.Equals(customer.Phone)))
                {
                    ModelState.AddModelError("Phone", "Số điện thoại này đã được sử dụng");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    customer.CreatedAt = DateTime.Now;
                    customer.Name = customer.Name.Trim();
                    customer.Phone = customer.Phone?.Trim();
                    customer.Email = customer.Email?.Trim();
                    customer.Address = customer.Address?.Trim();
                    
                    await _customerRepository.AddAsync(customer);
                    TempData["Success"] = "Thêm khách hàng thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Lỗi thêm khách hàng: {ex.Message}");
                    ModelState.AddModelError("", "Lỗi khi thêm khách hàng: " + ex.Message);
                }
            }
            return View(customer);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                TempData["Error"] = "Dữ liệu không khớp";
                return RedirectToAction("Index");
            }

            if (customer == null)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction("Index");
            }

            // Validation bổ sung
            if (string.IsNullOrWhiteSpace(customer.Name))
            {
                ModelState.AddModelError("Name", "Tên khách hàng không được để trống");
            }

            // Kiểm tra email trùng (trừ chính nó)
            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                var allCustomers = await _customerRepository.GetAllAsync();
                if (allCustomers.Any(c => c.CustomerId != customer.CustomerId && 
                    c.Email != null && c.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng");
                }
            }

            // Kiểm tra phone trùng (trừ chính nó)
            if (!string.IsNullOrWhiteSpace(customer.Phone))
            {
                var allCustomers = await _customerRepository.GetAllAsync();
                if (allCustomers.Any(c => c.CustomerId != customer.CustomerId && 
                    c.Phone != null && c.Phone.Equals(customer.Phone)))
                {
                    ModelState.AddModelError("Phone", "Số điện thoại này đã được sử dụng");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    customer.Name = customer.Name.Trim();
                    customer.Phone = customer.Phone?.Trim();
                    customer.Email = customer.Email?.Trim();
                    customer.Address = customer.Address?.Trim();
                    
                    await _customerRepository.UpdateAsync(customer);
                    TempData["Success"] = "Cập nhật khách hàng thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Lỗi cập nhật khách hàng: {ex.Message}");
                    ModelState.AddModelError("", "Lỗi khi cập nhật khách hàng: " + ex.Message);
                }
            }
            return View(customer);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "ID không hợp lệ";
                    return RedirectToAction("Index");
                }

                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    TempData["Error"] = "Không tìm thấy khách hàng";
                    return RedirectToAction("Index");
                }

                // Kiểm tra xem khách hàng có đơn hàng nào không
                if (customer.Orders != null && customer.Orders.Any())
                {
                    TempData["Error"] = $"Không thể xóa khách hàng '{customer.Name}' vì còn {customer.Orders.Count} đơn hàng liên quan";
                    return RedirectToAction("Index");
                }

                await _customerRepository.DeleteAsync(id);
                TempData["Success"] = "Xóa khách hàng thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi xóa khách hàng: {ex.Message}");
                
                // Kiểm tra lỗi foreign key constraint
                if (ex.InnerException?.Message.Contains("foreign key constraint") == true || 
                    ex.Message.Contains("FOREIGN KEY") || 
                    ex.Message.Contains("DELETE statement conflicted"))
                {
                    TempData["Error"] = "Không thể xóa khách hàng này vì còn đơn hàng liên quan";
                }
                else
                {
                    TempData["Error"] = "Lỗi khi xóa khách hàng: " + ex.Message;
                }
                
                return RedirectToAction("Index");
            }
        }
    }
}
