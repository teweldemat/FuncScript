using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.List
{
    public class TestFilterListFunction
    {
        [Test]
        public void TestFilterEvenNumbers()
        {
            var exp = "Filter([1, 2, 3, 4], (x) => x % 2 = 0)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.EquivalentTo(new []{2, 4}));
        }

        [Test]
        public void TestFilterEmptyList()
        {
            var exp = "Filter([], (x) => x > 0)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.Empty);
        }

        [Test]
        public void TestFilterWithInvalidParameterCount()
        {
            var exp = "Filter([1, 2])"; // Missing the filter function
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestFilterWithInvalidFirstParameter()
        {
            var exp = "Filter('not_a_list', (x) => x > 0)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestFilterWithInvalidSecondParameter()
        {
            var exp = "Filter([1, 2, 3], 42)"; // Second parameter is not a function
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
