using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Keyvalue
{
    public class TestKvcNoneNullMemberFunction
    {
        private const string FuncSymbol = "?.";

        [Test]
        public void TestKvcNoneNullMemberFunction_ValidKey()
        {
            var exp = "kv({'name': 'Alice', 'age': 30})?.'name'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Alice"));
        }

        [Test]
        public void TestKvcNoneNullMemberFunction_InvalidKey()
        {
            var exp = "kv({'name': 'Alice', 'age': 30})?.'gender'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestKvcNoneNullMemberFunction_NonKeyValueCollection()
        {
            var exp = "(1)?.'name'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestKvcNoneNullMemberFunction_NullTarget()
        {
            var exp = "null?.'name'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestKvcNoneNullMemberFunction_InvalidParameterCount()
        {
            var exp = "kv({'name': 'Alice'})?.'name'?'extraParam'";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestKvcNoneNullMemberFunction_InvalidKeyType()
        {
            var exp = "kv({'name': 'Alice', 'age': 30})?.(1)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
