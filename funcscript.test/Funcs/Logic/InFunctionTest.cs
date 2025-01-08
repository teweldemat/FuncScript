using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestInFunction
    {
        [Test]
        public void TestInFunction_WithValidInput_ReturnsTrue()
        {
            var exp = "1 in [1, 2, 3]";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestInFunction_WithValueNotInList_ReturnsFalse()
        {
            var exp = "4 in [1, 2, 3]";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestInFunction_WithNullList_ReturnsNull()
        {
            var exp = "1 in null";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestInFunction_WithInvalidListParameter_ReturnsError()
        {
            var exp = "1 in 'not_a_list'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestInFunction_WithInvalidParameterCount_ReturnsError()
        {
            var exp = "1 in [1, 2, 3, 4]";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestInFunction_WithNonListParameter_ReturnsError()
        {
            var exp = "1 in 2";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
