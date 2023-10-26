using Activity.API.Entities;

namespace Activity.API.Interfaces
{
    public interface IRepositoryBase <T> where T : EntityBase
    {
        Task<T?>  GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetManyByFunctionAsync(Func<T, bool> func);
        Task<T?> GetOneByFunctionAsync(Func<T, bool> func);
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}
