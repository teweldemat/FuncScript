using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Text
{
    public class TestFindTextFunction
    {
        [Test]
        public void TestFindTextWithValidInputs()
        {
            var exp = "find('hello world', 'world')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(6));
        }

        [Test]
        public void TestFindTextWithStartingIndex()
        {
            var exp = "find('hello world', 'o', 5)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(7));
        }

        [Test]
        public void TestFindTextNotFound()
        {
            var exp = "find('hello world', 'test')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(-1));
        }

        [Test]
        public void TestFindTextWithInvalidParameterCount()
        {
            var exp = "find('hello world')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestFindTextWithNullFirstParameter()
        {
            var exp = "find(null, 'world')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestFindTextWithNonStringFirstParameter()
        {
            var exp = "find(123, 'world')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestFindTextWithNullSecondParameter()
        {
            var exp = "find('hello world', null)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestFindTextWithNonStringSecondParameter()
        {
            var exp = "find('hello world', 123)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestFindTextWithInvalidStartIndex()
        {
            var exp = "find('hello world', 'world', -1)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestFindTextWithOutOfRangeStartIndex()
        {
            var exp = "find('hello world', 'world', 50)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestFindTextWithInvalidThirdParameter()
        {
            var exp = "find('hello world', 'world', 'ten')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
