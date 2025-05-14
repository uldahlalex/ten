import {authClient} from "../apiControllerClients.ts";
import toast from "react-hot-toast";
import {useAtom} from "jotai";
import {JwtAtom} from "../atoms.ts";
import {AuthRequestDto} from "../generated-client.ts";
import TotpLogin from "./TotpSignIn.tsx";
import {Outlet, Route, useLocation, useNavigate} from "react-router-dom";
import {SignInRoute, TotpRoute} from "../routeConstants.ts";

export default function SignIn() {

    const [jwt, setJwt] = useAtom(JwtAtom);
    const navigate = useNavigate();
    const location = useLocation();


    return (<div className="flex flex-col items-center justify-center h-screen">
        
          
                <button className="btn btn-primary" onClick={() =>
                    authClient.login(new AuthRequestDto({email: "test@user.dk", password: "abc"})).then(r => {
                        toast("welcome!")
                        setJwt(r);
                       
                    })}>Click to log in as a test user</button>


        {
        //     if location is totp route
            (location.pathname != SignInRoute + TotpRoute) &&  <button onClick={() => {
            navigate(SignInRoute + TotpRoute)
        }}>
        I want to sign in with authenticator
    </button>
        }
      
        <Outlet />

    </div>)
}