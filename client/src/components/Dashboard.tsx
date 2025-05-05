import { useState } from "react";
import Sidebar from "./Sidebar.tsx";
import TaskList from "./TaskList.tsx";

export default function Dashboard() {
    const [deviceLogs] = useState<any[]>([]);

    return (
        <div className="flex h-screen"> 
            <div className="w-64 h-full border-r border-gray-200 bg-white"> 
                <Sidebar />
            </div>
            <div className="flex-1 h-full"> 
                <TaskList />
            </div>
        </div>
    );
}