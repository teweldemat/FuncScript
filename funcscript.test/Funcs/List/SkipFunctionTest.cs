using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.List
{
    public class TestSkipFunction
    {
        [Test]
        public void TestSkipValidCases()
        {
            var exp = "Skip([1,2,3,4,5], 2)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EquivalentTo(new []{3, 4, 5}));

            exp = "Skip([10, 20, 30], 0)";
            res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EquivalentTo(new []{10, 20, 30}));

            exp = "Skip([1, 2, 3], 3)";
            res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EquivalentTo(new object[] { }));
        }

        [Test]
        public void TestSkipParameterCountMismatch()
        {
            var exp = "Skip([1, 2], 1, 3)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestSkipInvalidFirstParameter()
        {
            var exp = "Skip(123, 2)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSkipInvalidSecondParameter()
        {
            var exp = "Skip([1, 2, 3], 'two')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
