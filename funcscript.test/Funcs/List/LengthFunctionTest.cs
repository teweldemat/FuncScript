using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.List
{
    public class TestLengthFunction
    {
        [Test]
        public void TestLengthOfList()
        {
            var exp = "Len([1,2,3,4])";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(4));
        }

        [Test]
        public void TestLengthOfString()
        {
            var exp = "Len('Hello World')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(11));
        }

        [Test]
        public void TestLengthOfEmptyList()
        {
            var exp = "Len([])";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(0));
        }

        [Test]
        public void TestLengthOfNull()
        {
            var exp = "Len(null)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(0));
        }

        [Test]
        public void TestLengthWithInvalidParameterCount()
        {
            var exp = "Len([1, 2], 'extra')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestLengthWithInvalidType()
        {
            var exp = "Len(123)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
