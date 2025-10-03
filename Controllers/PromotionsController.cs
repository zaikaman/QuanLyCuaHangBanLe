using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Controllers
{
    public class PromotionsController : Controller
    {
        private static List<Promotion> promotions = new List<Promotion>
        {
            new Promotion 
            { 
                PromoId = 1, 
                PromoCode = "SUMMER2025", 
                Description = "Giảm giá mùa hè 2025",
                DiscountType = "percent", 
                DiscountValue = 20, 
                StartDate = DateTime.Now.AddDays(-10), 
                EndDate = DateTime.Now.AddDays(20),
                MinOrderAmount = 100000,
                UsageLimit = 100,
                UsedCount = 45,
                Status = "active"
            },
            new Promotion 
            { 
                PromoId = 2, 
                PromoCode = "FREESHIP", 
                Description = "Miễn phí vận chuyển",
                DiscountType = "fixed", 
                DiscountValue = 30000, 
                StartDate = DateTime.Now.AddDays(-5), 
                EndDate = DateTime.Now.AddDays(25),
                MinOrderAmount = 200000,
                UsageLimit = 50,
                UsedCount = 12,
                Status = "active"
            },
        };

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(promotions);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                promotion.PromoId = promotions.Count > 0 ? promotions.Max(p => p.PromoId) + 1 : 1;
                promotions.Add(promotion);
                return RedirectToAction("Index");
            }
            return View(promotion);
        }

        public IActionResult Delete(int id)
        {
            var promotion = promotions.FirstOrDefault(p => p.PromoId == id);
            if (promotion != null)
            {
                promotions.Remove(promotion);
            }
            return RedirectToAction("Index");
        }
    }
}
