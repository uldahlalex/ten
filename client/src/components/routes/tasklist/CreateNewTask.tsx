import {CreateTaskRequestDto} from '../../../generated-client';
import React, {FormEvent, useState} from "react";
import {useAtom} from "jotai";
import {ListsAtom, TagsAtom} from "../../../atoms/atoms.ts";

interface CreateNewTaskProps {
    onSubmit: (task: CreateTaskRequestDto) => void;
    onCancel?: () => void;
}

export default function CreateNewTask({onSubmit, onCancel}: CreateNewTaskProps) {
    const [newTask, setNewTask] = useState<CreateTaskRequestDto>({
        description: "",
        dueDate: new Date().toISOString(),
        listId: '',
        priority: 1,
        tagsIds: [],
        title: ''
    });

    const [lists] = useAtom(ListsAtom);
    const [tags] = useAtom(TagsAtom);

    const handleSubmit = (e: FormEvent) => {
        e.preventDefault();
        onSubmit(newTask);
    };

    return (
        <div className="p-6 max-w-2xl mx-auto">
            <form className="space-y-4" onSubmit={handleSubmit}>
                <div className="form-control w-full">
                    <label className="label">
                        <span className="label-text">Title</span>
                    </label>
                    <input
                        type="text"
                        className="input input-bordered w-full"
                        value={newTask.title}
                        onChange={(e) => setNewTask({...newTask, title: e.target.value})}
                        required
                    />
                </div>

                <div className="form-control w-full">
                    <label className="label">
                        <span className="label-text">Description</span>
                    </label>
                    <textarea
                        className="textarea textarea-bordered h-24"
                        value={newTask.description}
                        onChange={(e) => setNewTask({...newTask, description: e.target.value})}
                    />
                </div>

                <div className="form-control w-full">
                    <label className="label">
                        <span className="label-text">Due Date</span>
                    </label>
                    <input
                        type="date"
                        className="input input-bordered w-full"
                        value={new Date(newTask.dueDate!)?.toISOString().split('T')[0]}
                        onChange={(e) => setNewTask({...newTask, dueDate: e.target.value})}
                        required
                    />
                </div>

                <div className="form-control w-full">
                    <label className="label">
                        <span className="label-text">List</span>
                    </label>
                    <select
                        className="select select-bordered w-full"
                        value={newTask.listId}
                        onChange={(e) => setNewTask({...newTask, listId: e.target.value})}
                        required
                    >
                        <option disabled value="">Select a list</option>
                        {lists.map((list) => (
                            <option key={list.listId} value={list.listId}>
                                {list.name}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="form-control w-full">
                    <label className="label">
                        <span className="label-text">Tags</span>
                    </label>
                    <select
                        className="select select-bordered w-full h-32"
                        multiple
                        value={newTask.tagsIds}
                        onChange={(e) => {
                            const selectedTags = Array.from(e.target.selectedOptions).map(option => option.value);
                            setNewTask({
                                ...newTask,
                                tagsIds: tags.filter(tag => selectedTags.includes(tag.tagId)).map(t => t.tagId)
                            });
                        }}
                    >
                        {tags.map((tag) => (
                            <option key={tag.tagId} value={tag.tagId}>
                                {tag.name}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="flex gap-2 justify-end mt-6">
                    {onCancel && (
                        <button
                            type="button"
                            className="btn btn-ghost"
                            onClick={onCancel}
                        >
                            Cancel
                        </button>
                    )}
                    <button type="submit" className="btn btn-primary">
                        Create Task
                    </button>
                </div>
            </form>
        </div>
    );
}