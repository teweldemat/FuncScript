using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Os
{
    public class TestMkdirFunction
    {
        [Test]
        public void TestMkdirValidPath()
        {
            var exp = "mkdir('testDir')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("testDir"));
        }

        [Test]
        public void TestMkdirInvalidPath()
        {
            var exp = "mkdir(123)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That((FsError)res, Has.Property("ErrorType").EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestMkdirNullPath()
        {
            var exp = "mkdir(null)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That((FsError)res, Has.Property("ErrorType").EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestMkdirExcessParameters()
        {
            var exp = "mkdir('testDir', 'extra')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That((FsError)res, Has.Property("ErrorType").EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestMkdirZeroParameters()
        {
            var exp = "mkdir()";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That((FsError)res, Has.Property("ErrorType").EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }
    }
}
