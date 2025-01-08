using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.List
{
    public class TestReverseListFunction
    {
        [Test]
        public void TestReverseValidList()
        {
            var exp = "Reverse([1,2,3])";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.EquivalentTo(new[] { 3, 2, 1 }));
        }

        [Test]
        public void TestReverseEmptyList()
        {
            var exp = "Reverse([])";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.EquivalentTo(new object[] {}));
        }

        [Test]
        public void TestReverseNullList()
        {
            var exp = "Reverse(null)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestReverseInvalidParameterType()
        {
            var exp = "Reverse(123)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestReverseInvalidParameterCount()
        {
            var exp = "Reverse([1,2,3], 4)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
