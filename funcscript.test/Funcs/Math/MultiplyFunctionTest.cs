using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Math
{
    public class TestMultiplyFunction
    {
        [Test]
        public void TestMultiplyIntegers()
        {
            var exp = "2 * 3 * 4"; // Changed to infix notation
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(24));
        }

        [Test]
        public void TestMultiplyLongs()
        {
            var exp = "10000000000 * 2 * 3"; // Changed to infix notation
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(60000000000));
        }

        [Test]
        public void TestMultiplyDoubles()
        {
            var exp = "1.5 * 2.0 * 3.0"; // Changed to infix notation
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(9.0));
        }

        [Test]
        public void TestEmptyParameters()
        {
            var exp = "[]*2"; // Ensure to verify how empty parameters are handled with the original function if required
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestInvalidParameterType()
        {
            var exp = "1 * 'a' * 3"; // Changed to infix notation
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestMixedParameters()
        {
            var exp = "2 * 3.5 * 4"; // Changed to infix notation
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(28.0));
        }
    }
}
