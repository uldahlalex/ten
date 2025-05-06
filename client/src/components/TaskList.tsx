import {useAtom} from "jotai";
import {CurrentTasksDisplayView, JwtAtom} from "../atoms.ts";
import {taskClient} from "../apiControllerClients";
import {UpdateTaskRequestDto} from "../generated-client";
import toast from "react-hot-toast";

export default function TaskList() {

    const [tasks, setTasks] = useAtom(CurrentTasksDisplayView);
    const [jwt] = useAtom(JwtAtom);

    return (<ol>

        {
            tasks.map((task, index) => {
                return (<li key={task.taskId}>
                    <input type="checkbox" checked={task.completed} onChange={e => {
                        const updateDto: UpdateTaskRequestDto = {
                            listId: task.listId!,
                            completed: e.target.checked!,
                            taskId: task.taskId!,
                            title: task.title!,
                            dueDate: task.dueDate!,
                            priority: task.priority!,
                            id: task.taskId!,
                            description: task.description!
                        };
                        taskClient.updateTask(updateDto, jwt).then(result => {
                            setTasks(tasks.map(t => {
                                if (t.taskId === task.taskId) {
                                    return {...t, completed: e.target.checked};
                                }
                                return t;
                            }));
                            toast.success("Task updated successfully");

                        }).then(err => {
                            toast.error("Error updating task");
                            console.error(err);
                        })
                    }}/> {task.title}, {task.taskId}, {task.listId}

                </li>)
            })
        }

    </ol>)
}