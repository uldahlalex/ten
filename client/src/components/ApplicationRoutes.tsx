import {Route, Routes} from "react-router-dom";
import TaskList from "./tasklist/TaskList.tsx";
import {ProtectedRoute} from "./ProtectedRoute.tsx";
import TotpAuth from "./auth/authpages/TotpSignIn.tsx";
import Authentication from "./auth/Authentication.tsx";
import {MainLayout} from "./MainLayout.tsx";
import SignInWithPassword from "./auth/authpages/SignInWithPassword.tsx";
import useInitializeData from "../functions/useInitializeData.tsx";

export const TasksRoute = '/tasks';
export const AuthenticationRoute = "/";
export const PasswordSignInRoute = 'pass/';

export const TotpRoute = "totp/";



export default function ApplicationRoutes() {
    useInitializeData();

    return (
        <Routes>
            <Route element={<MainLayout />}>
                <Route element={<Authentication />} path={AuthenticationRoute} >
                    <Route element={<TotpAuth />} path={TotpRoute} />
                    <Route element={<SignInWithPassword />} path={PasswordSignInRoute} />
                </Route>
                <Route
                    path={TasksRoute}
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