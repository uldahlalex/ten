import {Outlet, useNavigate} from "react-router-dom";
import {AuthenticationRoute, PasswordSignInRoute, TotpRoute} from "../ApplicationRoutes.tsx";

export default function Authentication() {

    const navigate = useNavigate();
    
    function navigateToTotp() {
        navigate(AuthenticationRoute + TotpRoute)
    }

    function navigateToPassword() {
        navigate(AuthenticationRoute + PasswordSignInRoute)

    }

    return(<>
        <div className="flex flex-col items-center justify-center h-screen">
            {
             location.pathname != AuthenticationRoute + TotpRoute &&   <button className="btn btn-primary"  onClick={navigateToTotp}>I want to sign in with authenticator</button>
            }
            {
                location.pathname != AuthenticationRoute + PasswordSignInRoute &&
            <button className="btn btn-primary" onClick={navigateToPassword}>I want to sign in with password</button>
            }   
            <Outlet/>
        </div>
      
        
   
    
    </>);
}