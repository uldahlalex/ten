import {useAtom} from "jotai/index";
import {JwtAtom} from "../atoms/atoms.ts";
import Sidebar from "./sidebar/Sidebar.tsx";
import {Outlet} from "react-router-dom";

export const MainLayout = () => {

    const [jwt] = useAtom(JwtAtom)

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
                <Outlet/>
            </div>
        </div>
    );
};