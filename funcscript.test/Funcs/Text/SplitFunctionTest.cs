using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs
{
    public class TestSplitFunction
    {
        [Test]
        public void TestSplitValidInput()
        {
            var exp = "split('a,b,c', ',')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.EquivalentTo(new []{"a", "b", "c"}));
        }

        [Test]
        public void TestSplitNoParameters()
        {
            var exp = "split()";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestSplitInvalidFirstParameter()
        {
            var exp = "split(123, ',')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSplitEmptyString()
        {
            var exp = "split('', ',')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.EquivalentTo(new []{""}));
        }
        
        [Test]
        public void TestSplitWithEmptySeparator()
        {
            var exp = "split('abc', '')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsList);
            var list = (FsList)res;
            Assert.That(list, Is.EquivalentTo(new [] {"abc"}));
        }
    }
}
