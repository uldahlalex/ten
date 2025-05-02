import {useEffect} from "react";
import {useAtom} from "jotai";
import { JwtAtom} from "../atoms.ts";

export default function useInitializeData() {

    const [jwt] = useAtom(JwtAtom);

    useEffect(() => {
        if (jwt == null || jwt.length < 1)
            return;
        
    }, [jwt])

}