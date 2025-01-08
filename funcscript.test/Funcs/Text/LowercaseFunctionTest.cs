using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Text
{
    public class TestLowercaseFunction
    {
        [Test]
        public void TestLowercaseSimple()
        {
            var exp = "lowercase('HELLO')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("hello"));
        }

        [Test]
        public void TestLowercaseMixedCase()
        {
            var exp = "lowercase('HeLLo WoRLd')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("hello world"));
        }

        [Test]
        public void TestLowercaseEmptyString()
        {
            var exp = "lowercase('')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(""));
        }

        [Test]
        public void TestLowercaseNonStringParameter()
        {
            var exp = "lowercase(123)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestLowercaseNoParameters()
        {
            var exp = "lowercase()";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
