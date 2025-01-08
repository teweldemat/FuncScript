using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Math
{
    public class TestSubstractFunction
    {
        [Test]
        public void TestSubstractTwoIntegers()
        {
            var exp = "5 - 2";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(3));
        }

        [Test]
        public void TestSubstractMultipleIntegers()
        {
            var exp = "10 - 1 - 2 - 3";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(4));
        }

        [Test]
        public void TestSubstractWithZero()
        {
            var exp = "5 - 0";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(5));
        }

        [Test]
        public void TestSubstractNegativeResult()
        {
            var exp = "5 - 10";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(-5));
        }

        [Test]
        public void TestSubstractNoParameters()
        {
            var exp = "-()";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSubstractInvalidParameterType()
        {
            var exp = "5 - 'a'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSubstractMixedTypes()
        {
            var exp = "10.5 - 2 - 3";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(5.5));
        }

        [Test]
        public void TestSubstractWithLongs()
        {
            var exp = "10000000000 - 5000000000";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(5000000000));
        }

        [Test]
        public void TestSubstractTooManyParameters()
        {
            var exp = "10 - 2 - 3 - 4 - 5 - 6 - 7";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(-17));
        }
    }
}
