using System.Text;
using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public static ExpressionBlockResult GetFSTemplate(ParseContext context, int index)
        {
            var parseNode = (ParseNode)null;
            var prog = (ExpressionBlock)null;
            var parts = new List<ExpressionBlock>();
            var nodeParts = new List<ParseNode>();

            var i = index;
            var sb = new StringBuilder();
            int i2;
            var lastIndex = i;
            while (true)
            {
                i2 = GetLiteralMatch(context, i, "$${").NextIndex;
                if (i2 > i)
                {
                    sb.Append("${");
                    i = i2;
                }

                i2 = GetLiteralMatch(context, i, "${").NextIndex;
                if (i2 > i)
                {
                    if (sb.Length > 0)
                    {
                        parts.Add(new LiteralBlock(sb.ToString()));
                        nodeParts.Add(new ParseNode(ParseNodeType.LiteralString, lastIndex, i - lastIndex));
                        sb = new StringBuilder();
                    }

                    i = i2;

                    i = SkipSpace(context, i).NextIndex;
                    var resultExpr = GetExpression(context, i);
                    var expr = resultExpr.Block;
                    var nodeExpr = resultExpr.ParseNode;
                    i2 = resultExpr.NextIndex;

                    if (i2 == i)
                    {
                        context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, "expression expected"));
                        return new ExpressionBlockResult(null, null, index);
                    }

                    i = SkipSpace(context, i).NextIndex;

                    expr.SetContext(context.ReferenceProvider);
                    parts.Add(expr);
                    nodeParts.Add(nodeExpr);
                    i = i2;

                    i2 = GetLiteralMatch(context, i, "}").NextIndex;
                    if (i2 == i)
                    {
                        context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, "'}' expected"));
                        return new ExpressionBlockResult(null, null, index);
                    }

                    i = i2;
                    lastIndex = i;
                    if (i < context.Expression.Length)
                        continue;
                    else
                        break;
                }

                sb.Append(context.Expression[i]);
                i++;
                if (i == context.Expression.Length)
                    break;
            }

            if (sb.Length > 0)
            {
                parts.Add(new LiteralBlock(sb.ToString()));
                nodeParts.Add(new ParseNode(ParseNodeType.LiteralString, lastIndex, i - lastIndex));
            }

            if (parts.Count == 0)
            {
                prog = new LiteralBlock("");
                parseNode = new ParseNode(ParseNodeType.LiteralString, index, i - index);
            }

            if (parts.Count == 1)
            {
                prog = parts[0];
                parseNode = nodeParts[0];
            }
            else
            {
                prog = new FunctionCallExpression
                {
                    Function = new LiteralBlock(context.ReferenceProvider.Get(TemplateMergeMergeFunction.SYMBOL)),
                    Parameters = parts.ToArray()
                };
                prog.SetContext(context.ReferenceProvider);
                parseNode = new ParseNode(ParseNodeType.StringTemplate, index, i - index, nodeParts);
            }

            return new ExpressionBlockResult(prog, parseNode, i);
        }
    }
}