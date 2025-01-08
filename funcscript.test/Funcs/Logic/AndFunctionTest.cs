using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestAndFunction
    {
        [Test]
        public void TestAndTrue()
        {
            var exp = "and(true, true, true)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestAndFalse()
        {
            var exp = "and(true, false, true)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestAndMixedBooleans()
        {
            var exp = "and(true, true, false, true)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestAndInvalidParameter()
        {
            var exp = "and(true, 'notABool', false)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestAndNullParameter()
        {
            var exp = "and(true, null, false)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
