import { useEffect } from "react";
import { useAtom } from "jotai";
import { CurrentTasksDisplayView, JwtAtom, ListsAtom, QueryParametersAtom, TagsAtom } from "@/atoms";
import { taskClient } from "../apiControllerClients";

export default function useInitializeData() {
    const [jwt] = useAtom(JwtAtom);
    const [, setLists] = useAtom(ListsAtom);
    const [, setTags] = useAtom(TagsAtom);
    const [, setTasks] = useAtom(CurrentTasksDisplayView);
    const [params] = useAtom(QueryParametersAtom);

    useEffect(() => {
        if (jwt == null || jwt.jwt.length < 1)
            return;
        taskClient.getMyLists(jwt.jwt).then(result => {
            setLists(result);
        })
        taskClient.getMyTags(jwt.jwt).then(result => {
            setTags(result);
        });
        taskClient.getMyTasks(jwt.jwt, params).then(result => {
            setTasks(result)
        })
    }, [jwt])
}