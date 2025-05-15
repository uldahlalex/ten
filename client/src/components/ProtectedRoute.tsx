import {useAtom} from "jotai/index";
import {JwtAtom} from "../atoms/atoms.ts";
import {useNavigate} from "react-router-dom";
import {AuthenticationRoute, TaskListRoute} from "./ApplicationRoutes.tsx";
import {useEffect} from "react";

export interface ProtectedRouteProps {
    children: React.ReactNode;
}

export const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
    const [jwt] = useAtom(JwtAtom);
    const navigate = useNavigate();

    useEffect(() => {
        if (!jwt || !jwt.jwt || jwt.jwt!.length === 0) {
            navigate(AuthenticationRoute)
        } 
    }, [jwt]);

    return children;
};