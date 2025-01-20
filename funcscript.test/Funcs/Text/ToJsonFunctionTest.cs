using FuncScript.Model;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Text
{
    public class TestToJsonFunction
    {
        [Test]
        public void TestToJsonWithObject()
        {
            var exp = "tojson({a:1, b:2})";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("{\"a\":1,\"b\":2}"));
        }

        [Test]
        public void TestToJsonWithArray()
        {
            var exp = "Helpers([1, 2, 3])";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("[1,2,3]"));
        }

        [Test]
        public void TestToJsonEmptyObject()
        {
            var exp = "Helpers({})";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("{}"));
        }

        [Test]
        public void TestToJsonEmptyArray()
        {
            var exp = "Helpers([])";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo("[]"));
        }

        [Test]
        public void TestToJsonWithInvalidParameterCount()
        {
            var exp = "tojson()";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
