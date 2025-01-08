using FuncScript.Core;
using FuncScript.Error;
using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestIfConditionFunction
    {
        [Test]
        public void TestIfConditionTrue()
        {
            var exp = "If(true,'yes','no')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("yes"));
        }

        [Test]
        public void TestIfConditionFalse()
        {
            var exp = "If(false,'yes','no')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("no"));
        }

        [Test]
        public void TestIfConditionInvalidParameterCount()
        {
            var exp = "If(true,'yes')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestIfConditionInvalidType()
        {
            var exp = "If(1,'yes','no')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
