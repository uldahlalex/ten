import {RouteObject} from 'react-router-dom';
import {MainLayout} from "./MainLayout";
import Authentication from "./routes/auth/Authentication";
import Totp from "./routes/auth/totp/Totp";
import TotpSignIn from "./routes/auth/totp/signin/TotpSignIn";
import TotpRegister from "./routes/auth/totp/register/TotpRegister";
import {ProtectedRoute} from "./ProtectedRoute";
import TaskList from "./routes/tasklist/TaskList";
import SignInWithPassword from "./routes/auth/pass/SignInWithPassword";
import { mainLayoutPath } from './routes';

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

    }];