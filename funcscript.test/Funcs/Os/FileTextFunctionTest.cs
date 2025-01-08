using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Os
{
    public class TestFileTextFunction
    {
        [Test]
        public void TestFileText_MissingParameter()
        {
            var exp = "file()";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestFileText_NullParameter()
        {
            var exp = "file(null)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestFileText_InvalidTypeParameter()
        {
            var exp = "file(123)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestFileText_FileDoesNotExist()
        {
            var exp = "file('non_existent_file.txt')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_EVALUATION));
        }

        [Test]
        public void TestFileText_FileTooBig()
        {
            var exp = "file('path_to_large_file.txt')"; // Ensure this file exists and is larger than 1MB
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_EVALUATION));
        }

        [Test]
        public void TestFileText_ValidFile()
        {
            var filePath = "path_to_valid_file.txt"; // Ensure this file exists and is valid
            var exp = $"file('{filePath}')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<string>());
            Assert.That((string)res, Is.EqualTo(System.IO.File.ReadAllText(filePath)));
        }
    }
}
