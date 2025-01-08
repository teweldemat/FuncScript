using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Text
{
    public class TestEndsWithFunction
    {
        [Test]
        public void TestEndsWithTrue()
        {
            var exp = "endswith('Hello World', 'World')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestEndsWithFalse()
        {
            var exp = "endswith('Hello World', 'Hello')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestEndsWithNullFirstParameter()
        {
            var exp = "endswith(null, 'World')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res,Is.EqualTo(false));
        }

        [Test]
        public void TestEndsWithNullSecondParameter()
        {
            var exp = "endswith('Hello World', null)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res,Is.EqualTo(false));
        }

        [Test]
        public void TestEndsWithInvalidParameterCount()
        {
            var exp = "endswith('Hello World')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That((res as FsError).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestEndsWithInvalidParameterTypeFirst()
        {
            var exp = "endswith(123, 'World')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That((res as FsError).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestEndsWithInvalidParameterTypeSecond()
        {
            var exp = "endswith('Hello World', 456)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That((res as FsError).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
