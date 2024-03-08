using patient_test_task.Entities;
using patient_test_task.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace patient_test_task.Database
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationContext _context;
        public Repository(ApplicationContext context) => _context = context;

        public T Create(T entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public IEnumerable<T> CreateMany(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            _context.SaveChanges();
            return entities;
        }

        public Guid Delete(T entity)
        {
           _context.Remove(entity);
           _context.SaveChanges();
            return entity.Id;
        }

        public IEnumerable<T> GetAll() => _context.Set<T>().AsNoTracking();

        public T GetById(Guid id) => _context.Set<T>().SingleOrDefault(el => el.Id.Equals(id));

        public IQueryable<T1> GetListResultSpec<T1>(Func<IQueryable<T>, IQueryable<T1>> func) => func(_context.Set<T>());
        public T1 GetResultSpec<T1>(Func<IQueryable<T>, T1> func) => func(_context.Set<T>().AsNoTracking());

        public T Update(T entity)
        {
            _context.Update(entity);
            _context.SaveChanges();
            return entity;
        }
    }
}
