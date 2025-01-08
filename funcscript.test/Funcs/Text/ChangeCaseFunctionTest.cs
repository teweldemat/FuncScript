using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Text
{
    public class TestChangeCaseFunction
    {
        [Test]
        public void TestChangeCaseLower()
        {
            var exp = "changecase('Hello World', 'lower')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("hello world"));
        }

        [Test]
        public void TestChangeCaseUpper()
        {
            var exp = "changecase('Hello World', 'upper')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("HELLO WORLD"));
        }

        [Test]
        public void TestChangeCasePascal()
        {
            var exp = "changecase('hello world', 'pascal')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("HelloWorld"));
        }

        [Test]
        public void TestChangeCaseSnake()
        {
            var exp = "changecase('Hello World', 'snake')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("hello_world"));
        }

        [Test]
        public void TestChangeCaseKebab()
        {
            var exp = "changecase('Hello World', 'kebab')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("hello-world"));
        }

        [Test]
        public void TestChangeCaseInvalidType()
        {
            var exp = "changecase('Hello World', 'invalid')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestChangeCaseMissingParameters()
        {
            var exp = "changecase('Hello World')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestChangeCaseNullInput()
        {
            var exp = "changecase(null, 'upper')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestChangeCaseNullCaseType()
        {
            var exp = "changecase('Hello World', null)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
