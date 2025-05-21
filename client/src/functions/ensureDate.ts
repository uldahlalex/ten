// export const ensureDate = (value: string | Date | undefined | null): Date | undefined => {
//     if (!value) return undefined;
//
//     // If it's already a valid Date, return it
//     if (isValidDate(value)) return value;
//
//     // Try to parse the string into a Date
//     const date = new Date(value);
//     return isValidDate(date) ? date : undefined;
// };
//
// export const isValidDate = (value: any): value is Date => {
//     return value instanceof Date && !isNaN(value.getTime());
// };
