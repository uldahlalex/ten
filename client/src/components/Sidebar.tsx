import { useAtom } from "jotai";
import { CurrentTasksDisplayView, JwtAtom, ListsAtom, TagsAtom } from "../atoms.ts";
import { taskClient } from "../apiControllerClients.ts";
import { GetTasksFilterAndOrderParameters, TagDto, TasklistDto } from "../generated-client";
import { useState, useCallback } from "react";

export default function Sidebar() {
    const [lists] = useAtom(ListsAtom);
    const [jwt] = useAtom(JwtAtom);
    const [tags] = useAtom(TagsAtom);
    const [, setTasks] = useAtom(CurrentTasksDisplayView);
    const [selectedTags, setSelectedTags] = useState<string[]>([]);
    const [selectedList, setSelectedList] = useState<string | null>(null);

    const handleListClick = useCallback((list: TasklistDto) => {
        setSelectedList(list.listId);
        setSelectedTags([]); // Clear selected tags when selecting a list

        taskClient.getMyTasks(jwt, new GetTasksFilterAndOrderParameters({
            listIds: [list.listId]
        })).then(r => {
            setTasks(r);
        });
    }, [jwt, setTasks]);

    const handleTagClick = useCallback((tag: TagDto) => {
        setSelectedTags(prev => {
            const newSelected = prev.includes(tag.tagId)
                ? prev.filter(id => id !== tag.tagId)
                : [...prev, tag.tagId];

            setSelectedList(null); // Clear selected list when selecting tags

            // Fetch tasks with updated tag selection
            taskClient.getMyTasks(jwt, new GetTasksFilterAndOrderParameters({
                tagIds: newSelected
            })).then(r => {
                setTasks(r);
            });

            return newSelected;
        });
    }, [jwt, setTasks]);

    const handleSignOut = useCallback(() => {
        // Implement sign out logic here
    }, []);

    return (
        <div className="list bg-base-100 rounded-box shadow-md p-4">
            <div className="flex flex-col">
                <li className="pb-2 text-xs opacity-60 tracking-wide">Lists</li>
                {lists.map((list, index) => (
                    <button
                        className={`btn mt-2 ${selectedList === list.listId ? 'btn-primary' : ''}`}
                        key={index}
                        onClick={() => handleListClick(list)}
                    >
                        {list.name}
                    </button>
                ))}

                <li  className="pt-6 pb-2 text-xs opacity-60 tracking-wide">Tags</li>
                {tags.map((tag, index) => (
                    <button
                        className={`btn mt-2 ${selectedTags.includes(tag.tagId) ? 'btn-primary' : ''}`}
                        key={index}
                        onClick={() => handleTagClick(tag)}
                    >
                        {tag.name}
                    </button>
                ))}
            </div>

            <button
                className="btn btn-outline mt-8 w-full"
                onClick={handleSignOut}
            >
                Sign out
            </button>
        </div>
    );
}