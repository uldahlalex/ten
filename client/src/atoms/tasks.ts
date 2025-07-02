import { atom } from 'jotai';
import { TasklistDto, TagDto, TickticktaskDto, GetTasksFilterAndOrderParameters } from '@/models';

export const ListsAtom = atom<TasklistDto[]>([]);
export const TagsAtom = atom<TagDto[]>([]);
export const CurrentTasksDisplayView = atom<TickticktaskDto[]>([]);

export const QueryParametersAtom = atom<GetTasksFilterAndOrderParameters>({
    tagIds: [],
    listIds: [],
    earliestDueDate: undefined,
    isCompleted: undefined,
    isDescending: undefined,
    latestDueDate: undefined,
    maxPriority: undefined,
    minPriority: undefined,
    orderBy: undefined,
    searchTerm: undefined
});