import {useAtom} from "jotai";
import {CurrentTasksDisplayView, JwtAtom} from "../atoms.ts";
import {taskClient} from "../apiControllerClients";
import {UpdateTaskRequestDto} from "../generated-client";
import toast from "react-hot-toast";
import ToUpdateDto from "../mappings.ts";

export default function TaskList() {

    const [tasks, setTasks] = useAtom(CurrentTasksDisplayView);
    const [jwt] = useAtom(JwtAtom);

    function handleClickCheckbox(e, task) {
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
    }

    return (<>

        <ul className="list bg-base-100 rounded-box shadow-md">
            <li  className="p-4 pb-2 text-xs opacity-60 tracking-wide">Tasks:</li>

            {


                tasks.map((task, index) => {
                    return (

                        <li key={index} className="list-row">
                            <div>
                                <input type="checkbox" checked={task.completed} onChange={e => handleClickCheckbox(e, task)}/> {JSON.stringify(task)}

                            </div>

                        </li>

                    )
                    
                    })
            }


       

           

        </ul>
        

 </>)

}