using System;
using System.IO;
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
            // Arrange: Use a non-existent subfolder in the temporary directory
            string tempDirectory = Path.GetTempPath();
            string invalidPath = Path.Combine(tempDirectory, Guid.NewGuid().ToString(), "test.txt");

            // Act: Attempt to save a file to the invalid path
            var exp = $"SaveFile('{invalidPath}', 'Hello')";
            var res = FuncScript.Evaluate(exp);

            // Assert: Validate that the result is an FsError
            Assert.That(res, Is.InstanceOf<FsError>());
        }
    }
}
