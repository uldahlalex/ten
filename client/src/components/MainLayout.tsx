import {useAtom} from "jotai/index";
import {JwtAtom} from "../atoms/atoms.ts";
import Sidebar from "./sidebar/Sidebar.tsx";
import {Outlet, useNavigate} from "react-router-dom";
import {Breadcrumbs} from "./Breadcrumbs.tsx";
import {AuthenticationRoute, routes} from "./ApplicationRoutes.tsx";
import toast from "react-hot-toast";

export const MainLayout = () => {

    const [jwt] = useAtom(JwtAtom)
    const navigate = useNavigate();
    

    return (
        <div className="flex h-screen">

            {
                jwt && jwt.jwt.length > 0 && <>
                    <div className="w-64 h-full border-r border-gray-200 bg-white">
                        <Sidebar/>
                    </div>
                </>
            }

            <div className="flex-1 h-full">
                <main className="max-w-7xl mx-auto px-4 py-6 sm:px-6 lg:px-8">
                    <Breadcrumbs />

                    <div className="bg-white p-6 rounded-lg shadow">
                        <Outlet />
                    </div>
                </main>
            </div>
        </div>
    );
};