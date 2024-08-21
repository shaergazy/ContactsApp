using Microsoft.EntityFrameworkCore;
using ContactsApp.Services.Interfaces;

namespace ContactsApp.Services
{
    public class GenericService<TEntity, TKey> : IGenericService<TEntity, TKey>
        where TEntity : class
    {
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly ContactsDbContext _context;

        public GenericService(ContactsDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity model)
        {
            await _context.AddAsync(model);
            _context.SaveChanges();
        }

        public virtual async Task DeleteAsync(TKey id)
        {
            TEntity entity = await _dbSet.FindAsync(id);
            _context.Remove(entity);
            _context.SaveChanges();
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        public void Update(TEntity model)
        {
            _dbSet.Update(model);
            _context.SaveChanges();
        }
    }
}
