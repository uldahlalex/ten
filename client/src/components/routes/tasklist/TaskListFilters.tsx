import {useAtom} from "jotai";
import {CurrentTasksDisplayView, JwtAtom, QueryParametersAtom} from "@/atoms";
import {useEffect} from "react";
import {taskClient} from "../../../apiControllerClients";

export default function TaskListFilters() {
    
    const [queryParams, setQueryParams] = useAtom(QueryParametersAtom)
    const [, setTasks] = useAtom(CurrentTasksDisplayView);
    const [jwt] = useAtom(JwtAtom);
    
    useEffect(() => {
        if(jwt==null || jwt.jwt.length==0)
            return;
        taskClient.getMyTasks(jwt?.jwt, queryParams).then(result => {
            setTasks(result);
        })
    }, [queryParams])
    
    return <>
    
        <div className="flex flex-col gap-4 p-4">
            <h2 className="text-lg font-semibold">Filters</h2>
            <div className="form-control">
                <label className="label">
                    <span className="label-text">Search Term</span>
                </label>
                <input
                    type="text"
                    className="input input-bordered"
                    value={queryParams.searchTerm || ''}
                    onChange={(e) => setQueryParams({...queryParams, searchTerm: e.target.value})}
                />
            </div>
 
           
            <div className="form-control">
                <label className="label">
                    <span className="label-text">Minimum Priority</span>
                </label>
                <input
                    type="number"
                    min={0}
                    max={5}
                    className="input input-bordered"
                    value={queryParams.minPriority || ''}
                    onChange={(e) => setQueryParams({...queryParams, minPriority: parseInt(e.target.value)})}
                />
            </div>
            <div className="form-control">
                <label className="label">
                    <span className="label-text">Maximum Priority</span>
                </label>
                <input
                    type="number"
                    min={0}
                    max={5}
                    className="input input-bordered"
                    value={queryParams.maxPriority || ''}
                    onChange={(e) => setQueryParams({...queryParams, maxPriority: parseInt(e.target.value)})}
                />
            </div>
            
        </div>
        
        
    </>
    
}