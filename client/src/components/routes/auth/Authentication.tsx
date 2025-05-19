import {useNavigate} from "react-router-dom";
import {PasswordSignInRoute, TaskListRoute, TotpRoute} from "../../ApplicationRoutes.tsx";
import {useEffect} from "react";
import {useAtom} from "jotai";
import {JwtAtom} from "../../../atoms/atoms.ts";
import {JwtResponse} from "../../../generated-client.ts";

export default function Authentication() {
    const navigate = useNavigate();
    const [jwt] = useAtom<JwtResponse | undefined>(JwtAtom);

    useEffect(() => {
        if (jwt && jwt.jwt && jwt.jwt.length > 1) {
            navigate(TaskListRoute)
        }
    }, [jwt]);


    return (
        <div className="flex flex-col items-center justify-center min-h-screen gap-4 p-4 h-max">
            <button
                className="btn btn-primary"
                onClick={() => navigate(TotpRoute)}
            >
                Go to authenticator sign-in
            </button>

            <button
                className="btn btn-primary"
                onClick={() => navigate(PasswordSignInRoute)}
            >
                Go to password sign in
            </button>
        </div>
    );
}