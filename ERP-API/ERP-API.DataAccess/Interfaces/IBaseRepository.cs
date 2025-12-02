using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_API.DataAccess.Interfaces
{
    public interface IBaseRepository<TEntity, TId> where TEntity : class
    {
        // READ
        Task<IEnumerable<TEntity>> GetAllAsync();

        // This stays synchronous because it just prepares the query, doesn't execute it yet.
        IQueryable<TEntity> GetAllQueryable();

        Task<TEntity?> FindByIdAsync(TId id);

        // WRITE
        // Create is now Async because EF Core might need to generate IDs or check constraints
        Task CreateAsync(TEntity entity);

        // Update/Delete are often synchronous in EF Core (just tracking changes), 
        // but we keep them void or Task for consistency. 
        // Standard EF Core Update() is actually synchronous, but let's stick to standard patterns.
        void Update(TEntity entity);

        // Delete often requires finding the entity first, so Async is useful here if we pass an ID.
        Task<TEntity?> DeleteAsync(TId id);
    }
}
