import ApplicationRoutes from "./ApplicationRoutes.tsx";
import {DevTools} from "jotai-devtools";
import 'jotai-devtools/styles.css';

const prod = import.meta.env.PROD


export default function App() {
    

    return (<>
            <ApplicationRoutes/>
        {!prod && <DevTools/>}
    </>)
}