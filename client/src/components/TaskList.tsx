import {useAtom} from "jotai";
import {CurrentTasksDisplayView, JwtAtom} from "../atoms.ts";
import {taskClient} from "../apiControllerClients";
import {UpdateTaskRequestDto} from "../generated-client";
import toast from "react-hot-toast";
import ToUpdateDto from "../mappings.ts";

export default function TaskList() {

    const [tasks, setTasks] = useAtom(CurrentTasksDisplayView);
    const [jwt] = useAtom(JwtAtom);

    return (<ol>

        {
            tasks.map((task, index) => {
                return (<li key={task.taskId}>
                    <input type="checkbox" checked={task.completed} onChange={e => {
                        const updateDto = new UpdateTaskRequestDto(ToUpdateDto(task));
                        updateDto.completed = e.target.checked;
                        taskClient.updateTask(updateDto, jwt).then(result => {
                            setTasks(tasks.map(t => {
                                if (t.taskId === task.taskId) {
                                    return result;
                                }
                                return t;
                            }));
                            toast.success("Task updated successfully");

                        })
                    }}/> {JSON.stringify(task)}
                    <br />
                    <br />

                </li>)
            })
        }

    </ol>)
}