using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Os
{
    public class TestSaveFileFunction
    {
        [Test]
        public void TestSaveFileSuccess()
        {
            var exp = "SaveFile('test.txt', 'Hello, world!')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.EqualTo("Hello, world!"));
        }

        [Test]
        public void TestSaveFileInvalidParameterCount()
        {
            var exp = "SaveFile('test.txt')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestSaveFileNullParameters()
        {
            var exp = "SaveFile(null, null)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestSaveFileTypeMismatch()
        {
            var exp = "SaveFile(123, true)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
        
        [Test]
        public void TestSaveFilePathError()
        {
            var exp = "SaveFile('invalid_path/test.txt', 'Hello')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            // Note: Since we can't predict the internal exception, we won't check the message.
        }
    }
}
