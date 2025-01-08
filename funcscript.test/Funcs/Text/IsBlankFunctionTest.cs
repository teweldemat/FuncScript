using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Text
{
    public class TestIsBlankFunction
    {
        [Test]
        public void TestIsBlankWithNull()
        {
            var exp = "isBlank(null)";
            var res = Helpers.Evaluate(exp);
            Assert.IsTrue((bool)res);
        }

        [Test]
        public void TestIsBlankWithEmptyString()
        {
            var exp = "isBlank('')";
            var res = Helpers.Evaluate(exp);
            Assert.IsTrue((bool)res);
        }

        [Test]
        public void TestIsBlankWithBlankString()
        {
            var exp = "isBlank('   ')";
            var res = Helpers.Evaluate(exp);
            Assert.IsTrue((bool)res);
        }

        [Test]
        public void TestIsBlankWithNonBlankString()
        {
            var exp = "isBlank('Hello')";
            var res = Helpers.Evaluate(exp);
            Assert.IsFalse((bool)res);
        }

        [Test]
        public void TestIsBlankWithInvalidType()
        {
            var exp = "isBlank(123)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestIsBlankWithNoParameters()
        {
            var exp = "isBlank()";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
