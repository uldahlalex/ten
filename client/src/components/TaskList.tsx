import {useAtom} from "jotai";
import {CurrentTasksDisplayView} from "../atoms.ts";

export default function TaskList() {
    
    const [tasks, setTasks] = useAtom(CurrentTasksDisplayView);
    
    return(<ol>

        {
            tasks.map((task, index) => {
                return(<li>
                    {task.title}, {task.taskId}, {task.listId}
                
                </li>)
            })
        }
    
    </ol>)
}