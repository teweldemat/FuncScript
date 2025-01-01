"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.IsIdentfierOtherChar = IsIdentfierOtherChar;
function IsIdentfierOtherChar(ch) {
    return /\w/.test(ch) || ch === '_';
}
