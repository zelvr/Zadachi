namespace Zadachi.Lib.Database
{
    public interface IDatabaseService<T> where T : class
    {
        IQueryable<T> Query();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T? entity);
        Task SaveChangesAsync();
    }
}
