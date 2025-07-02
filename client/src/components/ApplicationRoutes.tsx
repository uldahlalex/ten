import {createBrowserRouter, RouterProvider} from 'react-router-dom';
import { useInitializeData } from "@/hooks";
import { routes } from './routeConfig';


function ApplicationRoutes() {
    useInitializeData();

    const router = createBrowserRouter(routes);

    return <>
        <RouterProvider router={router}/>
    </>
}

export default ApplicationRoutes;