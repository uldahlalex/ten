import {useEffect} from "react";
import {useAtom} from "jotai";
import {JwtAtom, ListsAtom, TagsAtom} from "../atoms/atoms.ts";
import {taskClient} from "../apiControllerClients.ts";

export default function useInitializeData() {

    const [jwt] = useAtom(JwtAtom);
    const [, setLists] = useAtom(ListsAtom);
    const [, setTags] = useAtom(TagsAtom);

    useEffect(() => {
        if (jwt == null || jwt.jwt.length < 1)
            return;
        taskClient.getMyLists(jwt.jwt).then(r => {
            setLists(r);
        })
        taskClient.getMyTags(jwt.jwt).then(r => {
            setTags(r);
        });
    }, [jwt])

}