using System.IO;
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
            // Arrange: Create a temporary file
            string tempFilePath = Path.Combine(Path.GetTempPath(), "temp_test_file.txt");
            File.WriteAllText(tempFilePath, "Temporary file content.");

            try
            {
                // Act: Use the temporary file in your expression
                var exp = $"isfile('{tempFilePath}')";
                var res = FuncScript.Evaluate(exp);

                // Assert: Validate the result
                Assert.That(res, Is.True);
            }
            finally
            {
                // Cleanup: Delete the temporary file
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
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
