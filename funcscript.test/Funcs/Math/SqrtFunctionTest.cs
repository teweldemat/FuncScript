using FuncScript.Core;
using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Math
{
    public class TestSqrtFunction
    {
        [Test]
        public void TestSqrtPositiveInteger()
        {
            var exp = "Sqrt(4)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(2));
        }

        [Test]
        public void TestSqrtPositiveDouble()
        {
            var exp = "Sqrt(4.0)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(2.0));
        }

        [Test]
        public void TestSqrtZero()
        {
            var exp = "Sqrt(0)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(0));
        }

        [Test]
        public void TestSqrtNegativeInteger()
        {
            var exp = "Sqrt(-4)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSqrtNegativeDouble()
        {
            var exp = "Sqrt(-4.0)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSqrtInvalidParameterCount()
        {
            var exp = "Sqrt(4, 5)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestSqrtStringParameter()
        {
            var exp = "Sqrt('string')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSqrtNullParameter()
        {
            var exp = "Sqrt(null)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
