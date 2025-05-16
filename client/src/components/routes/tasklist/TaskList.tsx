import {useAtom} from "jotai";
import {CurrentTasksDisplayView, JwtAtom} from "../../../atoms/atoms.ts";
import {taskClient} from "../../../apiControllerClients.ts";
import {CreateTaskRequestDto, TickticktaskDto, UpdateTaskRequestDto} from "../../../generated-client.ts";
import toast from "react-hot-toast";
import ToUpdateDto from "../../../functions/mappings.ts";
import CreateNewTask from "./CreateNewTask.tsx";
import {useState} from "react";

export default function TaskList() {
    const [tasks, setTasks] = useAtom(CurrentTasksDisplayView);
    const [jwt] = useAtom(JwtAtom);
    const [isModalOpen, setIsModalOpen] = useState(false);

    function handleClickCheckbox(e: React.ChangeEvent<HTMLInputElement>, task: TickticktaskDto) {
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
        <div className="w-full px-4">
            <ul className="list bg-base-100 rounded-box shadow-md w-full space-y-4">
                <button
                    className="btn btn-primary"
                    onClick={() => setIsModalOpen(true)}
                >
                    Create New Task
                </button>

                {isModalOpen && (
                    <div className="modal modal-open">
                        <div className="modal-box">
                            <h3 className="font-bold text-lg">Create New Task</h3>
                            <div className="py-4">
                                <CreateNewTask onCancel={() => setIsModalOpen(false)} onSubmit={(task) => {
                                    taskClient.createTask(new CreateTaskRequestDto(task), jwt!.jwt).then(result => {
                                        setTasks([...tasks, result]);
                                        toast.success("Task created successfully");
                                        setIsModalOpen(false);
                                    })
                                } } />
                            </div>
                            <div className="modal-action">
                                <button
                                    className="btn"
                                    onClick={() => setIsModalOpen(false)}
                                >
                                    Close
                                </button>
                            </div>
                        </div>
                        <div
                            className="modal-backdrop"
                            onClick={() => setIsModalOpen(false)}
                        ></div>
                    </div>
                )}
                    
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