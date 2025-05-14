import {useAtom} from "jotai/index";
import {JwtAtom} from "../atoms.ts";
import {Navigate, Outlet} from "react-router-dom";
import {SignInRoute} from "../routeConstants.ts";
import SignIn from "./SignIn.tsx";

export interface ProtectedRouteProps {
    children: React.ReactNode;
}

export const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
    const [jwt] = useAtom(JwtAtom);

    if (!jwt || jwt.jwt!.length === 0) {
        return <SignIn />;
    }

    return children;
};