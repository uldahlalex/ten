import {useAtom} from "jotai";
import {CurrentTasksDisplayView, JwtAtom, ListsAtom, QueryParametersAtom, TagsAtom} from "../../atoms/atoms.ts";
import {taskClient} from "../../apiControllerClients.ts";
import {
    GetTasksFilterAndOrderParameters,
    IGetTasksFilterAndOrderParameters,
    TagDto,
    TasklistDto
} from "../../generated-client.ts";
import {useCallback} from "react";
import SignOut from "../../functions/signOut.tsx";
import {useNavigate} from "react-router-dom";

export default function Sidebar() {
    const [lists] = useAtom(ListsAtom);
    const [jwt] = useAtom(JwtAtom);
    const [tags] = useAtom(TagsAtom);
    const [, setTasks] = useAtom(CurrentTasksDisplayView);
    const [params, setParams] = useAtom<IGetTasksFilterAndOrderParameters>(QueryParametersAtom);
    const navigate = useNavigate();

    const handleListClick = ((list: TasklistDto) => {
        setParams({...params, listIds: [list.listId]});
        taskClient.getMyTasks(jwt!.jwt, new GetTasksFilterAndOrderParameters(params)).then(r => {
            setTasks(r);
        });
    });

    const handleTagClick = ((tag: TagDto) => {
        const existingTagIds: string[] = [...params.tagIds!]
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


        taskClient.getMyTasks(jwt!.jwt, new GetTasksFilterAndOrderParameters(params)).then(r => {
            setTasks(r);
        });


    });

    const handleSignOut = useCallback(() => {
        SignOut();
    }, []);

    return (
        <div className="flex flex-col justify-between list bg-base-100 rounded-box shadow-md p-4 h-full">


            {
                jwt && jwt.jwt.length > 0 ? <>
                    <div className="flex flex-col">
                        <li className="pb-2 text-xs opacity-60 tracking-wide">Lists</li>
                        {lists.map((list, index) => (
                            <button
                                className={`btn mt-2 ${params.listIds!.includes(list.listId) ? 'btn-primary' : ''}`}
                                key={index}
                                onClick={() => handleListClick(list)}
                            >
                                {list.name}
                            </button>
                        ))}

                        <li className="pt-6 pb-2 text-xs opacity-60 tracking-wide">Tags</li>
                        {tags.map((tag, index) => (
                            <button
                                className={`btn mt-2 ${params.tagIds!.includes(tag.tagId) ? 'btn-primary' : ''}`}
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

                </> : 
                    
                    <>
                     
                    </>
            }
            
            
        </div>
    )
}