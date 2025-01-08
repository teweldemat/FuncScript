using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestReplaceIfNullFunction
    {
        [Test]
        public void TestReplaceIfNull_WithNullFirstValue_ReturnsSecondValue()
        {
            var exp = "null ?? 42";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(42));
        }

        [Test]
        public void TestReplaceIfNull_WithNonNullFirstValue_ReturnsFirstValue()
        {
            var exp = "5 ?? 42";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(5));
        }

        [Test]
        public void TestReplaceIfNull_WithTwoNulls_ReturnsSecondNull()
        {
            var exp = "null ?? null";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(null));
        }

        [Test]
        public void TestReplaceIfNull_WithParameterCountMismatch_ReturnsError()
        {
            var exp = "null ??";
            Assert.Throws<SyntaxError>(() =>
            {
                var res = Helpers.Evaluate(exp);
            });
        }

        [Test]
        public void TestReplaceIfNull_WithTooManyParameters_ReturnsError()
        {
            var exp = "null ?? 42 ?? 100";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
