using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.List
{
    public class TestSeriesFunction
    {
        [Test]
        public void TestSeriesValidParameters()
        {
            var exp = "Series(1, 5)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.EquivalentTo(new []{1, 2, 3, 4, 5}));
        }

        [Test]
        public void TestSeriesInvalidParameterCount()
        {
            var exp = "Series(1)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestSeriesInvalidStartParameterType()
        {
            var exp = "Series('start', 5)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSeriesInvalidCountParameterType()
        {
            var exp = "Series(1, 'count')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
