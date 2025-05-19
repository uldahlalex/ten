import {Outlet, useLocation, useNavigate} from "react-router-dom";
import React from "react";
import {TotpRegisterRoute, TotpSignInRoute} from "../../../ApplicationRoutes.tsx";

export default function Totp() {
    
    const navigate = useNavigate();
    const location = useLocation();
    
    return(<div className="flex flex-col justify-center">
        <button className="btn btn-primary" onClick={() => navigate(TotpSignInRoute)}>
            I have the authenticator code on my device</button>


        <button className="btn btn-accent" onClick={() => navigate(TotpRegisterRoute)}>
                I want to register new device</button>
     
        


    </div>)
}