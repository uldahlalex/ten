import { atomWithStorage } from 'jotai/utils';
import { JwtResponse } from '@/models';

export const JwtAtom = atomWithStorage<JwtResponse | undefined>('jwt', undefined);