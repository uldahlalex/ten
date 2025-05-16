import { ErrorBoundary } from "react-error-boundary";
import ApplicationRoutes from "./ApplicationRoutes.tsx";
import {DevTools} from "jotai-devtools";
const prod = import.meta.env.PROD
import 'jotai-devtools/styles.css'

function App() {
    return (
        <>
            <ErrorBoundary
                fallback={<></>}  
                onError={(error, errorInfo) => {
                    console.log('Caught an error:', error, errorInfo);
                }}
            >
                <ApplicationRoutes />
            </ErrorBoundary>
            {!prod && <DevTools />}
        </>
    );
}

export default App;