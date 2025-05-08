import {atom} from 'jotai';
import {TagDto, TasklistDto, TickticktaskDto} from "./generated-client.ts";
import {atomWithStorage} from "jotai/utils";

export const JwtAtom = atomWithStorage<string>('jwt', localStorage.getItem('jwt') || '');

export const ListsAtom = atom<TasklistDto[]>([]);
export const TagsAtom = atom<TagDto[]>([]);

export const CurrentTasksDisplayView = atom<TickticktaskDto[]>([]);