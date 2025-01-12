using FsStudio.Server.FileSystem.Exec;
using Microsoft.Extensions.Configuration;

namespace FsStudio.Server.FileSystem.Tests
{
    [TestFixture]
    public class SessionManagerTests
    {
        private string _tempFolder;
        private SessionManager _sessionManager;
        private IConfiguration _configuration;
        private RemoteLogger _remoteLogger;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempFolder);
            _remoteLogger = new TestRemoteLogger();
            _sessionManager = new SessionManager(_remoteLogger);
            _sessionManager.SetRootFolder(_tempFolder);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (Directory.Exists(_tempFolder))
                Directory.Delete(_tempFolder, true);
        }

        [Test]
        public void CreateFile_CreatesNewFsp()
        {
            var fileName = "testFile";
            _sessionManager.CreateFile("", fileName);
            var fullPath = Path.Combine(_tempFolder, fileName + ".fsp");
            Assert.IsTrue(File.Exists(fullPath));
        }

        [Test]
        public void CreateFolder_CreatesNewDirectory()
        {
            var folderName = "testFolder";
            _sessionManager.CreateFolder("", folderName);
            var fullPath = Path.Combine(_tempFolder, folderName);
            Assert.IsTrue(Directory.Exists(fullPath));
        }

        [Test]
        public void ListSubFoldersAndFiles_ReturnsCorrectListings()
        {
            var folderName = "subFolder";
            var fileName = "subFile";
            _sessionManager.CreateFolder("", folderName);
            _sessionManager.CreateFile(folderName, fileName);
            var (directories, files) = _sessionManager.ListSubFoldersAndFiles(folderName);
            Assert.IsEmpty(directories);
            Assert.Contains(fileName, files);
        }

        [Test]
        public void CreateOrGetSession_ReturnsNewSession()
        {
            _sessionManager.CreateFile("", "mySession");  // Ensure repeatSession.fsp is actually there
            var session = _sessionManager.CreateOrGetSession("mySession",true);
            Assert.IsNotNull(session);
        }

        [Test]
        public void CreateOrGetSession_ExistingSession_ReturnsSame()
        {
            _sessionManager.CreateFile("", "repeatSession");  // Ensure repeatSession.fsp is actually there
            var s1 = _sessionManager.CreateOrGetSession("repeatSession",true);
            var s2 = _sessionManager.CreateOrGetSession("repeatSession",true);
            Assert.AreEqual(s1.SessionId, s2.SessionId);
        }

        [Test]
        public void UnloadSession_RemovesSession()
        {
            _sessionManager.CreateFile("","toUnload");
            var session = _sessionManager.CreateOrGetSession("toUnload",true);
            Assert.IsTrue(_sessionManager.UnloadSession(session.SessionId));
            Assert.IsFalse(_sessionManager.UnloadSession(session.SessionId));
        }

        [Test]
        public void DeleteItem_RemovesFile()
        {
            var fileName = "deleteMe";
            _sessionManager.CreateFile("", fileName);
            _sessionManager.DeleteItem(fileName);
            var fullPath = Path.Combine(_tempFolder, fileName + ".fsp");
            Assert.IsFalse(File.Exists(fullPath));
        }

        [Test]
        public void DeleteItem_RemovesDirectory()
        {
            var folderName = "deleteMeFolder";
            _sessionManager.CreateFolder("", folderName);
            _sessionManager.DeleteItem(folderName);
            var fullPath = Path.Combine(_tempFolder, folderName);
            Assert.IsFalse(Directory.Exists(fullPath));
        }

        [Test]
        public void DuplicateFile_CreatesNewCopy()
        {
            var original = "duplicateMe";
            var copy = "duplicated";
            _sessionManager.CreateFile("", original);
            _sessionManager.DuplicateFile(original, copy);
            var newFullPath = Path.Combine(_tempFolder, copy + ".fsp");
            Assert.IsTrue(File.Exists(newFullPath));
        }

        [Test]
        public void RenameItem_ChangesFileName()
        {
            var originalName = "renameMe";
            var newName = "renamedFile";
            _sessionManager.CreateFile("", originalName);
            _sessionManager.RenameItem(originalName, newName);
            var oldPath = Path.Combine(_tempFolder, originalName + ".fsp");
            var newPath = Path.Combine(_tempFolder, newName + ".fsp");
            Assert.IsFalse(File.Exists(oldPath));
            Assert.IsTrue(File.Exists(newPath));
        }

        [Test]
        public void RenameItem_ChangesFolderName()
        {
            var folderName = "renameFolder";
            var newFolderName = "renamedFolder";
            _sessionManager.CreateFolder("", folderName);
            _sessionManager.RenameItem(folderName, newFolderName);
            var oldPath = Path.Combine(_tempFolder, folderName);
            var newPath = Path.Combine(_tempFolder, newFolderName);
            Assert.IsFalse(Directory.Exists(oldPath));
            Assert.IsTrue(Directory.Exists(newPath));
        }

        [Test]
        public void GetSession_ReturnsExistingSession()
        {
            _sessionManager.CreateFile("","someSession");
            
            var s = _sessionManager.CreateOrGetSession("someSession",true);
            var fetched = _sessionManager.GetSession(s.SessionId);
            Assert.IsNotNull(fetched);
            Assert.AreEqual(s.SessionId, fetched.SessionId);
        }

        private class TestRemoteLogger : RemoteLogger
        {
            public void WriteLine(string text) {}
            public void Clear() {}
        }
    }
}