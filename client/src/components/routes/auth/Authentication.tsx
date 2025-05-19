import { Outlet, useNavigate, useLocation } from "react-router-dom";
import {PasswordSignInRoute, TaskListRoute, TotpRoute} from "../../ApplicationRoutes.tsx";
import {useEffect} from "react";
import {useAtom} from "jotai";
import {JwtAtom} from "../../../atoms/atoms.ts";
import {JwtResponse} from "../../../generated-client.ts";
import {pathIncludes} from "../../../functions/pathIncludes.ts";

export default function Authentication() {
    const navigate = useNavigate();
    const location = useLocation();
    const [jwt] = useAtom<JwtResponse | undefined>(JwtAtom);

    useEffect(() => {
        if(jwt && jwt.jwt && jwt.jwt.length > 1) {
            navigate(TaskListRoute)
        } 
    }, [jwt]);


    return (
        <div className="flex flex-col items-center space-between min-h-screen gap-4 p-4">
            {!pathIncludes(TotpRoute, location) && (
                <button
                    className="px-4 py-2 text-white bg-blue-600 rounded hover:bg-blue-700"
                    onClick={() => navigate(TotpRoute)}
                >
                    Go to authenticator sign-in
                </button>
            )}

            {!pathIncludes(PasswordSignInRoute, location) && (
                <button
                    className="px-4 py-2 text-white bg-blue-600 rounded hover:bg-blue-700"
                    onClick={() => navigate(PasswordSignInRoute)}
                >
                    Go to password sign in
                </button>
            )}

            <Outlet />
        </div>
    );
}