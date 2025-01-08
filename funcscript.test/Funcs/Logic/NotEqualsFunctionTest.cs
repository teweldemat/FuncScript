using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestNotEqualsFunction
    {
        [Test]
        public void TestNotEqualsDifferentNumbers()
        {
            var exp = "1!=2";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestNotEqualsSameNumbers()
        {
            var exp = "1!=1";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestNotEqualsDifferentStrings()
        {
            var exp = "'hello'!='world'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestNotEqualsSameStrings()
        {
            var exp = "'hello'!='hello'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestNotEqualsNulls()
        {
            var exp = "null!=null";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(false));
        }

        [Test]
        public void TestNotEqualsNullAndValue()
        {
            var exp = "null!='value'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestNotEqualsValueAndNull()
        {
            var exp = "'value'!=null";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestNotEqualsDifferentTypes()
        {
            var exp = "1!='1'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(true));
        }

        [Test]
        public void TestNotEqualsParameterCountMismatch()
        {
            var exp = "1!=2!=3";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
