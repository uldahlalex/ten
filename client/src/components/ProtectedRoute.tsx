import {useAtom} from "jotai/index";
import {JwtAtom} from "../atoms/atoms.ts";
import {useNavigate} from "react-router-dom";
import {AuthenticationRoute} from "./ApplicationRoutes.tsx";

export interface ProtectedRouteProps {
    children: React.ReactNode;
}

export const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
    const [jwt] = useAtom(JwtAtom);
    const navigate = useNavigate();

    if (!jwt || jwt.jwt!.length === 0) {
        navigate(AuthenticationRoute)
    }

    return children;
};