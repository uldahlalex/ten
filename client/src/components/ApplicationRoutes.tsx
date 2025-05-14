import { Route, Routes, useNavigate, Outlet } from "react-router-dom";
import useInitializeData from "../hooks/useInitializeData.tsx";
import { HomeRoute, SignInRoute } from '../routeConstants.ts';
import SignIn from "./SignIn.tsx";
import Sidebar from "./Sidebar.tsx";
import TaskList from "./TaskList.tsx";
import {ProtectedRoute} from "./ProtectedRoute.tsx";


const MainLayout = () => {
    return (
        <div className="flex h-screen">
            <div className="w-64 h-full border-r border-gray-200 bg-white">
                <Sidebar />
            </div>
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
                <Route path={SignInRoute} element={<SignIn />} />
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