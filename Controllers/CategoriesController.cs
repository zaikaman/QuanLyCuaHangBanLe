using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class CategoriesController : Controller
    {
        private static List<Category> categories = new List<Category>
        {
            new Category { CategoryId = 1, CategoryName = "Đồ uống" },
            new Category { CategoryId = 2, CategoryName = "Bánh kẹo" },
            new Category { CategoryId = 3, CategoryName = "Gia vị" },
            new Category { CategoryId = 4, CategoryName = "Đồ gia dụng" },
            new Category { CategoryId = 5, CategoryName = "Mỹ phẩm" },
        };

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.CategoryId = categories.Count > 0 ? categories.Max(c => c.CategoryId) + 1 : 1;
                categories.Add(category);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Edit(int id)
        {
            var category = categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var existingCategory = categories.FirstOrDefault(c => c.CategoryId == category.CategoryId);
                if (existingCategory != null)
                {
                    existingCategory.CategoryName = category.CategoryName;
                }
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Delete(int id)
        {
            var category = categories.FirstOrDefault(c => c.CategoryId == id);
            if (category != null)
            {
                categories.Remove(category);
            }
            return RedirectToAction("Index");
        }
    }
}
