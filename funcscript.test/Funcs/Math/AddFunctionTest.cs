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
            var exp = "1 + 2 + 3";  // Changed to valid FuncScript syntax for addition
            var res = FuncScript.Evaluate(exp);
            Assert.AreEqual(6, res);
        }

        [Test]
        public void TestAddDoubles()
        {
            var exp = "1.5 + 2.3 + 3.2";  // Changed to valid FuncScript syntax for addition
            var res = FuncScript.Evaluate(exp);
            Assert.AreEqual(7.0, res);
        }

        [Test]
        public void TestAddStrings()
        {
            var exp = "'Hello, ' + 'World!'";  // Changed to valid FuncScript syntax for string concatenation
            var res = FuncScript.Evaluate(exp);
            Assert.AreEqual("Hello, World!", res);
        }

        [Test]
        public void TestAddMixedTypes()
        {
            var exp = "1 + ' apples' + 2";  // Changed to valid FuncScript syntax
            var res = FuncScript.Evaluate(exp);
            Assert.AreEqual("1 apples2", res);
        }

        [Test]
        public void TestAddList()
        {
            var exp = "[1, 2] + [3, 4]";  // Changed to valid FuncScript syntax for list addition
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsList);
            var sumList = (FsList)res;
            Assert.That(sumList, Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
        }

        [Test]
        public void TestAddInvalidType()
        {
            var exp = "1 + {}+ 3"; // This should remain the same if `add` function is implemented correctly.
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.AreEqual(FsError.ERROR_TYPE_INVALID_PARAMETER, error.ErrorType);
        }

        [Test]
        public void TestAddNegativeIndex()
        {
            var exp = "-1+ 2+ 3";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res,Is.EqualTo(4));
        }

        
    }
}
