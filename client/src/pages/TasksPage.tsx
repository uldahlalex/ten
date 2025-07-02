import { TaskList, TaskListFilters } from "@/components";

export default function TasksPage() {
    return (
        <div className="container mx-auto p-4">
            <div className="mb-6">
                <TaskListFilters />
            </div>
            <TaskList />
        </div>
    );
}