using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestIsErrorFunction
    {
        [Test]
        public void TestIsErrorWithError()
        {
            var exp = "isError(Error('test error'))";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is bool);
            Assert.That((bool)res, Is.True);
        }

        [Test]
        public void TestIsErrorWithNonErrorObject()
        {
            var exp = "isError(42)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is bool);
            Assert.That((bool)res, Is.False);
        }

        [Test]
        public void TestIsErrorWithNull()
        {
            var exp = "isError(null)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is bool);
            Assert.That((bool)res, Is.False);
        }

        [Test]
        public void TestIsErrorWithInvalidParameterCount()
        {
            var exp = "isError()";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestIsErrorWithMultipleParameters()
        {
            var exp = "isError(1, 2)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
