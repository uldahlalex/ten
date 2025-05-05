import {atom} from 'jotai';
import {TagDto, TasklistDto, TickticktaskDto} from "./generated-client.ts";

export const JwtAtom = atom<string>(localStorage.getItem('jwt') || '')

export const ListsAtom = atom<TasklistDto[]>([]);
export const TagsAtom = atom<TagDto[]>([]);

export const CurrentTasksDisplayView = atom<TickticktaskDto[]>([]);