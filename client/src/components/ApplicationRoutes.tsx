import {Route, Routes, useNavigate} from "react-router";
import useInitializeData from "../hooks/useInitializeData.tsx";
import {DashboardRoute, SignInRoute} from '../routeConstants.ts';
import Dock from "./Dock.tsx";
import SignIn from "./SignIn.tsx";
import {useEffect} from "react";
import {useAtom} from "jotai";
import {JwtAtom} from "../atoms.ts";
import toast from "react-hot-toast";
import Dashboard from "./Dashboard.tsx";

export default function ApplicationRoutes() {
    
    const navigate = useNavigate();
    const [jwt] = useAtom(JwtAtom);
    useInitializeData();

    useEffect(() => {
        if (jwt == undefined || jwt.length < 1) {
            navigate(SignInRoute)
            toast("Please sign in to continue")
        }
    }, [])
    
    return (<>
        {/*the browser router is defined in main tsx so that i can use useNavigate anywhere*/}
        <Routes>
    
            <Route element={<SignIn/>} path={SignInRoute}/>
            <Route element={<Dashboard/>} path={DashboardRoute}/>

        </Routes>
        <Dock/>

    </>)
}