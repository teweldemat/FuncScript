using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test
{
    public class TestCosinFunction
    {
        [Test]
        public void TestCosinWithInteger()
        {
            var exp = "Cosin(0)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(1.0));
        }

        [Test]
        public void TestCosinWithDouble()
        {
            var exp = "Cosin(1.0)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(System.Math.Cos(1.0)));
        }

        [Test]
        public void TestCosinWithLong()
        {
            var exp = "Cosin(10L)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(System.Math.Cos(10.0)));
        }

        [Test]
        public void TestCosinWithMultipleParameters()
        {
            var exp = "Cosin(1, 2)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestCosinWithInvalidType()
        {
            var exp = "Cosin('string')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestCosinWithNull()
        {
            var exp = "Cosin(null)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
