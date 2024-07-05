using DataStorageCore.Repositories;
using DataStorageDataAccess.DataBaseContext;
using Microsoft.EntityFrameworkCore;

namespace DataStorageDataAccess
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly EnergyContext _datacontext;
        protected DbSet<T> Data;


        public EfRepository(EnergyContext datacontext)
        {
            _datacontext = datacontext;
            Data = _datacontext.Set<T>();
        }
        public Task AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Data.ToListAsync();
        }

        public Task<T> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
