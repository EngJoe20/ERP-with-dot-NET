using ERP_API.DataAccess.Interfaces;
using ERP_API.DataAccess.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Repositories
{
    internal class BaseRepository<TEntity, TId> : IBaseRepository<TEntity, TId> where TEntity : class
    {
        private readonly ErpDBContext _shoppingDbContext;
        private readonly DbSet<TEntity> _dbSet;

        public BaseRepository(ErpDBContext shoppingDbContext)
        {
            _shoppingDbContext = shoppingDbContext;
            _dbSet = _shoppingDbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAllQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }



        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity?> FindByIdAsync(TId id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task CreateAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<TEntity?> DeleteAsync(TId id)
        {
            TEntity? entity = await FindByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                return entity;
            }
            return null;
        }
    }
}
