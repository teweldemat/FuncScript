using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Misc
{
    public class TestLogFunction
    {
        [Test]
        public void TestLogFunctionWithNoParameters()
        {
            var exp = "log()";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestLogFunctionWithAnchorOnly()
        {
            var exp = "log(5)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(5));
        }

        [Test]
        public void TestLogFunctionWithValue()
        {
            var exp = "log(5, 'test value')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(5));
            // Verifying the log output would be implementation-specific and may require a logging framework setup.
        }

        [Test]
        public void TestLogFunctionWithFunctionAsSecondParameter()
        {
            var exp = "log(5, (x) => x + 3)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(5));
            // Verifying the log output would be implementation-specific and may require a logging framework setup.
        }
    }
}
