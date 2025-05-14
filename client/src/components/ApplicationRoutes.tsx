import { Route, Routes, useNavigate, Outlet } from "react-router-dom";
import useInitializeData from "../hooks/useInitializeData.tsx";
import {HomeRoute, SignInRoute, TotpRoute} from '../routeConstants.ts';
import SignIn from "./SignIn.tsx";
import Sidebar from "./Sidebar.tsx";
import TaskList from "./TaskList.tsx";
import {ProtectedRoute} from "./ProtectedRoute.tsx";
import {useAtom} from "jotai";
import {JwtAtom} from "../atoms.ts";
import TotpAuth from "./TotpSignIn.tsx";


const MainLayout = () => {
    
    const [jwt] = useAtom(JwtAtom)
    
    return (
        <div className="flex h-screen">
            {
                jwt && jwt.jwt.length > 0 && <>   <div className="w-64 h-full border-r border-gray-200 bg-white">
                    <Sidebar />
                </div></>
            }
         
            <div className="flex-1 h-full">
                <Outlet />
            </div>
        </div>
    );
};

export default function ApplicationRoutes() {
    useInitializeData();

    return (
        <Routes>
            <Route element={<MainLayout />}>
                <Route path={SignInRoute} element={<SignIn />}>
                    <Route element={<TotpAuth />} path={TotpRoute} />
                </Route>
                <Route
                    path={HomeRoute}
                    element={
                        <ProtectedRoute>
                            <TaskList />
                        </ProtectedRoute>
                    }
                />

            </Route>
        </Routes>
    );
}