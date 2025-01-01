export function IsIdentifierFirstChar(ch: string): boolean {
    return /^[a-zA-Z]$/.test(ch) || ch === '_';
}