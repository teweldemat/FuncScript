import { ParseContext, ParseResult } from "../FuncScriptParser.Main";
import { GetFSTemplate } from "./FuncScriptParser.GetFSTemplate";

export function ParseFsTemplate(context: ParseContext): ParseResult {
    const result = GetFSTemplate(context, 0);
    return result;
}
