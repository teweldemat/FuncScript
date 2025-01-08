using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.List
{
    public class TestTakeFunction
    {
        [Test]
        public void TestTakeValid()
        {
            var exp = "Take([1, 2, 3, 4], 2)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.EquivalentTo(new[] { 1, 2 }));
        }

        [Test]
        public void TestTakeParameterCountMismatch()
        {
            var exp = "Take([1, 2, 3])";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestTakeInvalidFirstParameter()
        {
            var exp = "Take(123, 2)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestTakeInvalidSecondParameter()
        {
            var exp = "Take([1, 2, 3], 'two')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestTakeNegativeNumber()
        {
            var exp = "Take([1, 2, 3, 4], -1)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.EquivalentTo(new object[] { }));
        }

        [Test]
        public void TestTakeExceedingLength()
        {
            var exp = "Take([1, 2], 5)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.EquivalentTo(new[] { 1, 2 }));
        }
    }
}
