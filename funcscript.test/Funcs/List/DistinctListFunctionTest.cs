using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.List
{
    public class TestDistinctListFunction
    {
        [Test]
        public void TestDistinctWithDuplicates()
        {
            var exp = "Distinct([1, 2, 2, 3, 4, 4])";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
        }

        [Test]
        public void TestDistinctWithNull()
        {
            var exp = "Distinct([null, 1, 2, null, 3])";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.EquivalentTo(new object[] { null, 1, 2, 3 }));
        }

        [Test]
        public void TestDistinctEmptyList()
        {
            var exp = "Distinct([])";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.EquivalentTo(new object[] { }));
        }

        [Test]
        public void TestDistinctInvalidParameterCount()
        {
            var exp = "Distinct([1,2], [3])";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestDistinctInvalidParameterType()
        {
            var exp = "Distinct('not_a_list')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
