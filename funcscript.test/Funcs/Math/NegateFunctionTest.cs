using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Math
{
    public class TestNegateFunction
    {
        [Test]
        public void TestNegateInteger()
        {
            var exp = "neg(5)";
            var res = FuncScript.Evaluate(exp);
            Assert.AreEqual(-5, res);
        }

        [Test]
        public void TestNegateLong()
        {
            var exp = "neg(10000000000)";
            var res = FuncScript.Evaluate(exp);
            Assert.AreEqual(-10000000000, res);
        }

        [Test]
        public void TestNegateDouble()
        {
            var exp = "neg(3.14)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(-3.14).Within(0.00001));
        }

        [Test]
        public void TestNegateInvalidParameterCount()
        {
            var exp = "neg()";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.AreEqual(FsError.ERROR_PARAMETER_COUNT_MISMATCH, error.ErrorType);
        }

        [Test]
        public void TestNegateInvalidParameterType()
        {
            var exp = "neg('not a number')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.AreEqual(FsError.ERROR_TYPE_INVALID_PARAMETER, error.ErrorType);
        }
    }
}
