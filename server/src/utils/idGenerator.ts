import { customAlphabet } from "nanoid";

const ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
const DEFAULT_SIZE = 6;

export const ID_REGEX = new RegExp(`^[${ALPHABET}]{${DEFAULT_SIZE}}`);

export function generateId(): string {
    const nanoid = customAlphabet(ALPHABET, DEFAULT_SIZE);
    return nanoid();
}
