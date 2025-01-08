using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Text
{
    public class TestSubStringFunction
    {
        [Test]
        public void TestSubstringValid()
        {
            var exp = "substring('Hello World', 6, 5)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("World"));
        }

        [Test]
        public void TestSubstringNoParameters()
        {
            var exp = "substring()";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestSubstringFirstParamNotString()
        {
            var exp = "substring(123, 0, 3)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSubstringIndexOutOfRange()
        {
            var exp = "substring('Hello', 10)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSubstringNegativeCount()
        {
            var exp = "substring('Hello', 0, -1)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
