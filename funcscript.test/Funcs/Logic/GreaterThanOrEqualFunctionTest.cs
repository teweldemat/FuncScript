using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestGreaterThanOrEqualFunction
    {
        [Test]
        public void TestGreaterThanOrEqual_EqualNumbers()
        {
            var exp = "5 >= 5";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.True);
        }

        [Test]
        public void TestGreaterThanOrEqual_GreaterNumber()
        {
            var exp = "7 >= 5";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.True);
        }

        [Test]
        public void TestGreaterThanOrEqual_LesserNumber()
        {
            var exp = "3 >= 5";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.False);
        }

        [Test]
        public void TestGreaterThanOrEqual_DifferentTypes_Error()
        {
            var exp = "'a' >= 5";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestGreaterThanOrEqual_NullValues_Error()
        {
            var exp = "null >= 5";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestGreaterThanOrEqual_InvalidParameterCount_Error()
        {
            var exp = "5 >= 5 >= 3";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
