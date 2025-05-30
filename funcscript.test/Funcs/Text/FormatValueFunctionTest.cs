using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Text
{
    public class TestFormatValueFunction
    {
        [Test]
        public void TestFormatWithSingleValue()
        {
            var exp = "format(42)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("42"));
        }

       

        [Test]
        public void TestFormatEmptyParameters()
        {
            var exp = "format()";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        

        [Test]
        public void TestFormatWithNonStringFormat()
        {
            var exp = "format(42, 123)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("42")); // Assuming it defaults to toString for non-string formats.
        }
    }
}
