using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestNotFunction
    {
        [Test]
        public void TestNotTrue()
        {
            var exp = "!(true)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestNotFalse()
        {
            var exp = "!(false)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestNotNull()
        {
            var exp = "!(null)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestNotParameterCountMismatch()
        {
            var exp = "!(true, false)";
            Assert.Throws<SyntaxError>(()=>
            {
                FuncScript.Evaluate(exp);
            });
        }

        [Test]
        public void TestNotNonBoolean()
        {
            var exp = "!(1)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
