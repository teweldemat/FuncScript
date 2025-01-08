using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.List
{
    public class TestReduceListFunction
    {
        [Test]
        public void TestReduceSum()
        {
            var exp = "Reduce([1, 2, 3], (x, acc) => x + acc, 0)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(6));
        }

        [Test]
        public void TestReduceProduct()
        {
            var exp = "Reduce([1, 2, 3, 4], (x, acc) => x * acc, 1)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(24));
        }

        [Test]
        public void TestReduceWithInvalidFunction()
        {
            var exp = "Reduce([1, 2, 3], 'not_a_function')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestReduceWithInsufficientParameters()
        {
            var exp = "Reduce([1, 2, 3])";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestReduceWithInvalidListType()
        {
            var exp = "Reduce('not_a_list', (x, acc) => x + acc)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
