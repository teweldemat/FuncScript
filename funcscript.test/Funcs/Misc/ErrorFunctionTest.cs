using FuncScript.Core;
using FuncScript.Error;
using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Misc
{
    public class TestErrorFunction
    {
        [Test]
        public void TestErrorWithSingleStringParameter()
        {
            var exp = "error('This is an error message')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_DEFAULT));
        }

        [Test]
        public void TestErrorWithNonStringParameter()
        {
            var exp = "error(123)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestErrorWithNoParameters()
        {
            var exp = "error()";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestErrorWithMultipleParameters()
        {
            var exp = "error('first', 'second')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_DEFAULT));
        }
    }
}
