using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.List
{
    public class TestFindFirstFunction
    {
        [Test]
        public void TestFindFirstValidCase()
        {
            var exp = "First([1, 2, 3], (x) => x > 1)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(2));
        }

        [Test]
        public void TestFindFirstNoMatch()
        {
            var exp = "First([1, 2, 3], (x) => x > 4)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestFindFirstInvalidParameterCount()
        {
            var exp = "First([1, 2])"; // Missing the second parameter
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestFindFirstFirstParameterInvalidType()
        {
            var exp = "First(1, (x) => x > 1)"; // First parameter is not a list
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestFindFirstSecondParameterInvalidType()
        {
            var exp = "First([1, 2, 3], 1)"; // Second parameter is not a function
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
