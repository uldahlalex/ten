import ApplicationRoutes from "./ApplicationRoutes.tsx";
import {DevTools} from "jotai-devtools";
import 'jotai-devtools/styles.css';

const baseUrl = import.meta.env.VITE_API_BASE_URL
const prod = import.meta.env.PROD


export default function App() {
    

    return (<>
            <ApplicationRoutes/>
        {!prod && <DevTools/>}
    </>)
}