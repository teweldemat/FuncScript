"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.IsIdentifierFirstChar = IsIdentifierFirstChar;
function IsIdentifierFirstChar(ch) {
    return /^[a-zA-Z]$/.test(ch) || ch === '_';
}
