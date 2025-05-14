import {TickticktaskDto, UpdateTaskRequestDto} from "../generated-client.ts";

export default function ToUpdateDto(task: TickticktaskDto): UpdateTaskRequestDto {
    return new UpdateTaskRequestDto({
        listId: task.listId,
        completed: task.completed,
        title: task.title,
        dueDate: task.dueDate,
        priority: task.priority,
        id: task.taskId,
        description: task.description
    });
}