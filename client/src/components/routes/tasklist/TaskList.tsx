import {useAtom} from "jotai";
import {CurrentTasksDisplayView, JwtAtom} from "../../../atoms/atoms.ts";
import {taskClient} from "../../../apiControllerClients.ts";
import {TickticktaskDto} from "../../../generated-client.ts";
import toast from "react-hot-toast";
import ToUpdateDto from "../../../functions/mappings.ts";
import CreateNewTask from "./CreateNewTask.tsx";
import {useState} from "react";
import UpdateTask from "./UpdateTask.tsx";
import TaskListFilters from "./TaskListFilters.tsx";

export interface EditModal {
    isOpen: boolean;
    task?: TickticktaskDto;
}

export default function TaskList() {
    const [tasks, setTasks] = useAtom(CurrentTasksDisplayView);
    const [jwt] = useAtom(JwtAtom);
    const [createModalControl, setCreateModalControl] = useState(false);
    const [editModalControl, setEditModalControl] = useState<EditModal>({isOpen: false, task: undefined})

    function handleClickCheckbox(e: React.ChangeEvent<HTMLInputElement>, task: TickticktaskDto) {
        const updateDto = (ToUpdateDto(task));
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
        <div className="w-full px-4">
            <ul className="list bg-base-100 rounded-box shadow-md w-full space-y-4">
                <button
                    className="btn btn-primary"
                    onClick={() => setCreateModalControl(true)}
                >
                    Create New Task
                </button>

                <TaskListFilters />
                

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
                                        {/*    indented JSON representation*/}
                                        <pre className="whitespace-pre-wrap">
                                                {JSON.stringify(task, null, 2)}
                                            </pre>
                                    </div>
                                </div>
                                <div className="card-actions justify-end mt-4">
                                    <button onClick={() => {
                                        setEditModalControl({isOpen: true, task: task});
                                    }} className="btn btn-primary">Edit
                                    </button>
                                    <button className="btn btn-error">Delete</button>
                                </div>
                            
                            </div>
                        </div>
                    </li>
                ))}
            </ul>

            {createModalControl && (
                <div className="modal modal-open">
                    <div className="modal-box">
                        <h3 className="font-bold text-lg">Create New Task</h3>
                        <div className="py-4">
                            <CreateNewTask onCancel={() => setCreateModalControl(false)} onSubmit={(task) => {
                                taskClient.createTask((task), jwt!.jwt).then(result => {
                                    setTasks([...tasks, result]);
                                    toast.success("Task created successfully");
                                    setCreateModalControl(false);
                                })
                            }}/>
                        </div>
                        <div className="modal-action">
                            <button
                                className="btn"
                                onClick={() => setCreateModalControl(false)}
                            >
                                Close
                            </button>
                        </div>
                    </div>
                    <div
                        className="modal-backdrop"
                        onClick={() => setCreateModalControl(false)}
                    ></div>
                </div>
            )}

            {editModalControl.isOpen && (
                <div className="modal modal-open">
                    <div className="modal-box">
                        <h3 className="font-bold text-lg">Edit Task</h3>
                        <div className="py-4">
                            <UpdateTask onCancel={() => setEditModalControl({
                                isOpen: false,
                                task: undefined
                            })} task={editModalControl.task!} onSubmit={(updateDto) => {
                                taskClient.updateTask((updateDto), jwt!.jwt).then(result => {
                                    setTasks(tasks.map(t => {
                                        if (t.taskId === result.taskId) {
                                            return result;
                                        }
                                        return t;
                                    }));
                                    toast.success("Task updated successfully");
                                    setEditModalControl({isOpen: false, task: undefined});
                                })
                            }}/>
                        </div>
                        <div className="modal-action">
                            <button
                                className="btn"
                                onClick={() => setEditModalControl({
                                    isOpen: false,
                                    task: undefined
                                })}
                            >
                                Close
                            </button>
                        </div>
                    </div>
                    <div
                        className="modal-backdrop"
                        onClick={() => setEditModalControl({isOpen: false, task: undefined})}
                    ></div>
                </div>
            )}
        </div>
    )

}