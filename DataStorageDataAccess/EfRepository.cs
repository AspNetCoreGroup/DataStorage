using DataStorageCore.Domain;
using DataStorageCore.Repositories;
using DataStorageDataAccess.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataStorageDataAccess
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly EnergyContext _datacontext;
        protected DbSet<T> Data;


        public EfRepository(EnergyContext datacontext)
        {
            _datacontext = datacontext;
            Data = _datacontext.Set<T>();
        }
        public async Task AddAsync(T entity)
        {
            await Data.AddAsync(entity);
            await _datacontext.SaveChangesAsync();
            //await Data.FirstOrDefaultAsync(x=>entity.Equals);
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Data.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await Data.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await Data.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await Data.Where(predicate).ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entity)
        {
            await Data.AddRangeAsync(entity);
            await _datacontext.SaveChangesAsync();
        }
    }
}
