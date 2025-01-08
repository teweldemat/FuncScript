using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.List
{
    public class TestAnyMatchFunction
    {
        [Test]
        public void TestAnyMatchWithValidParameters()
        {
            var exp = "Any([1, 2, 3, 4], (x) => x > 2)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestAnyMatchWithNoMatchingElement()
        {
            var exp = "Any([1, 2, 3], (x) => x > 3)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestAnyMatchWithEmptyList()
        {
            var exp = "Any([], (x) => x > 2)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestAnyMatchWithParameterCountMismatch()
        {
            var exp = "Any([1, 2, 3])"; // No filter function provided
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestAnyMatchWithInvalidFirstParameter()
        {
            var exp = "Any('not_a_list', (x) => x > 2)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestAnyMatchWithInvalidSecondParameter()
        {
            var exp = "Any([1, 2, 3], 'not_a_function')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
