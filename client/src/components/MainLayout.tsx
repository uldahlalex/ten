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
        <div className="flex h-screen overflow-hidden">
            {jwt && jwt.jwt.length > 0 && (
                <div className="w-64 h-full border-r border-gray-200 bg-white">
                    <Sidebar/>
                </div>
            )}

            <div className="flex-1 flex flex-col h-max">
                <div className="flex-none">
                    <Breadcrumbs />
                </div>

                <div className="flex-1 overflow-auto min-h-0 p-6">
                    <div className="bg-base-100 rounded-lg shadow h-full">
                        <Outlet />
                    </div>
                </div>
            </div>
        </div>
    );
};

export default MainLayout;