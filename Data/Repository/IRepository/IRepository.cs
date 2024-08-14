using System.Linq.Expressions;

public interface IRepository<T> where T: class {
    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);    
    public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
    public void Add(T entity);
    public void Remove(T entity);
    public void RemoveRange(IEnumerable<T> entities);

}