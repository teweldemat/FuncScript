using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestEvaluateIfNotNullFunction
    {
        [Test]
        public void TestEvaluateIfNotNull_ValidValue()
        {
            var exp = "1 ?! 'default'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("default"));
        }
        
        [Test]
        public void TestEvaluateIfNotNull_NullValue()
        {
            var exp = "null ?! 'fallback'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestEvaluateIfNotNull_ParameterCountMismatch()
        {
            var exp = "1 ?! 'default' ?! 'extra'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
