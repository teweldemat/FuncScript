using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Text
{
    public class TestUppercaseFunction
    {
        [Test]
        public void TestUppercaseValidString()
        {
            var exp = "uppercase('hello')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("HELLO"));
        }

        [Test]
        public void TestUppercaseEmptyString()
        {
            var exp = "uppercase('')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(string.Empty));
        }

        [Test]
        public void TestUppercaseParameterCountMismatch()
        {
            var exp = "uppercase()";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestUppercaseInvalidParameterType()
        {
            var exp = "uppercase(123)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
