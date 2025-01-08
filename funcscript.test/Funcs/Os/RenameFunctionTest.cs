using FuncScript.Core;
using FuncScript.Error;
using FuncScript.Model;
using NUnit.Framework;
using System.IO;

namespace FuncScript.Test.Funcs.Os
{
    public class TestRenameFunction
    {
        [Test]
        public void TestRenameFile_ValidParameters_ReturnsNewPath()
        {
            // Arrange: Create a temporary file
            string tempDirectory = Path.GetTempPath();
            string oldFilePath = Path.Combine(tempDirectory, "test.txt");
            string newFileName = "renamed_test.txt";
            string newFilePath = Path.Combine(tempDirectory, newFileName);

            File.WriteAllText(oldFilePath, "Temporary file content.");

            try
            {
                // Act: Rename the file using the expression
                var exp = $"RenameFile('{oldFilePath}', '{newFilePath}')";
                var res = Helpers.Evaluate(exp);

                // Assert: Validate the result
                Assert.That(res, Is.EqualTo(newFilePath));
                Assert.That(File.Exists(newFilePath), Is.True);
            }
            finally
            {
                // Cleanup: Delete both the old and new files if they exist
                if (File.Exists(oldFilePath)) File.Delete(oldFilePath);
                if (File.Exists(newFilePath)) File.Delete(newFilePath);
            }
        }

        [Test]
        public void TestRenameFile_InvalidParameterCount_ReturnsError()
        {
            // Arrange
            var exp = "RenameFile('oldPath')";

            // Act
            var res = Helpers.Evaluate(exp);

            // Assert
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_PARAMETER_COUNT_MISMATCH));
        }

        [Test]
        public void TestRenameFile_NullParameters_ReturnsError()
        {
            // Arrange
            var exp = "RenameFile(null, 'newName')";

            // Act
            var res = Helpers.Evaluate(exp);

            // Assert
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestRenameFile_TypeMismatch_ReturnsError()
        {
            // Arrange
            var exp = "RenameFile(123, 'newName')";

            // Act
            var res = Helpers.Evaluate(exp);

            // Assert
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestRenameFile_FileDoesNotExist_ReturnsError()
        {
            // Arrange
            var exp = "RenameFile('non_existent_file.txt', 'newName')";

            // Act
            var res = Helpers.Evaluate(exp);

            // Assert
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_EVALUATION));
        }
    }
}
