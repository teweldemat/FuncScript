using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestLessThanOrEqualFunction
    {
        [Test]
        public void TestLessThanOrEqual_ValidNumbers()
        {
            var exp = "3<=5";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestLessThanOrEqual_EqualNumbers()
        {
            var exp = "4<=4";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestLessThanOrEqual_InvalidParameterCount()
        {
            var exp = "5<=3<=2";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestLessThanOrEqual_NullParameters()
        {
            var exp = "null<=2";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestLessThanOrEqual_IncompatibleTypes()
        {
            var exp = "'a'<=5";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestLessThanOrEqual_NumericAndStringComparison()
        {
            var exp = "2<='b'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestLessThanOrEqual_FloatsAndIntegers()
        {
            var exp = "2.5<=3";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }
    }
}
