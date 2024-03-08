using patient_test_task.Entities;

namespace patient_test_task.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        T Create(T entity);
        T Update(T entity);
        Guid Delete(T entity);
        IEnumerable<T> GetAll();
        T1 GetResultSpec<T1>(Func<IQueryable<T>, T1> func);
        T GetById(Guid id);
        IQueryable<T1> GetListResultSpec<T1>(Func<IQueryable<T>, IQueryable<T1>> func);
        IEnumerable<T> CreateMany(IEnumerable<T> entities);

    }
}
