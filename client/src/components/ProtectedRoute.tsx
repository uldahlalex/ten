import {useAtom} from "jotai";
import {JwtAtom} from "@/atoms";
import {useNavigate} from "react-router-dom";
import {AuthenticationRoute} from "./routes";
import {useEffect} from "react";

export interface ProtectedRouteProps {
    children: React.ReactNode;
}

export const ProtectedRoute = ({children}: ProtectedRouteProps) => {
    const [jwt] = useAtom(JwtAtom);
    const navigate = useNavigate();

    useEffect(() => {
        if (!jwt || !jwt.jwt || jwt.jwt!.length === 0) {
            navigate(AuthenticationRoute)
        }
    }, [jwt]);

    return children;
};