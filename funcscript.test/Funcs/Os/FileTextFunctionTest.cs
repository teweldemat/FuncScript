using System.IO;
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
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestFileText_NullParameter()
        {
            var exp = "file(null)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestFileText_InvalidTypeParameter()
        {
            var exp = "file(123)";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestFileText_FileDoesNotExist()
        {
            var exp = "file('non_existent_file.txt')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_EVALUATION));
        }

        [Test]
        public void TestFileText_FileTooBig()
        {
            var exp = "file('path_to_large_file.txt')"; // Ensure this file exists and is larger than 1MB
            var res = Helpers.Evaluate(exp);
            Assert.That(res, Is.InstanceOf<FsError>());
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_EVALUATION));
        }

        [Test]
        public void TestFileText_ValidFile()
        {
            // Arrange: Create a temporary file with predefined content
            string tempFilePath = Path.Combine(Path.GetTempPath(), "temp_valid_file.txt");
            string fileContent = "This is a test file content.";
            File.WriteAllText(tempFilePath, fileContent);

            try
            {
                // Act: Use the temporary file in your expression
                var exp = $"file('{tempFilePath}')";
                var res = Helpers.Evaluate(exp);

                // Assert: Validate the result
                Assert.That(res, Is.InstanceOf<string>());
                Assert.That((string)res, Is.EqualTo(File.ReadAllText(tempFilePath)));
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
    }
}
