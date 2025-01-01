"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ParseFsTemplate = ParseFsTemplate;
const FuncScriptParser_GetFSTemplate_1 = require("./FuncScriptParser.GetFSTemplate");
function ParseFsTemplate(context) {
    const result = (0, FuncScriptParser_GetFSTemplate_1.GetFSTemplate)(context, 0);
    return result;
}
