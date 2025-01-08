using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Text
{
    public class TestJoinTextFunction
    {
        [Test]
        public void TestJoinTextValid()
        {
            var exp = "join(['Hello', 'World'], ', ')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Hello, World"));
        }

        [Test]
        public void TestJoinTextEmptyList()
        {
            var exp = "join([], ', ')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(""));
        }

        [Test]
        public void TestJoinTextWithNullElement()
        {
            var exp = "join(['Hello', null, 'World'], ', ')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Hello, World"));
        }

        [Test]
        public void TestJoinTextParameterCountMismatch()
        {
            var exp = "join(['Hello', 'World'])";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestJoinTextInvalidListParameter()
        {
            var exp = "join('NotAList', ', ')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestJoinTextInvalidSeparatorParameter()
        {
            var exp = "join(['Hello', 'World'], 123)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestJoinTextNullParameters()
        {
            var exp = "join(null, null)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
