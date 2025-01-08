using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestLessThanFunction
    {
        [Test]
        public void TestLessThan_NumericComparison()
        {
            var exp = "1 < 2";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestLessThan_Strings()
        {
            var exp = "'apple' < 'banana'";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestLessThan_NullParameter()
        {
            var exp = "null < 5";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestLessThan_ParameterCountMismatch()
        {
            var exp = "1 < 2 < 3";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestLessThan_IncompatibleTypes()
        {
            var exp = "'5' < 5";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
