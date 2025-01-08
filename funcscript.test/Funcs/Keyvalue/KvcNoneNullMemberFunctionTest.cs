using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Keyvalue
{
    public class TestKvcNoneNullMemberFunction
    {
        [Test]
        public void TestKvcNoneNullMemberFunction_ValidKey()
        {
            // Testing valid key access in a key-value collection
            var exp = "{'name': 'Alice', 'age': 30}?.'name'";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Alice"));
        }

        [Test]
        public void TestKvcNoneNullMemberFunction_InvalidKey()
        {
            // Testing access to a key that doesn't exist
            var exp = "{'name': 'Alice', 'age': 30}?.'gender'";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        [Test]
        public void TestKvcNoneNullMemberFunction_NonKeyValueCollection()
        {
            // Testing access on a non-key-value collection
            var exp = "(1)?.'name'";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestKvcNoneNullMemberFunction_NullTarget()
        {
            // Testing access with a null target
            var exp = "null?.'name'";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.Null);
        }

        

        [Test]
        public void TestKvcNoneNullMemberFunction_InvalidKeyType()
        {
            // Testing access with a key that is not a string
            var exp = "{'name': 'Alice', 'age': 30}?.(1)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
