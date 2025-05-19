import {createBrowserRouter, RouteObject, RouterProvider} from 'react-router-dom';
import {MainLayout} from "./MainLayout.tsx";
import Authentication from "./routes/auth/Authentication.tsx";
import Totp from "./routes/auth/totp/Totp.tsx";
import TotpSignIn from "./routes/auth/totp/signin/TotpSignIn.tsx";
import TotpRegister from "./routes/auth/totp/register/TotpRegister.tsx";
import {ProtectedRoute} from "./ProtectedRoute.tsx";
import TaskList from "./routes/tasklist/TaskList.tsx";
import useInitializeData from "../functions/useInitializeData.tsx";
import SignInWithPassword from "./routes/auth/pass/SignInWithPassword.tsx";

// Update your route constants
export const mainLayoutPath = "/";
export const AuthenticationRoute = "/auth";
export const TotpRoute = "/auth/totp";
export const PasswordSignInRoute = "/auth/password";
export const TotpSignInRoute = "/auth/totp/signin";
export const TotpRegisterRoute = "/auth/totp/register";
export const TaskListRoute = "/tasks";


export const routes: RouteObject[] = [
    {
        path: mainLayoutPath,
        Component: MainLayout,
        handle: {label: 'Home'},
        children: [
            {
                index: true, // This makes it the default route
                Component: Authentication,
                handle: {label: 'Authentication'},
            },
            {
                path: 'auth',
                Component: Authentication,
                handle: {label: 'Authentication'}
            },

            {
                path: 'auth/totp',
                Component: Totp,
                handle: {label: 'TOTP Authentication'},
            },
            {
                path: 'auth/totp/register',
                Component: TotpRegister,
                handle: {label: 'Register TOTP'},
            },
            {
                path: 'auth/totp/signin',
                Component: TotpSignIn,
                handle: {label: 'Sign In with TOTP'},
            },
            {
                path: 'auth/password',
                Component: SignInWithPassword,
                handle: {label: 'Password Sign In'},
            },
            
            {
                path: 'tasks',
                element: <ProtectedRoute><TaskList/></ProtectedRoute>,
                handle: {label: 'Tasks'},
            }
        ]

    }]


function ApplicationRoutes() {
    useInitializeData();

    const router = createBrowserRouter(routes);

    return <>
        <RouterProvider router={router}/>
    </>
}

export default ApplicationRoutes;