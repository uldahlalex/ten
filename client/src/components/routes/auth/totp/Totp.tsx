import {Outlet, useLocation, useNavigate} from "react-router-dom";
import React from "react";
import {TotpRegisterRoute, TotpSignInRoute} from "../../../ApplicationRoutes.tsx";

export default function Totp() {
    
    const navigate = useNavigate();
    const route = useLocation();
    
    return(<>
        <button onClick={() => navigate(TotpRegisterRoute)}>I want to register new device</button>
        <button onClick={() => navigate(TotpSignInRoute)}>I already have added to authenticator app</button>
        <Outlet />


    </>)
}