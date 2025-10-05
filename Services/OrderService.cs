using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangBanLe.Data;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Services
{
    public class OrderService : GenericRepository<Order>
    {
        public OrderService(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Tạo đơn hàng mới với validation đầy đủ và cập nhật tồn kho
        /// </summary>
        public async Task<(bool Success, string Message, Order? Order)> CreateOrderAsync(Order order, List<OrderItem> orderItems, string paymentMethod = "cash")
        {
            Console.WriteLine("🔵 ========== OrderService.CreateOrderAsync ==========");
            
            // Sử dụng transaction để đảm bảo tính toàn vẹn dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validate đơn hàng cơ bản
                Console.WriteLine("   [1] Validate đơn hàng cơ bản...");
                
                if (order == null)
                {
                    Console.WriteLine("   ❌ Order null");
                    return (false, "Thông tin đơn hàng không hợp lệ", null);
                }

                if (orderItems == null || !orderItems.Any(i => i.ProductId > 0))
                {
                    Console.WriteLine($"   ❌ OrderItems không hợp lệ - null: {orderItems == null}, count: {orderItems?.Count}, có ProductId > 0: {orderItems?.Any(i => i.ProductId > 0)}");
                    return (false, "Đơn hàng phải có ít nhất một sản phẩm", null);
                }

                if (order.CustomerId == null || order.CustomerId <= 0)
                {
                    Console.WriteLine($"   ❌ CustomerId không hợp lệ: {order.CustomerId}");
                    return (false, "Vui lòng chọn khách hàng", null);
                }

                Console.WriteLine($"   ✅ Validation cơ bản OK - CustomerId: {order.CustomerId}, Items: {orderItems.Count(i => i.ProductId > 0)}");

                // 2. Kiểm tra khách hàng tồn tại
                Console.WriteLine("   [2] Kiểm tra khách hàng tồn tại...");
                var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == order.CustomerId);
                if (!customerExists)
                {
                    Console.WriteLine($"   ❌ Khách hàng {order.CustomerId} không tồn tại");
                    return (false, "Khách hàng không tồn tại", null);
                }
                Console.WriteLine($"   ✅ Khách hàng {order.CustomerId} tồn tại");

                // 3. Validate và tính toán từng sản phẩm
                Console.WriteLine("   [3] Validate và tính toán sản phẩm...");
                decimal calculatedTotal = 0;
                var validOrderItems = new List<OrderItem>();

                foreach (var item in orderItems.Where(i => i.ProductId > 0))
                {
                    Console.WriteLine($"      - Xử lý ProductId: {item.ProductId}, Quantity: {item.Quantity}");
                    
                    // Lấy thông tin sản phẩm
                    var product = await _context.Products
                        .Include(p => p.Inventory)
                        .FirstOrDefaultAsync(p => p.ProductId == item.ProductId);

                    if (product == null)
                    {
                        Console.WriteLine($"      ❌ Sản phẩm {item.ProductId} không tồn tại");
                        return (false, $"Sản phẩm ID {item.ProductId} không tồn tại", null);
                    }

                    // Kiểm tra số lượng
                    if (item.Quantity <= 0)
                    {
                        Console.WriteLine($"      ❌ Số lượng không hợp lệ: {item.Quantity}");
                        return (false, $"Số lượng sản phẩm '{product.ProductName}' phải lớn hơn 0", null);
                    }

                    // Kiểm tra tồn kho
                    var currentStock = product.Inventory?.Quantity ?? 0;
                    Console.WriteLine($"      Tồn kho hiện tại: {currentStock}");
                    
                    if (currentStock < item.Quantity)
                    {
                        Console.WriteLine($"      ❌ Không đủ tồn kho");
                        return (false, $"Sản phẩm '{product.ProductName}' chỉ còn {currentStock} {product.Unit} trong kho", null);
                    }

                    // Tính toán giá
                    item.Price = product.Price;
                    item.Subtotal = item.Quantity * item.Price;
                    calculatedTotal += item.Subtotal;

                    Console.WriteLine($"      ✅ {product.ProductName}: {item.Quantity} x {item.Price} = {item.Subtotal}");

                    validOrderItems.Add(item);
                }

                Console.WriteLine($"   ✅ Tổng tiền hàng: {calculatedTotal}");

                // 4. Validate giảm giá
                Console.WriteLine("   [4] Validate giảm giá...");
                if (order.DiscountAmount < 0)
                {
                    Console.WriteLine($"   ❌ Giảm giá âm: {order.DiscountAmount}");
                    return (false, "Giá trị giảm giá không hợp lệ", null);
                }

                if (order.DiscountAmount > calculatedTotal)
                {
                    Console.WriteLine($"   ❌ Giảm giá {order.DiscountAmount} > tổng tiền {calculatedTotal}");
                    return (false, "Giá trị giảm giá không được vượt quá tổng tiền hàng", null);
                }
                Console.WriteLine($"   ✅ Giảm giá OK: {order.DiscountAmount}");

                // 5. Validate promotion (nếu có)
                if (order.PromoId.HasValue && order.PromoId > 0)
                {
                    Console.WriteLine($"   [5] Kiểm tra promotion: {order.PromoId}");
                    var promotion = await _context.Promotions
                        .FirstOrDefaultAsync(p => p.PromoId == order.PromoId);

                    if (promotion == null)
                    {
                        Console.WriteLine($"   ❌ Promotion không tồn tại");
                        return (false, "Mã khuyến mãi không tồn tại", null);
                    }

                    if (promotion.Status != "active")
                    {
                        Console.WriteLine($"   ❌ Promotion không active: {promotion.Status}");
                        return (false, "Mã khuyến mãi không còn hiệu lực", null);
                    }

                    if (promotion.StartDate > DateTime.Now || promotion.EndDate < DateTime.Now)
                    {
                        Console.WriteLine($"   ❌ Promotion hết hạn");
                        return (false, "Mã khuyến mãi đã hết hạn hoặc chưa bắt đầu", null);
                    }

                    if (promotion.UsageLimit > 0 && promotion.UsedCount >= promotion.UsageLimit)
                    {
                        Console.WriteLine($"   ❌ Promotion hết lượt: {promotion.UsedCount}/{promotion.UsageLimit}");
                        return (false, "Mã khuyến mãi đã hết lượt sử dụng", null);
                    }

                    if (promotion.MinOrderAmount > 0 && calculatedTotal < promotion.MinOrderAmount)
                    {
                        Console.WriteLine($"   ❌ Không đủ giá trị tối thiểu: {calculatedTotal} < {promotion.MinOrderAmount}");
                        return (false, $"Đơn hàng phải có giá trị tối thiểu {promotion.MinOrderAmount:N0}đ để sử dụng mã này", null);
                    }

                    // Cập nhật số lần sử dụng
                    promotion.UsedCount++;
                    _context.Promotions.Update(promotion);
                    Console.WriteLine($"   ✅ Promotion OK, UsedCount: {promotion.UsedCount}");
                }

                // 6. Thiết lập thông tin đơn hàng
                Console.WriteLine("   [6] Thiết lập thông tin đơn hàng...");
                order.OrderDate = DateTime.Now;
                order.TotalAmount = calculatedTotal;
                order.Status = string.IsNullOrEmpty(order.Status) ? "pending" : order.Status;

                // Validate tổng tiền cuối
                var finalAmount = order.TotalAmount - order.DiscountAmount;
                if (finalAmount < 0)
                {
                    Console.WriteLine($"   ❌ Tổng tiền âm: {finalAmount}");
                    return (false, "Tổng tiền đơn hàng không hợp lệ", null);
                }
                Console.WriteLine($"   ✅ Tổng thanh toán: {finalAmount}");

                // 7. Lưu đơn hàng
                Console.WriteLine("   [7] Lưu đơn hàng vào DB...");
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                Console.WriteLine($"   ✅ Đơn hàng đã lưu, OrderId: {order.OrderId}");

                // 8. Lưu OrderItems và cập nhật tồn kho
                Console.WriteLine("   [8] Lưu OrderItems và cập nhật tồn kho...");
                foreach (var item in validOrderItems)
                {
                    item.OrderId = order.OrderId;
                    await _context.OrderItems.AddAsync(item);
                    Console.WriteLine($"      - Thêm OrderItem: ProductId={item.ProductId}, Quantity={item.Quantity}");

                    // Cập nhật tồn kho
                    var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(i => i.ProductId == item.ProductId);

                    if (inventory != null)
                    {
                        var oldQuantity = inventory.Quantity;
                        inventory.Quantity -= item.Quantity;
                        inventory.UpdatedAt = DateTime.Now;
                        _context.Inventories.Update(inventory);
                        Console.WriteLine($"      - Cập nhật inventory: {oldQuantity} -> {inventory.Quantity}");
                    }
                    else
                    {
                        // Tạo mới inventory nếu chưa có (với số âm)
                        var newInventory = new Inventory
                        {
                            ProductId = item.ProductId,
                            Quantity = -item.Quantity,
                            UpdatedAt = DateTime.Now
                        };
                        await _context.Inventories.AddAsync(newInventory);
                        Console.WriteLine($"      - Tạo mới inventory: {newInventory.Quantity}");
                    }
                }

                await _context.SaveChangesAsync();
                Console.WriteLine("   ✅ OrderItems và inventory đã lưu");

                // 9. Tạo Payment record nếu đơn hàng đã thanh toán
                if (order.Status == "paid")
                {
                    Console.WriteLine("   [9] Tạo Payment record...");
                    
                    // Validate payment method
                    var validPaymentMethods = new[] { "cash", "card", "bank_transfer", "e_wallet" };
                    if (!validPaymentMethods.Contains(paymentMethod.ToLower()))
                    {
                        Console.WriteLine($"   ⚠️ Payment method không hợp lệ: {paymentMethod}, dùng mặc định 'cash'");
                        paymentMethod = "cash";
                    }
                    
                    var payment = new Payment
                    {
                        OrderId = order.OrderId,
                        Amount = finalAmount,
                        PaymentMethod = paymentMethod,
                        PaymentDate = DateTime.Now
                    };
                    await _context.Payments.AddAsync(payment);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"   ✅ Payment đã lưu, Amount: {finalAmount}, Method: {paymentMethod}");
                }

                // 10. Commit transaction
                Console.WriteLine("   [10] Commit transaction...");
                await transaction.CommitAsync();
                Console.WriteLine("   ✅ Transaction committed");

                // Load lại đơn hàng với đầy đủ thông tin
                var createdOrder = await GetByIdAsync(order.OrderId);
                Console.WriteLine($"✅ ========== TẠO ĐƠN HÀNG THÀNH CÔNG - OrderId: {order.OrderId} ==========");
                return (true, "Tạo đơn hàng thành công", createdOrder);
            }
            catch (Exception ex)
            {
                // Rollback nếu có lỗi
                Console.WriteLine($"❌ ========== LỖI: {ex.Message} ==========");
                Console.WriteLine($"   StackTrace: {ex.StackTrace}");
                await transaction.RollbackAsync();
                return (false, $"Lỗi khi tạo đơn hàng: {ex.Message}", null);
            }
        }

        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _dbSet
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.Promotion)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .AsSplitQuery()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public override async Task<Order?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.Promotion)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .AsSplitQuery()
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<IEnumerable<Order>> GetByCustomerAsync(int customerId)
        {
            return await _dbSet
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.Promotion)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .AsSplitQuery()
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.Promotion)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .AsSplitQuery()
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Include(o => o.Customer)
                .Include(o => o.User)
                .Include(o => o.Promotion)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Payments)
                .AsSplitQuery()
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.Where(o => o.Status == "paid");

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value);

            return await query.SumAsync(o => o.TotalAmount - o.DiscountAmount);
        }

        public async Task<int> GetTotalOrdersAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate <= endDate.Value);

            return await query.CountAsync();
        }
    }
}
