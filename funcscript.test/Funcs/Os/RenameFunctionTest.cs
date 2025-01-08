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
            // Arrange
            var oldFilePath = "test.txt";
            var newFileName = "renamed_test.txt";
            File.Create(oldFilePath).Dispose();

            var exp = $"RenameFile('{oldFilePath}', '{newFileName}')";
            // Act
            var res = FuncScript.Evaluate(exp);

            // Assert
            Assert.That(res, Is.EqualTo(Path.Combine(Directory.GetCurrentDirectory(), newFileName)));

            // Cleanup
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), newFileName));
        }

        [Test]
        public void TestRenameFile_InvalidParameterCount_ReturnsError()
        {
            // Arrange
            var exp = "RenameFile('oldPath')";

            // Act
            var res = FuncScript.Evaluate(exp);

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
            var res = FuncScript.Evaluate(exp);

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
            var res = FuncScript.Evaluate(exp);

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
            var res = FuncScript.Evaluate(exp);

            // Assert
            Assert.That(res is FsError);
            Assert.That(((FsError)res).ErrorType, Is.EqualTo(FsError.ERROR_TYPE_EVALUATION));
        }
    }
}
