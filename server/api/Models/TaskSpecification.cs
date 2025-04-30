using api;
using efscaffold.Entities;

public class TaskSpecification : BaseSpecification<Tickticktask>
{
    public TaskSpecification(GetTasksFilterAndOrderParameters parameters)
    {
        if (parameters.IsCompleted.HasValue)
            AddCriteria(x => x.Completed == parameters.IsCompleted.Value);

        if (parameters.DueDateStart.HasValue)
            AddCriteria(x => x.DueDate >= parameters.DueDateStart.Value);
        
        if (parameters.DueDateEnd.HasValue)
            AddCriteria(x => x.DueDate <= parameters.DueDateEnd.Value);

        
        switch (parameters.OrderBy)
        {
            case StaticConstants.DueDate:
                if (parameters.IsDescending)
                    AddOrderByDesc(x => x.DueDate);
                else
                    AddOrderBy(x => x.DueDate);
                break;
        }
    }
}