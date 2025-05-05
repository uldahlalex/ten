import {useAtom} from "jotai";
import {CurrentTasksDisplayView, ListsAtom} from "../atoms.ts";
import {taskClient} from "../apiControllerClients.ts";

export default function Sidebar() {
    
    
    const [lists, setLists] = useAtom(ListsAtom);
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
                        taskClient.getMyTasks(localStorage.getItem('jwt')!,  {
                            listIds: [list.listId!]
                        }).then(r => {
                            setTasks(r);
                        })
                    }}>
                        {list.name}
                    </li>)}
                    )
                }

            </div>
        </>
    );
}