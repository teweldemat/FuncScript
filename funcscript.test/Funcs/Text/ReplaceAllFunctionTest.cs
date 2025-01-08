using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Text
{
    public class TestReplaceAllFunction
    {
        [Test]
        public void TestReplaceAllSuccessful()
        {
            var exp = "replaceall('hello world', 'world', 'FuncScript')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("hello FuncScript"));
        }

        [Test]
        public void TestReplaceAllWithEmptyInput()
        {
            var exp = "replaceall('', 'world', 'FuncScript')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(""));
        }

        [Test]
        public void TestReplaceAllWithEmptySearch()
        {
            var exp = "replaceall('hello world', 'world', 'FuncScript')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("hello FuncScript")); // Inserting "FuncScript" before each character
        }

        [Test]
        public void TestReplaceAllWithErrorNotEnoughParameters()
        {
            var exp = "replaceall('hello world', 'world')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestReplaceAllWithNullInput()
        {
            var exp = "replaceall(null, 'world', 'FuncScript')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestReplaceAllWithNullSearch()
        {
            var exp = "replaceall('hello world', null, 'FuncScript')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestReplaceAllWithNullReplacement()
        {
            var exp = "replaceall('hello world', 'world', null)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
