import { ParseContext } from "../FuncScriptParser.Main";
import { GetSpaceSeparatedStringListExpression } from "./FuncScriptParser.GetSpaceSepratedStringListExpression";

export function ParseSpaceSeparatedList(context: ParseContext): string[] {
    //this function is not supported for the TypeScript parser
    return [];
}