using FuncScript.Model;
using FuncScript.Error;
using NUnit.Framework;
using System.IO;

namespace FuncScript.Test.Funcs.Os
{
    public class TestDirectoryListFunction
    {
        [Test]
        public void TestDirListValidDirectory()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "testDir");
            Directory.CreateDirectory(tempDir);
            var exp = $"dirlist('{tempDir}')";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsList,$"Unexpected value {Helpers.FormatToJson(res)}");
            Directory.Delete(tempDir, true);
        }

        [Test]
        public void TestDirListNonExistentDirectory()
        {
            var exp = "dirlist(['nonexistentDirectory'])";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestDirListInvalidParameterCount()
        {
            var exp = "dirlist(['path1', 'path2'])";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestDirListInvalidParameterType()
        {
            var exp = "dirlist([123])";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
            var error = (FsError)res;
            Assert.That(error.ErrorType, Is.EqualTo(FsError.ERROR_TYPE_INVALID_PARAMETER));
        }

        [Test]
        public void TestDirListHandleException()
        {
            var invalidPath = Path.Combine(Path.GetTempPath(), "testDir", "invalidFile");
            var exp = $"dirlist(['{invalidPath}'])";
            var res = Helpers.Evaluate(exp);
            Assert.That(res is FsError);
        }
    }
}
