import {TickticktaskDto, UpdateTaskRequestDto} from "@/models";
import {useState} from "react";
import {useAtom} from "jotai";
import {ListsAtom} from "@/atoms";
import TaskTagsDropdown from "./updateTags";
import DateTimeInput from "../../reusables/DateTime";

export interface UpdateTaskProps {
    task: TickticktaskDto;
    onSubmit: (task: UpdateTaskRequestDto) => void;
    onCancel?: () => void;
}

export default function UpdateTask(props: UpdateTaskProps) {

    const [updateForm, setUpdateForm] = useState<UpdateTaskRequestDto>({
        description: props.task.description,
        completed: props.task.completed,
        dueDate: props.task.dueDate,
        id: props.task.taskId,
        listId: props.task.listId,
        priority: props.task.priority,
        title: props.task.title
    })

    console.log(updateForm)

    const [myLists] = useAtom(ListsAtom);


    return (<>
        <div className="p-6 max-w-2xl mx-auto">
            <form className="space-y-4" onSubmit={(e) => {
                e.preventDefault();
                props.onSubmit(updateForm);
            }}>
                <div className="form-control w-full">
                    <label className="label">
                        <span className="label-text">Title</span>
                    </label>
                    <input
                        type="text"
                        className="input input-bordered w-full"
                        value={updateForm.title}
                        onChange={(e) => setUpdateForm({...updateForm, title: e.target.value})}
                        required
                    />
                </div>

                <div className="form-control w-full">
                    <label className="label">
                        <span className="label-text">Description</span>
                    </label>
                    <textarea
                        className="textarea textarea-bordered h-24"
                        value={updateForm.description}
                        onChange={(e) => setUpdateForm({...updateForm, description: e.target.value})}
                    />
                </div>


                <div className="form-control w-full">
                    <label className="label">
                        <span className="label-text">Due Date</span>
                    </label>
                    {
                        JSON.stringify(updateForm)
                    }
                   <DateTimeInput onChange={date => setUpdateForm({...updateForm,dueDate: date.toISOString()})} 
                                  value={new Date(updateForm.dueDate!)} />
                </div>
                <div className="form-control w-full">
                    <label className="label">
                        <span className="label-text">List</span>
                    </label>
                    <select
                        className="select select-bordered w-full"
                        value={updateForm.listId}
                        onChange={(e) => setUpdateForm({...updateForm, listId: e.target.value})}
                        required
                    >
                        {myLists.map((list) => (
                            <option key={list.listId} value={list.listId}>
                                {list.name}
                            </option>
                        ))}
                    </select>
                </div>
                <TaskTagsDropdown task={props.task}/>
                <div className="form-control w-full">
                    <label className="label">
                        <span className="label-text">Priority</span>
                    </label>
                    <select
                        className="select select-bordered w-full"
                        value={updateForm.priority}
                        onChange={(e) => setUpdateForm({...updateForm, priority: Number.parseInt(e.target.value)})}
                        required
                    >
                        <option value="0">None</option>
                        <option value="1">Low</option>
                        <option value="2">Medium</option>
                        <option value="3">High</option>
                    </select>
                </div>
                <div className="form-control w-full">
                    <label className="label">
                        <span className="label-text">Completed</span>
                    </label>
                    <input
                        type="checkbox"
                        className="checkbox"
                        checked={updateForm.completed}
                        onChange={(e) => setUpdateForm({...updateForm, completed: e.target.checked})}
                    />
                    <label className="label-text">Mark as completed</label>
                </div>
                <div className="modal-action">
                    <button
                        type="submit"
                        className="btn btn-primary"
                    >
                        Update Task
                    </button>
                    {props.onCancel && (
                        <button
                            type="button"
                            className="btn btn-secondary"
                            onClick={props.onCancel}
                        >
                            Cancel
                        </button>
                    )}
                </div>

            </form>
        </div>
    </>)
}