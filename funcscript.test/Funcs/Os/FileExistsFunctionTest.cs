using System.IO;
using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;

namespace FuncScript.Test.Funcs.Os
{
    public class TestFileExistsFunction
    {
        [Test]
        public void TestFileExists_ValidFilePath_ReturnsTrue()
        {
            // Arrange: Create a temporary file
            string tempFilePath = Path.Combine(Path.GetTempPath(), "temp_test_file.txt");
            File.WriteAllText(tempFilePath, "Temporary test file content");

            try
            {
                // Act: Use the temporary file in your expression
                var exp = $"fileexists('{tempFilePath}')";
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
        public void TestFileExists_InvalidFilePath_ReturnsFalse()
        {
            var exp = "fileexists('C:/path/to/nonexistent/file.txt')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res, Is.False);
        }

        [Test]
        public void TestFileExists_InvalidParameterCount_ReturnsError()
        {
            var exp = "fileexists('C:/path/to/file.txt', 'extra')";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestFileExists_NullParameter_ReturnsError()
        {
            var exp = "fileexists(null)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestFileExists_InvalidTypeParameter_ReturnsError()
        {
            var exp = "fileexists(123)";
            var res = FuncScript.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }
    }
}
