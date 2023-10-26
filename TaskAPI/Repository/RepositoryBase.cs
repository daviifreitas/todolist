using Activity.API.Context;
using Activity.API.Entities;
using Activity.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Activity.API.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : EntityBase
    {
        private readonly TaskDbContext _context;

        public RepositoryBase(TaskDbContext context)
        {
            _context = context;
        }

        public Task<T?> GetByIdAsync(int id)
        {
            return _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id);
        }

        public Task<List<T>> GetAllAsync()
        {
            return _context.Set<T>().Where(entity => entity.StatusDefault == StatusDefault.Active).ToListAsync();
        }

        public async Task<List<T>> GetManyByFunctionAsync(Func<T, bool> func)
        {
            return _context.Set<T>().Where(func).ToList();
        }

        public async Task<T?> GetOneByFunctionAsync(Func<T, bool> func)
        {
            return _context.Set<T>().FirstOrDefault(func);
        }

        public async Task<bool> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.Now;
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            T? entityForDelete = _context.Set<T>().FirstOrDefault(entity => entity.Id == id);

            if (entityForDelete is null) return false;

            entityForDelete.StatusDefault = StatusDefault.Deleted;
            _context.Set<T>().Update(entityForDelete);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
