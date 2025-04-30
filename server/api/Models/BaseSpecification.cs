using System.Linq.Expressions;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; private set; }  // Add ? to make it nullable
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }  // Add ? for consistency
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }  // Add ?

    protected void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = Criteria == null ? criteria : And(Criteria, criteria);
    }

    protected void AddOrderBy(Expression<Func<T, object>> orderBy) => OrderBy = orderBy;
    protected void AddOrderByDesc(Expression<Func<T, object>> orderByDesc) => OrderByDescending = orderByDesc;

    private Expression<Func<T, bool>> And(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var body = Expression.AndAlso(
            Expression.Invoke(first, param),
            Expression.Invoke(second, param)
        );
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}