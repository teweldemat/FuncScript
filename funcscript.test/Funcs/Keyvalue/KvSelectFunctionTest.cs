using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Keyvalue
{
    public class KvSelectFunctionTest
    {
        [Test]
        public void TestKvSelectValidInput()
        {
            var exp = "Select({a:1, b:2}, {a:null, b:null})";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is SimpleKeyValueCollection);
            var collection = (SimpleKeyValueCollection)res;
            Assert.That(collection.Get("a"), Is.EqualTo(1));
            Assert.That(collection.Get("b"), Is.EqualTo(2));
        }

        [Test]
        public void TestKvSelectInvalidParameterCount()
        {
            var exp = "Select({a:1}, {a:null}, {extra:3})";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestKvSelectFirstParameterInvalidType()
        {
            var exp = "Select(123, {a:null})";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestKvSelectSecondParameterInvalidType()
        {
            var exp = "Select({a:1}, 456)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
