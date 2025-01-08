using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Math
{
    public class TestModuloFunction
    {
        [Test]
        public void TestModuloInt()
        {
            var exp = "5 % 3";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(2));
        }

        [Test]
        public void TestModuloLong()
        {
            var exp = "10000000000 % 3";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(1));
        }

        [Test]
        public void TestModuloDouble()
        {
            var exp = "5.5 % 1.5";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(1.0));
        }

        [Test]
        public void TestModuloWithZeroDivisor()
        {
            var exp = "5 % 0";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestModuloWithInvalidType()
        {
            var exp = "5 % 'string'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestModuloEmptyParameters()
        {
            var exp = "%";
            Assert.Throws<SyntaxError>(() =>
            {
                FuncScript.Evaluate(exp);
            });
        }

        [Test]
        public void TestModuloMixedTypes()
        {
            var exp = "5 % 2.5";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(0));
        }
    }
}
