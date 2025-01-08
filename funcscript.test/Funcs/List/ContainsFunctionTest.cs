using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.List
{
    public class TestContainsFunction
    {
        [Test]
        public void TestContainsInList_Positive()
        {
            var exp = "Contains([1, 2, 3], 2)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestContainsInList_Negative()
        {
            var exp = "Contains([1, 2, 3], 4)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestContainsInString_Positive()
        {
            var exp = "Contains('Hello World', 'world')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestContainsInString_Negative()
        {
            var exp = "Contains('Hello World', 'moon')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestContainsInvalidType_Container()
        {
            var exp = "Contains(123, 2)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestContainsInvalidType_Item()
        {
            var exp = "Contains([1, 2, 3], 'two')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res,Is.EqualTo(false));
        }

        [Test]
        public void TestContainsParameterCountMismatch()
        {
            var exp = "Contains([1, 2, 3])";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
