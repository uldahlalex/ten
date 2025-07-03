import {CreateListRequestDto} from "@/models";
import {useState} from "react";

export interface CreateListModalProps {
    onSubmit: (dto: CreateListRequestDto) => void;
    onClose: () => void;
}

export default function CreateListModal(props: CreateListModalProps) {

    const [state, setState] = useState<CreateListRequestDto>({
        listName: ''
    })

    return <>
        <div className="modal modal-open">
            <div className="modal-box">
                <h3 className="font-bold text-lg">Create New List</h3>
                <div className="py-4">
                    <input className="input" value={state.listName} placeholder="New list name"
                           onChange={e => setState({...state, listName: e.target.value})}/>
                  
                </div>  <button className="btn btn-primary" onClick={() => {
                props.onSubmit(state);
            }}>Create new list
            </button>
                <button className="btn btn-secondary" onClick={() => props.onClose()}>Close</button>
            </div>
        </div>
        
    </>
}