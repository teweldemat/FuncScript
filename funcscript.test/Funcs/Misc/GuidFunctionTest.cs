using System;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Misc
{
    public class TestGuidFunction
    {
        [Test]
        public void TestValidGuid()
        {
            var exp = "guid('dcbce14a-99a7-4191-9f78-ae24469a73c4')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<Guid>());
        }

        [Test]
        public void TestInvalidGuidFormat()
        {
            var exp = "guid('invalid-guid-format')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestNullParameter()
        {
            var exp = "guid(null)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestNonStringParameter()
        {
            var exp = "guid(123)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestParameterCountMismatch()
        {
            var exp = "guid('dcbce14a-99a7-4191-9f78-ae24469a73c4', 'extra')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
