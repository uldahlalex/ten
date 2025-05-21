import {useEffect} from "react";
import {useAtom} from "jotai";
import {CurrentTasksDisplayView, JwtAtom, ListsAtom, QueryParametersAtom, TagsAtom} from "../atoms/atoms.ts";
import {taskClient} from "../apiControllerClients.ts";

export default function useInitializeData() {

    const [jwt] = useAtom(JwtAtom);
    const [, setLists] = useAtom(ListsAtom);
    const [, setTags] = useAtom(TagsAtom);
    const [, setTasks] = useAtom(CurrentTasksDisplayView);
    const [params] = useAtom(QueryParametersAtom);

    useEffect(() => {
        if (jwt == null || jwt.jwt.length < 1)
            return;
        taskClient.getMyLists(jwt.jwt).then(r => {
            setLists(r);
        })
        taskClient.getMyTags(jwt.jwt).then(r => {
            setTags(r);
        });
        taskClient.getMyTasks(jwt.jwt, params).then(r => {
            setTasks(r)
        })
    }, [jwt])

}