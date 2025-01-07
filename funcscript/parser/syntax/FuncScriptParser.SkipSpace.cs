namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        record SkipSpaceResult(int NextIndex);

        static SkipSpaceResult SkipSpace(ParseContext context, int index)
        {
            int i = index;
            var expression = context.Expression;
            while (index < expression.Length)
            {
                if (IsCharWhiteSpace(expression[index]))
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
