import {useAtom} from "jotai";
import {CurrentTasksDisplayView, JwtAtom, ListsAtom, QueryParametersAtom, TagsAtom} from "@/atoms";
import {taskClient} from "../../apiControllerClients";
import {GetTasksFilterAndOrderParameters, TagDto, TasklistDto} from "@/models";
import {useCallback, useState} from "react";
import {SignOut} from "@/functions";
import CreateListModal from "./CreateListModal.tsx";
import toast from "react-hot-toast";

export default function Sidebar() {
    const [lists, setLists] = useAtom(ListsAtom);
    const [jwt] = useAtom(JwtAtom);
    const [tags] = useAtom(TagsAtom);
    const [, setTasks] = useAtom(CurrentTasksDisplayView);
    const [params, setParams] = useAtom<GetTasksFilterAndOrderParameters>(QueryParametersAtom);
    const [createListModal, setOpenCreateListModal] = useState(false)

    const handleListClick = (list: TasklistDto) => {
        setParams((prevParams) => {
            const exists = prevParams.listIds?.includes(list.listId) ?? false;
            let newParams = null;
            if (!exists)
                newParams = {...prevParams, listIds: [list.listId]};
            else
                newParams = {...prevParams, listIds: prevParams.listIds?.filter((id) => id !== list.listId)};
            taskClient.getMyTasks(
                jwt!.jwt,
                (newParams)
            ).then(result => {
                setTasks(result);
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
            taskClient.getMyTasks(jwt!.jwt, (newObject)).then(result => {
                setTasks(result);
            });
            return newObject;
        })


    });

    const handleSignOut = useCallback(() => {
        SignOut();
    }, []);

    return (
        <>
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
                                <button
                                    className={`btn mt-2  btn-accent`}
                                    onClick={() => setOpenCreateListModal(true)}
                                >Create new list</button>

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

                                className="btn btn-danger mt-8 w-full"
                                onClick={handleSignOut}
                            >
                                Sign out
                            </button>

                        </> :

                        <>

                        </>
                }


            </div>

            { createListModal &&
                <CreateListModal onSubmit={(dto) => {
                taskClient.createList(jwt?.jwt, dto).then(r => {
                      setLists((prevLists) => [...prevLists, r]);
                      toast.success('New list created: '+r.name)
                    setOpenCreateListModal(false)
                }) 
            }} onClose={() => setOpenCreateListModal(false)} /> }
        </>
    )
}