using FuncScript.Core;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Math
{
    public class TestSineFunction
    {
        [Test]
        public void TestSineWithInteger()
        {
            var exp = "Sin(0)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(0.0));
        }

        [Test]
        public void TestSineWithDouble()
        {
            var exp = "Sin(1.5708)"; // Approximately π/2
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(1.0).Within(0.0001));
        }

        [Test]
        public void TestSineWithLong()
        {
            var exp = "Sin(10L)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(-0.544021110329).Within(0.0001));
        }

        [Test]
        public void TestSineWithIncorrectParameterCount()
        {
            var exp = "Sin(1, 2)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestSineWithInvalidParameter()
        {
            var exp = "Sin('text')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }

    public class TestCosineFunction
    {
        [Test]
        public void TestCosineWithInteger()
        {
            var exp = "Cos(0)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(1.0));
        }

        [Test]
        public void TestCosineWithDouble()
        {
            var exp = "Cos(3.14159)"; // Approximately π
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(-1.0).Within(0.001));
        }

        [Test]
        public void TestCosineWithLong()
        {
            var exp = "Cos(10L)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.EqualTo(-0.839071529076).Within(0.001));
        }

        [Test]
        public void TestCosineWithIncorrectParameterCount()
        {
            var exp = "Cos(1, 2)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestCosineWithInvalidParameter()
        {
            var exp = "Cos('text')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
