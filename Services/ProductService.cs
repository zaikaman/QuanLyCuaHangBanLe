using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangBanLe.Data;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Services
{
    public class ProductService : GenericRepository<Product>
    {
        public ProductService(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking() // Không track để tránh conflict khi update
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Inventory)
                .ToListAsync();
        }

        public override async Task<Product?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Inventory)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetBySupplierAsync(int supplierId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Inventory)
                .Where(p => p.SupplierId == supplierId)
                .ToListAsync();
        }

        public async Task<Product?> GetByBarcodeAsync(string barcode)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        public async Task<IEnumerable<Product>> SearchAsync(string keyword)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Inventory)
                .Where(p => p.ProductName.Contains(keyword) || 
                           (p.Barcode != null && p.Barcode.Contains(keyword)))
                .ToListAsync();
        }

        /// <summary>
        /// Kiểm tra xem sản phẩm có thể xóa được không
        /// </summary>
        public async Task<(bool CanDelete, string Message)> CanDeleteProductAsync(int productId)
        {
            // Kiểm tra xem sản phẩm có tồn tại không
            var product = await GetByIdAsync(productId);
            if (product == null)
            {
                return (false, "Không tìm thấy sản phẩm");
            }

            // Kiểm tra xem sản phẩm có trong đơn hàng nào không
            var hasOrders = await _context.OrderItems
                .AnyAsync(oi => oi.ProductId == productId);

            if (hasOrders)
            {
                var orderCount = await _context.OrderItems
                    .Where(oi => oi.ProductId == productId)
                    .Select(oi => oi.OrderId)
                    .Distinct()
                    .CountAsync();

                return (false, $"Không thể xóa sản phẩm '{product.ProductName}' vì đã có {orderCount} đơn hàng sử dụng sản phẩm này");
            }

            return (true, "Có thể xóa sản phẩm");
        }
    }
}
