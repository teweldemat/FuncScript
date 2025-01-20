using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Math
{
    public class TestTanFunction
    {
        [Test]
        public void TestTanWithInteger()
        {
            var exp = "Tan(0)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(0.0));
        }

        [Test]
        public void TestTanWithDouble()
        {
            var exp = "Tan(1.5708/2)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(1.0).Within(0.0001));
        }

        [Test]
        public void TestTanWithLong()
        {
            var angle = System.Math.PI/ 4;
            var exp = $"Tan({angle})";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(1.0).Within(0.0001));
        }

        [Test]
        public void TestTanWithInvalidParameterCount()
        {
            var exp = "Tan(0, 1)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var err = (FsError)res;
            Assert.That(err.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestTanWithInvalidParameterType()
        {
            var exp = "Tan('text')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var err = (FsError)res;
            Assert.That(err.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestTanWithNullParameter()
        {
            var exp = "Tan(null)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var err = (FsError)res;
            Assert.That(err.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
