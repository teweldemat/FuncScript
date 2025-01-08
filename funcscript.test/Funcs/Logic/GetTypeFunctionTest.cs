using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Logic
{
    public class TestGetTypeFunction
    {
        [Test]
        public void TestGetTypeNumber()
        {
            var exp = "type(123)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Integer")); // Changed from "number" to "Integer"
        }

        [Test]
        public void TestGetTypeString()
        {
            var exp = "type('hello')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("String"));
        }

        [Test]
        public void TestGetTypeBooleanTrue()
        {
            var exp = "type(true)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Boolean"));
        }

        [Test]
        public void TestGetTypeBooleanFalse()
        {
            var exp = "type(false)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Boolean"));
        }

        [Test]
        public void TestGetTypeNull()
        {
            var exp = "type(null)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Null"));
        }

        [Test]
        public void TestGetTypeArray()
        {
            var exp = "type([1, 2, 3])";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("List")); // Changed from "array" to "List"
        }

        [Test]
        public void TestGetTypeInvalidParameterCount()
        {
            var exp = "type(1, 2)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestGetTypeNoParameters()
        {
            var exp = "type()";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
