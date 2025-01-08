using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestGreaterThanFunction
    {
        [Test]
        public void TestGreaterThanWithValidInputs()
        {
            var exp = "3 > 2";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestGreaterThanWithEqualInputs()
        {
            var exp = "2 > 2";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestGreaterThanWithInvalidCount()
        {
            var exp = "1 > 2 > 3";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestGreaterThanWithNullInputs()
        {
            var exp = "null > 5";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestGreaterThanWithIncompatibleTypes()
        {
            var exp = "'apple' > 5";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestGreaterThanWithDifferentTypes()
        {
            var exp = "3.14 > 'banana'";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
