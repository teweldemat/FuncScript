using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Misc
{
    public class TestBreakFunction
    {
        [Test]
        public void TestBreakWithExtraData()
        {
            var exp = "break('some data')";
            var res = Helpers.Evaluate(exp);

            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.CONTROL_BREAK));
            Assert.That(error.ErrorData, Is.EqualTo("some data"));
        }

        [Test]
        public void TestBreakWithNoParameters()
        {
            var exp = "break()";
            var res = Helpers.Evaluate(exp);

            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestBreakWithTooManyParameters()
        {
            var exp = "break('data', 'extra')";
            var res = Helpers.Evaluate(exp);

            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
