import {useEffect, useState} from 'react';
import {taskClient} from "../../../apiControllerClients.ts";
import {ChangeTaskTagRequestDto, TickticktaskDto} from "../../../generated-client.ts";
import toast from "react-hot-toast";
import {useAtom} from "jotai";
import {CurrentTasksDisplayView, JwtAtom, QueryParametersAtom, TagsAtom} from "../../../atoms/atoms.ts";

export interface TaskTagsDropdownProps {
    task: TickticktaskDto;
}

const TaskTagsDropdown = (props: TaskTagsDropdownProps) => {
    const [selectedTags, setSelectedTags] = useState<string[]>([]);
    const [isOpen, setIsOpen] = useState(false);
    const [myTags] = useAtom(TagsAtom);
    const [jwt] = useAtom(JwtAtom)
    const [tasks, setTasks] = useAtom(CurrentTasksDisplayView)
    const [queryParams] = useAtom(QueryParametersAtom);

    useEffect(() => {
        setSelectedTags(props.task.taskTags.map(t => t.tagId));
    }, [props.task.taskTags]);

    const handleTagToggle = (tagId: string) => {
        if (selectedTags.includes(tagId)) {

            taskClient.removeTaskTag(jwt?.jwt, {
                tagId: tagId,
                taskId: props.task.taskId
            }).then(async r => {
                tasks.map(task => {
                    if (task.taskId === props.task.taskId) {
                        return {
                            ...task,
                            taskTags: task.taskTags.filter(tag => tag.tagId !== tagId)
                        };
                    }
                    return task;
                });
                toast.success("Tag removed successfully");
                setSelectedTags(selectedTags.filter(id => id !== tagId));
                setTasks(await taskClient.getMyTasks(jwt?.jwt, queryParams));
            });
        } else {
            const param: ChangeTaskTagRequestDto = {
                tagId: tagId,
                taskId: props.task.taskId
            }
            taskClient.addTaskTag(jwt?.jwt, param).then(async r => {
                tasks.map(task => {
                    if (task.taskId === props.task.taskId) {
                        return {
                            ...task,
                            taskTags: [...task.taskTags, {
                                tagId: tagId,
                                name: myTags.find(tag => tag.tagId === tagId)?.name
                            }]
                        };
                    }
                    return task;
                });
                toast.success("Tag added successfully");
                setSelectedTags([...selectedTags, tagId]);
                setTasks(await taskClient.getMyTasks(jwt?.jwt, queryParams));
            });
        }
    };

    const getSelectedTagNames = () => {
        return selectedTags
            .map(tagId => myTags.find(tag => tag.tagId === tagId)?.name)
            .filter(Boolean)
            .join(', ');
    };

    return (
        <div className="form-control w-full">
            <label className="label">
                <span className="label-text">Tags</span>
            </label>

            <div className="dropdown w-full">
                <div
                    tabIndex={0}
                    className="select select-bordered w-full flex items-center justify-between cursor-pointer"
                    onClick={() => setIsOpen(!isOpen)}
                >
                    {selectedTags.length > 0 ? (
                        <span className="truncate">{getSelectedTagNames()}</span>
                    ) : (
                        <span className="text-gray-400">Select tags</span>
                    )}
                </div>

                <div className="menu dropdown-content p-2 shadow bg-base-100 rounded-box w-full">
                    {myTags.map((tag) => {
                        return (
                            <label key={tag.tagId} className="flex">
                                <input
                                    type="checkbox"
                                    className="checkbox checkbox-primary"
                                    checked={selectedTags.includes(tag.tagId)}
                                    onChange={() => handleTagToggle(tag.tagId)}
                                />
                                <span>{tag.name}</span>
                            </label>
                        );
                    })}
                </div>


            </div>
        </div>
    );
};

export default TaskTagsDropdown;