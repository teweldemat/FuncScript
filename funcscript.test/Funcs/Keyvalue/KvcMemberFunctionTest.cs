using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Keyvalue
{
    public class TestKvcMemberFunction
    {
        [Test]
        public void TestKvcMemberFunction_ValidKey()
        {
            var exp = "{ a: 1, b: 2 }.a"; 
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(1));
        }

        [Test]
        public void TestKvcMemberFunction_InvalidKey()
        {
            var exp = "{ a: 1, b: 2 }.c"; 
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo(null)); // Assuming null is returned for missing keys
        }

        [Test]
        public void TestKvcMemberFunction_NullCollection()
        {
            var exp = "null.a"; 
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.TypeOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestKvcMemberFunction_InvalidParameterCount()
        {
            var exp = "{ a: 1, b: 2 }.a.b"; 
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.TypeOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestKvcMemberFunction_InvalidType()
        {
            var exp = "5.a"; // Accessing key from an integer
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.TypeOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
