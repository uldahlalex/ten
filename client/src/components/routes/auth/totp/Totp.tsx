import {Outlet, useLocation, useNavigate} from "react-router-dom";
import React from "react";
import {TotpRegisterRoute, TotpSignInRoute} from "../../../ApplicationRoutes.tsx";
import {pathIncludes, pathIsExactly} from "../../../../functions/pathIncludes.ts";

export default function Totp() {
    
    const navigate = useNavigate();
    const location = useLocation();
    
    return(<div>
        {
            !pathIsExactly(TotpRegisterRoute, location) &&             
            <button onClick={() => navigate(TotpRegisterRoute)}>I want to register</button>
        }
        {
            !pathIsExactly(TotpSignInRoute, location) &&
            <button onClick={() => navigate(TotpSignInRoute)}>I already have added the authenticator to my device</button>
        }
        <Outlet />


    </div>)
}