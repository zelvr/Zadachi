using Microsoft.EntityFrameworkCore;
using Zadachi.Models;

namespace Zadachi.Lib.Database
{
    public class DatabaseService<T>:IDatabaseService<T> where T : class
    {
        private readonly ZadachiDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public DatabaseService(ZadachiDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }
        public IQueryable<T> Query() => _dbSet.AsQueryable();

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T? entity)
        {
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
    }
}
