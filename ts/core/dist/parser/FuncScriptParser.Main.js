"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.s_KeyWords = exports.KW_ERROR = exports.KW_SWITCH = exports.KW_CASE = exports.KW_RETURN = exports.s_prefixOp = exports.s_operatorSymols = exports.ParseNode = exports.SyntaxErrorData = exports.ParseNodeType = void 0;
var ParseNodeType;
(function (ParseNodeType) {
    ParseNodeType[ParseNodeType["Comment"] = 0] = "Comment";
    ParseNodeType[ParseNodeType["FunctionParameterList"] = 1] = "FunctionParameterList";
    ParseNodeType[ParseNodeType["FunctionCall"] = 2] = "FunctionCall";
    ParseNodeType[ParseNodeType["MemberAccess"] = 3] = "MemberAccess";
    ParseNodeType[ParseNodeType["Selection"] = 4] = "Selection";
    ParseNodeType[ParseNodeType["InfixExpression"] = 5] = "InfixExpression";
    ParseNodeType[ParseNodeType["LiteralInteger"] = 6] = "LiteralInteger";
    ParseNodeType[ParseNodeType["KeyWord"] = 7] = "KeyWord";
    ParseNodeType[ParseNodeType["LiteralDouble"] = 8] = "LiteralDouble";
    ParseNodeType[ParseNodeType["LiteralLong"] = 9] = "LiteralLong";
    ParseNodeType[ParseNodeType["Identifier"] = 10] = "Identifier";
    ParseNodeType[ParseNodeType["IdentifierList"] = 11] = "IdentifierList";
    ParseNodeType[ParseNodeType["Operator"] = 12] = "Operator";
    ParseNodeType[ParseNodeType["LambdaExpression"] = 13] = "LambdaExpression";
    ParseNodeType[ParseNodeType["ExpressionInBrace"] = 14] = "ExpressionInBrace";
    ParseNodeType[ParseNodeType["LiteralString"] = 15] = "LiteralString";
    ParseNodeType[ParseNodeType["StringTemplate"] = 16] = "StringTemplate";
    ParseNodeType[ParseNodeType["KeyValuePair"] = 17] = "KeyValuePair";
    ParseNodeType[ParseNodeType["KeyValueCollection"] = 18] = "KeyValueCollection";
    ParseNodeType[ParseNodeType["List"] = 19] = "List";
    ParseNodeType[ParseNodeType["Key"] = 20] = "Key";
    ParseNodeType[ParseNodeType["Case"] = 21] = "Case";
    ParseNodeType[ParseNodeType["GeneralInfixExpression"] = 22] = "GeneralInfixExpression";
    ParseNodeType[ParseNodeType["PrefixOperatorExpression"] = 23] = "PrefixOperatorExpression";
    ParseNodeType[ParseNodeType["ReturnExpression"] = 24] = "ReturnExpression";
})(ParseNodeType || (exports.ParseNodeType = ParseNodeType = {}));
class SyntaxErrorData {
    constructor(loc, length, message) {
        this.Loc = loc;
        this.Message = message;
        this.Length = length;
    }
}
exports.SyntaxErrorData = SyntaxErrorData;
class ParseNode {
    constructor(nodeType, pos, length, children = []) {
        this.NodeType = nodeType;
        this.Pos = pos;
        this.Length = length;
        this.Children = children;
    }
}
exports.ParseNode = ParseNode;
exports.s_operatorSymols = [
    ["^"],
    ["*", "/", "%"],
    ["+", "-"],
    [">=", "<=", "!=", ">", "<", "in"],
    ["=", "??", "?!", "?."],
    ["or", "and"],
    ["|"],
    [">>"]
];
exports.s_prefixOp = [
    ["!", "not"],
    ["-", "neg"]
];
exports.KW_RETURN = "return";
exports.KW_CASE = "case";
exports.KW_SWITCH = "switch";
exports.KW_ERROR = "fault";
exports.s_KeyWords = new Set();
(function initKeyWords() {
    exports.s_KeyWords.add(exports.KW_RETURN);
    exports.s_KeyWords.add(exports.KW_ERROR);
    exports.s_KeyWords.add(exports.KW_CASE);
    exports.s_KeyWords.add(exports.KW_SWITCH);
    exports.s_KeyWords.add(exports.KW_SWITCH);
})();
