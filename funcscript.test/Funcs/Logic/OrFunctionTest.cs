using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestOrFunction
    {
        [Test]
        public void TestOrTrueTrue()
        {
            var exp = "or(true, true)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestOrTrueFalse()
        {
            var exp = "or(true, false)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestOrFalseFalse()
        {
            var exp = "or(false, false)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestOrMixedTypes()
        {
            var exp = "or(1, true)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestOrWithNull()
        {
            var exp = "or(null, true)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestOrEmptyList()
        {
            var exp = "or()";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }
    }
}
