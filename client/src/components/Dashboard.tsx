import {DeviceLogsAtom} from "../atoms.ts";
import {useAtom} from 'jotai';
export default function Dashboard() {
    
    const [deviceLogs] = useAtom<DeviceLog>(DeviceLogsAtom);
    
    return (
        <div className="flex flex-col items-center justify-center h-screen">
        <h1 className="textarea-xl">Welcome to the Dashboard!</h1>
        <p className="textarea-lg">This is where you can manage your weather stations.</p>
            {
           
                        deviceLogs.map((log, index) => (
                            <div key={index} className="card">
                                <h2>Log {index + 1}</h2>
                                <p>{JSON.stringify(log)}</p>
                            </div>
                        ))
                   
            }
   
    </div>)
}