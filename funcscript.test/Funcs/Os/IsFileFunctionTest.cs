using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Os
{
    public class TestIsFileFunction
    {
        [Test]
        public void TestIsFileValidPath()
        {
            var exp = "isfile('C:\\path\\to\\your\\file.txt')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.True);
        }

        [Test]
        public void TestIsFileInvalidPath()
        {
            var exp = "isfile('C:\\path\\to\\nonexistent\\file.txt')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.False);
        }

        [Test]
        public void TestIsFileDirectoryPath()
        {
            var exp = "isfile('C:\\path\\to\\your\\directory')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.False);
        }

        [Test]
        public void TestIsFileNullParameter()
        {
            var exp = "isfile(null)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestIsFileInvalidParameterCount()
        {
            var exp = "isfile('file.txt', 'extra')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestIsFileInvalidParameterType()
        {
            var exp = "isfile(123)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
