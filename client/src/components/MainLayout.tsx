import {useAtom} from "jotai";
import {JwtAtom} from "@/atoms";
import Sidebar from "./sidebar/Sidebar";
import {Outlet} from "react-router-dom";
import {Breadcrumbs} from "./Breadcrumbs";

export const MainLayout = () => {
    const [jwt] = useAtom(JwtAtom)

    return (
        <div className="flex h-screen">
            {jwt && jwt.jwt.length > 0 && (
                <div className="w-64 h-full border-r border-gray-200 bg-white">
                    <Sidebar/>
                </div>
            )}

            <div className="flex-1 flex flex-col h-max">
                <div className="flex-none">
                    <Breadcrumbs/>
                </div>

                <div className="flex-1 overflow-auto min-h-0 p-6">
                    <div className="bg-base-100 rounded-lg shadow h-full">
                        <Outlet/>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default MainLayout;