using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestEqualsFunction
    {
        [Test]
        public void TestEqualsWithEqualNumbers()
        {
            var exp = "3=3";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.True);
        }

        [Test]
        public void TestEqualsWithDifferentNumbers()
        {
            var exp = "3=4";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.False);
        }

        [Test]
        public void TestEqualsWithEqualStrings()
        {
            var exp = "'hello'='hello'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.True);
        }

        [Test]
        public void TestEqualsWithDifferentStrings()
        {
            var exp = "'hello'='world'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.False);
        }

        [Test]
        public void TestEqualsWithNulls()
        {
            var exp = "null=null";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.True);
        }

        [Test]
        public void TestEqualsWithOneNull()
        {
            var exp = "null='value'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.False);
        }

        [Test]
        public void TestEqualsWithParameterCountMismatch()
        {
            var exp = "1=2=3";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestEqualsWithDifferentTypes()
        {
            var exp = "1='1'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.False);
        }
    }
}
