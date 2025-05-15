import ApplicationRoutes from "./ApplicationRoutes.tsx";
import {DevTools} from "jotai-devtools";
import 'jotai-devtools/styles.css';
import EnhancedErrorBoundary from "./ErrorBoundary.tsx";

const prod = import.meta.env.PROD


export default function App() {
    

    return (<>
        <EnhancedErrorBoundary>
            <ApplicationRoutes/>
        </EnhancedErrorBoundary>
        
        {!prod && <DevTools/>}
    </>)
}