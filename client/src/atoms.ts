import {atom} from 'jotai';
import {
    GetTasksFilterAndOrderParameters,
    IGetTasksFilterAndOrderParameters,
    TagDto,
    TasklistDto,
    TickticktaskDto
} from "./generated-client.ts";
import {atomWithStorage} from "jotai/utils";

export const JwtAtom = atomWithStorage<string>('jwt', localStorage.getItem('jwt') || '');

export const ListsAtom = atom<TasklistDto[]>([]);
export const TagsAtom = atom<TagDto[]>([]);

export const CurrentTasksDisplayView = atom<TickticktaskDto[]>([]);

export const QueryParametersAtom = atom<IGetTasksFilterAndOrderParameters>({
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