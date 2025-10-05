using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangBanLe.Data;
using System.Linq.Expressions;

namespace QuanLyCuaHangBanLe.Services
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            // Detach tất cả entities đang được tracked với cùng key
            var entry = _context.Entry(entity);
            var keyValues = entry.Metadata.FindPrimaryKey()!.Properties
                .Select(p => entry.Property(p.Name).CurrentValue)
                .ToArray();
            
            var trackedEntity = _context.ChangeTracker.Entries<T>()
                .FirstOrDefault(e => e.Metadata.FindPrimaryKey()!.Properties
                    .Select(p => e.Property(p.Name).CurrentValue)
                    .SequenceEqual(keyValues));
            
            if (trackedEntity != null && trackedEntity.Entity != entity)
            {
                trackedEntity.State = EntityState.Detached;
            }
            
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}
