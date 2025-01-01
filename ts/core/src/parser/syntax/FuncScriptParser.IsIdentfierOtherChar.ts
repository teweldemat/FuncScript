export function IsIdentfierOtherChar(ch: string): boolean {
    return /\w/.test(ch) || ch === '_';
}
