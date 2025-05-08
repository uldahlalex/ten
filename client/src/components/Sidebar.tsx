import { useAtom } from "jotai";
import {CurrentTasksDisplayView, JwtAtom, ListsAtom, QueryParametersAtom, TagsAtom} from "../atoms.ts";
import { taskClient } from "../apiControllerClients.ts";
import {
    GetTasksFilterAndOrderParameters,
    IGetTasksFilterAndOrderParameters,
    TagDto,
    TasklistDto
} from "../generated-client";
import { useState, useCallback } from "react";
import toast from "react-hot-toast";
import SignOut from "../signOut.tsx";

export default function Sidebar() {
    const [lists] = useAtom(ListsAtom);
    const [jwt] = useAtom(JwtAtom);
    const [tags] = useAtom(TagsAtom);
    const [, setTasks] = useAtom(CurrentTasksDisplayView);
    const [params, setParams] = useAtom<IGetTasksFilterAndOrderParameters>(QueryParametersAtom);

    const handleListClick = ((list: TasklistDto) => {
        setParams({...params, listIds: [list.listId]});
        taskClient.getMyTasks(jwt, params).then(r => {
            setTasks(r);
        });
    });

    const handleTagClick = ((tag: TagDto) => {
        const existingTagIds = [...params.tagIds]
        const includesNewTag = existingTagIds.includes(tag.tagId);
        if (includesNewTag) {
            // Remove the tag ID from the array
            const newTagIds = existingTagIds.filter(tagId => tagId !== tag.tagId);
            setParams({...params, tagIds: newTagIds});
        } else {
            // Add the tag ID to the array
            existingTagIds.push(tag.tagId);
            setParams({...params, tagIds: existingTagIds});
        }
        
        
            // Fetch tasks with updated tag selection
            taskClient.getMyTasks(jwt, params).then(r => {
                setTasks(r);
            });

        
    });

    const handleSignOut = useCallback(() => {
        SignOut();
    }, []);

    return (
        <div className="list bg-base-100 rounded-box shadow-md p-4">
            <div className="flex flex-col">
                <li className="pb-2 text-xs opacity-60 tracking-wide">Lists</li>
                {lists.map((list, index) => (
                    <button
                        className={`btn mt-2 ${params.listIds.includes(list.listId) ? 'btn-primary' : ''}`}
                        key={index}
                        onClick={() => handleListClick(list)}
                    >
                        {list.name}
                    </button>
                ))}

                <li  className="pt-6 pb-2 text-xs opacity-60 tracking-wide">Tags</li>
                {tags.map((tag, index) => (
                    <button
                        className={`btn mt-2 ${params.tagIds.includes(tag.tagId) ? 'btn-primary' : ''}`}
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