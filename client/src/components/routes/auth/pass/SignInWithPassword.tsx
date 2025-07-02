import {authClient} from "../../../../apiControllerClients";
import toast from "react-hot-toast";
import {useAtom} from "jotai";
import {JwtAtom} from "@/atoms";
import {useNavigate} from "react-router-dom";
import {TaskListRoute} from "../../../routes";
import {useState} from "react";
import {AuthRequestDto} from "@/models";

export default function SignInWithPassword() {

    const [, setJwt] = useAtom(JwtAtom);
    const [registerForm, setRegisterForm] = useState<AuthRequestDto>({
        email: '',
        password: '',
    })
    const [passwordRepeat, setPasswordRepeat] = useState<string>('');
    const navigate = useNavigate();

    return (<div className="flex flex-col items-center justify-center h-screen">
        <h1 className="text-3xl font-bold mb-4">Register with password</h1>

        <form className="space-y-4" onSubmit={(e) => {
            e.preventDefault();
            if (registerForm.password !== passwordRepeat) {
                toast.error("Passwords do not match");
                return;
            }
            authClient.register((registerForm)).then(r => {
                toast("welcome!")
                setJwt(r);
                navigate(TaskListRoute)
            })
        }}>
            <div className="form-control w-full">
                <label className="label">
                    <span className="label-text">Email</span>
                </label>
                <input
                    type="email"
                    className="input input-bordered w-full"
                    value={registerForm.email}
                    onChange={(e) => setRegisterForm({...registerForm, email: e.target.value})}
                    required
                />
            </div>

            <div className="form-control w-full">
                <label className="label">
                    <span className="label-text">Password</span>
                </label>
                <input
                    type="password"
                    className="input input-bordered w-full"
                    value={registerForm.password}
                    onChange={(e) => setRegisterForm({...registerForm, password: e.target.value})}
                    required
                />

            </div>
            <div className="form-control w-full">
                <label className="label">
                    <span className="label-text">Repeat Password</span>
                </label>
                <input
                    type="password"
                    className="input input-bordered w-full"
                    value={passwordRepeat}
                    onChange={(e) => setPasswordRepeat(e.target.value)}
                    required
                />

                <button type="submit" className="btn btn-primary w-full">Register</button>
            </div>

        </form>


        <div>
            <h1>test login button below:</h1>
            <button className="btn btn-primary" onClick={() =>
                authClient.login(({email: "test@user.dk", password: "abc"})).then(r => {
                    toast("welcome!")
                    setJwt(r);
                    navigate(TaskListRoute)
                })}>Click to log in as a test user (this is just for local development, ill change later)
            </button>

        </div>
    </div>)


}