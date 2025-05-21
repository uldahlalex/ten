import {useAtom} from "jotai";
import {CurrentTasksDisplayView, JwtAtom, ListsAtom, QueryParametersAtom, TagsAtom} from "../../atoms/atoms.ts";
import {taskClient} from "../../apiControllerClients.ts";
import {GetTasksFilterAndOrderParameters, TagDto, TasklistDto} from "../../generated-client.ts";
import {useCallback} from "react";
import SignOut from "../../functions/signOut.tsx";

export default function Sidebar() {
    const [lists] = useAtom(ListsAtom);
    const [jwt] = useAtom(JwtAtom);
    const [tags] = useAtom(TagsAtom);
    const [, setTasks] = useAtom(CurrentTasksDisplayView);
    const [params, setParams] = useAtom<GetTasksFilterAndOrderParameters>(QueryParametersAtom);

    const handleListClick = (list: TasklistDto) => {
        setParams((prevParams) => {
            const exists = (prevParams.listIds?.includes(list.listId))!;
            let newParams = null;
            if(!exists)
                newParams = {...prevParams, listIds: [list.listId]};
            else
                newParams = {...prevParams, listIds: prevParams.listIds?.filter((id) => id !== list.listId)};
            taskClient.getMyTasks(
                jwt!.jwt,
                (newParams)
            ).then(r => {
                setTasks(r);
            });
            return newParams;
        });
    };

    const handleTagClick = ((tag: TagDto) => {
        let duplicateTagIds: string[] = [...params.tagIds!]
        const includesNewTag = duplicateTagIds.includes(tag.tagId);
        if (includesNewTag) {
            duplicateTagIds = duplicateTagIds.filter(tagId => tagId !== tag.tagId);
        } else {
            duplicateTagIds.push(tag.tagId);
        }

        setParams((existingParams) => {
            const newObject = {...existingParams, tagIds: duplicateTagIds};
            taskClient.getMyTasks(jwt!.jwt, (newObject)).then(r => {
                setTasks(r);
            });
            return newObject;
        })


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