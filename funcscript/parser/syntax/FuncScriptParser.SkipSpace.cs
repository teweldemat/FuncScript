namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public static SkipSpaceResult SkipSpace(ParseContext context, int index)
        {
            int i = index;
            var expression = context.Expression;
            while (index < expression.Length)
            {
                if (isCharWhiteSpace(expression[index]))
                {
                    index++;
                }
                else
                {
                    var commentBlockResult = GetCommentBlock(context, index);
                    if (commentBlockResult.NextIndex == index)
                        break;
                    index = commentBlockResult.NextIndex;
                }
            }

            return new SkipSpaceResult(index);
        }
    }

    public record SkipSpaceResult(int NextIndex);
}