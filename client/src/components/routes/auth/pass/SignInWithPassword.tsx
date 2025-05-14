import {authClient} from "../../../../apiControllerClients.ts";
import toast from "react-hot-toast";
import {useAtom} from "jotai";
import {JwtAtom} from "../../../../atoms/atoms.ts";
import {AuthRequestDto} from "../../../../generated-client.ts";
import {useNavigate} from "react-router-dom";
import {TaskListRoute} from "../../../ApplicationRoutes.tsx";

export default function SignInWithPassword() {

    const [, setJwt] = useAtom(JwtAtom);
    const navigate = useNavigate();

    return (<div className="flex flex-col items-center justify-center h-screen">
        <button className="btn btn-primary" onClick={() =>
            authClient.login(new AuthRequestDto({email: "test@user.dk", password: "abc"})).then(r => {
                toast("welcome!")
                setJwt(r);
                navigate(TaskListRoute)
            })}>Click to log in as a test user
        </button>
    </div>)


}