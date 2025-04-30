using Microsoft.EntityFrameworkCore;

public static class SpecificationEvaluator
{
    public static IQueryable<T> ApplySpecification<T>(this IQueryable<T> query, 
        ISpecification<T> spec) where T : class
    {
        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);
        
        if (spec.OrderBy != null)
            query = query.OrderBy(spec.OrderBy);
        else if (spec.OrderByDescending != null)
            query = query.OrderByDescending(spec.OrderByDescending);
        
        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

        return query;
    }
}