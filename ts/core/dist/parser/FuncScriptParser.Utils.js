"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.isDigit = isDigit;
exports.isCharWhiteSpace = isCharWhiteSpace;
function isDigit(char) {
    return /\d/.test(char);
}
function isCharWhiteSpace(ch) {
    return ch === ' ' ||
        ch === '\r' ||
        ch === '\t' ||
        ch === '\n';
}
