import { Outlet, useNavigate, useLocation } from "react-router-dom";
import {PasswordSignInRoute, TotpRoute} from "../../ApplicationRoutes.tsx";

export default function Authentication() {
    const navigate = useNavigate();
    const location = useLocation();

    const isPath = (path: string) => location.pathname === path;

    return (
        <div className="flex flex-col items-center justify-center min-h-screen gap-4 p-4">
            {!isPath(TotpRoute) && (
                <button
                    className="px-4 py-2 text-white bg-blue-600 rounded hover:bg-blue-700"
                    onClick={() => navigate(TotpRoute)}
                >
                    Sign in with authenticator
                </button>
            )}

            {!isPath(PasswordSignInRoute) && (
                <button
                    className="px-4 py-2 text-white bg-blue-600 rounded hover:bg-blue-700"
                    onClick={() => navigate(PasswordSignInRoute)}
                >
                    Sign in with password
                </button>
            )}

            <Outlet />
        </div>
    );
}