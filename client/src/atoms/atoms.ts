import {atom} from 'jotai';
import {
    GetTasksFilterAndOrderParameters,
    JwtResponse,
    TagDto,
    TasklistDto,
    TickticktaskDto
} from "../generated-client.ts";
import {atomWithStorage} from "jotai/utils";

export const JwtAtom = atomWithStorage<JwtResponse | undefined>('jwt', undefined);

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