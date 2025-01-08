using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Math
{
    public class TestAddFunction
    {
        [Test]
        public void TestAddIntegers()
        {
            var exp = "add(1, 2, 3)";
            var res = FuncScript.Evaluate(exp);
            Assert.AreEqual(6, res);
        }

        [Test]
        public void TestAddDoubles()
        {
            var exp = "add(1.5, 2.3, 3.2)";
            var res = FuncScript.Evaluate(exp);
            Assert.AreEqual(7.0, res);
        }

        [Test]
        public void TestAddStrings()
        {
            var exp = "add('Hello, ', 'World!')";
            var res = FuncScript.Evaluate(exp);
            Assert.AreEqual("Hello, World!", res);
        }

        [Test]
        public void TestAddMixedTypes()
        {
            var exp = "add(1, ' apples', 2)";
            var res = FuncScript.Evaluate(exp);
            Assert.AreEqual("1 apples2", res);
        }

        [Test]
        public void TestAddList()
        {
            var exp = "add([1, 2], [3, 4])";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsList);
            var sumList = (FsList)res;
            Assert.That(sumList, Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
        }

        [Test]
        public void TestAddInvalidType()
        {
            var exp = "add(1, {}, 3)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.AreEqual(FsError.ERROR_TYPE_INVALID_PARAMETER, error.ErrorType);
        }

        [Test]
        public void TestAddNegativeIndex()
        {
            var exp = "add(-1, 2, 3)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.AreEqual(FsError.ERROR_DEFAULT, error.ErrorType);
        }

        [Test]
        public void TestAddEmptyList()
        {
            var exp = "add([])";
            var res = FuncScript.Evaluate(exp);
            Assert.AreEqual(0, res);
        }
    }
}
