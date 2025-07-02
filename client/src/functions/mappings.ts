import { TickticktaskDto, UpdateTaskRequestDto } from "@/models";

export default function ToUpdateDto(task: TickticktaskDto): UpdateTaskRequestDto {
    return ({
        listId: task.listId,
        completed: task.completed,
        title: task.title,
        dueDate: task.dueDate,
        priority: task.priority,
        id: task.taskId,
        description: task.description
    });
}