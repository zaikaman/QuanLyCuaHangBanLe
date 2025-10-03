using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class CustomersController : Controller
    {
        private static List<Customer> customers = new List<Customer>
        {
            new Customer { CustomerId = 1, Name = "Nguyễn Văn A", Phone = "0909000001", Email = "kh1@mail.com", Address = "Địa chỉ 1" },
            new Customer { CustomerId = 2, Name = "Trần Thị B", Phone = "0909000002", Email = "kh2@mail.com", Address = "Địa chỉ 2" },
            new Customer { CustomerId = 3, Name = "Lê Văn C", Phone = "0909000003", Email = "kh3@mail.com", Address = "Địa chỉ 3" },
            new Customer { CustomerId = 4, Name = "Phạm Thị D", Phone = "0909000004", Email = "kh4@mail.com", Address = "Địa chỉ 4" },
            new Customer { CustomerId = 5, Name = "Hoàng Văn E", Phone = "0909000005", Email = "kh5@mail.com", Address = "Địa chỉ 5" },
        };

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(customers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CustomerId = customers.Count > 0 ? customers.Max(c => c.CustomerId) + 1 : 1;
                customers.Add(customer);
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        public IActionResult Edit(int id)
        {
            var customer = customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        public IActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                var existingCustomer = customers.FirstOrDefault(c => c.CustomerId == customer.CustomerId);
                if (existingCustomer != null)
                {
                    existingCustomer.Name = customer.Name;
                    existingCustomer.Phone = customer.Phone;
                    existingCustomer.Email = customer.Email;
                    existingCustomer.Address = customer.Address;
                }
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        public IActionResult Delete(int id)
        {
            var customer = customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer != null)
            {
                customers.Remove(customer);
            }
            return RedirectToAction("Index");
        }
    }
}
