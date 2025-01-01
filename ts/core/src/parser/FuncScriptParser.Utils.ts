export function isDigit(char: string): boolean {
    return /\d/.test(char);
}export function isCharWhiteSpace(ch: string): boolean {
    return ch === ' ' ||
           ch === '\r' ||
           ch === '\t' ||
           ch === '\n';
}