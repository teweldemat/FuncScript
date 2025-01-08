using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Math
{
    public class TestDivisionFunction
    {
        [Test]
        public void TestDivisionValidIntegers()
        {
            var exp = "6 / 3 / 2";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(1));
        }

        [Test]
        public void TestDivisionValidLongs()
        {
            var exp = "18L / 2L / 3L";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(3L));
        }

        [Test]
        public void TestDivisionValidDoubles()
        {
            var exp = "10.0 / 2.0 / 5.0";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(1.0));
        }

        [Test]
        public void TestDivisionDivisionByZero()
        {
            var exp = "5 / 0";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestDivisionInvalidParameterType()
        {
            var exp = "5 / 'a'";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError,$"Unexpected value {Helpers.FormatToJson(res)}");
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestDivisionNoParameters()
        {
            var exp = "6 / ";
            Assert.Throws<SyntaxError>(() => { Helpers.Evaluate(exp); });
        }
    }
}
