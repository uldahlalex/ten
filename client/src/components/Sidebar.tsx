import {useAtom} from "jotai";
import {CurrentTasksDisplayView, JwtAtom, ListsAtom, TagsAtom} from "../atoms.ts";
import {taskClient} from "../apiControllerClients.ts";
import {GetTasksFilterAndOrderParameters} from "../generated-client";

export default function Sidebar() {
    
    
    const [lists, setLists] = useAtom(ListsAtom);
    const [jwt, setJwt] = useAtom(JwtAtom)
    const [tags] = useAtom(TagsAtom);
    const [, setTasks] = useAtom(CurrentTasksDisplayView)
    
    return (
        
        <>
            <button onClick={() => {
                
            }}>Sign out</button>
            <div className="flex flex-col">
                <ol>Lists</ol>
                {
                    lists.map((list, index) =>  {
                    return(<li key={list.listId} onClick={() => {
                        taskClient.getMyTasks(jwt,  new GetTasksFilterAndOrderParameters({
                            listIds: [list.listId]
                        })).then(r => {
                            setTasks(r);
                        })
                    }}>
                        {list.name}
                    </li>)}
                    )
                }
                <ol> Tags </ol>
                    {
                        tags.map((tag,  index) => {
                            return(<li key={tag.tagId} onClick={() => {
                                const tasksForTag = taskClient.getMyTasks(jwt, {
                                    tagIds: [tag.tagId!]
                                }).then(r => {
                                    setTasks(r);
                                })
                            }}>
                                {
                                    tag.name
                                }
                            </li>);
                        })
                    }
              
            </div>
            
        </>
    );
}