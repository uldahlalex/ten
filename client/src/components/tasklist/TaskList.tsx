import {useAtom} from "jotai";
import {CurrentTasksDisplayView, JwtAtom} from "../../atoms/atoms.ts";
import {taskClient} from "../../apiControllerClients.ts";
import {TickticktaskDto, UpdateTaskRequestDto} from "../../generated-client.ts";
import toast from "react-hot-toast";
import ToUpdateDto from "../../functions/mappings.ts";

export default function TaskList() {

    const [tasks, setTasks] = useAtom(CurrentTasksDisplayView);
    const [jwt] = useAtom(JwtAtom);

    function handleClickCheckbox(e:  React.ChangeEvent<HTMLInputElement>, task: TickticktaskDto) {
        const updateDto = new UpdateTaskRequestDto(ToUpdateDto(task));
        updateDto.completed = e.target.checked;
        taskClient.updateTask(updateDto, jwt!.jwt).then(result => {
            setTasks(tasks.map(t => {
                if (t.taskId === task.taskId) {
                    return result;
                }
                return t;
            }));
            toast.success("Task updated successfully");

        })
    }
    return (

        <div className="w-full px-4"> {/* Added padding and full width */}
                <ul className="list bg-base-100 rounded-box shadow-md w-full space-y-4">
                    {/*<input value={} />*/}
                    
                    <li className="p-4 pb-2 text-xs opacity-60 tracking-wide">Tasks:</li>
                    {tasks.map((task, index) => (
                        <li key={index} className="w-full">
                            <div className="card w-full bg-base-100 shadow-md">
                                <div className="card-body">
                                    <div className="flex items-center gap-4">
                                        <input
                                            type="checkbox"
                                            checked={task.completed}
                                            onChange={e => handleClickCheckbox(e, task)}
                                            className="checkbox"
                                        />
                                        <div className="flex-1">
                                            <h2 className="card-title">{task.title}</h2>
                                            <p className="text-gray-600">{task.description}</p>
                                            <p className="text-gray-600">{task.taskId}</p>
                                        </div>
                                    </div>
                                    <div className="card-actions justify-end mt-4">
                                        <button className="btn btn-primary">Edit</button>
                                        <button className="btn btn-error">Delete</button>
                                    </div>
                                </div>
                            </div>
                        </li>
                    ))}
                </ul>
            </div>
    )

}