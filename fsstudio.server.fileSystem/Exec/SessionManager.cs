using System.Collections.Concurrent;
using FuncScript;
using FuncScript.Host;

namespace FsStudio.Server.FileSystem.Exec
{
    public class SessionManager
    {
        private readonly ConcurrentDictionary<string, ExecutionSession> _sessionsByFile = new();
        private string _rootPath = null;
        private readonly RemoteLogger _remoteLogger;
        private readonly string _lastRootFolderFilePath;

        public string RootPath => _rootPath;

        public String GetAbsolutePath(string relativePath)
        {
            if (_rootPath == null)
                throw new InvalidOperationException("Root path not set");
            if (relativePath.StartsWith("/"))
                return GetAbsolutePath(relativePath.Substring(1));
            if (relativePath.EndsWith("/"))
                return GetAbsolutePath(relativePath.Substring(0, relativePath.Length - 1));
            return Path.Combine(_rootPath, relativePath);
        }

        public class RemoteLoggerForFs(RemoteLogger rl, string sessionId) : FsLogger
        {
            public override void WriteLine(string text)
            {
                rl.WriteLine(sessionId, text);
            }

            public override void Clear()
            {
                Console.WriteLine("Clearing console");
                rl.Clear(sessionId);
            }
        }

        public SessionManager(IConfiguration configuration, RemoteLogger remoteLogger)
        {
            _remoteLogger = remoteLogger;
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataFolder, "fs-studio");
            if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);
            _lastRootFolderFilePath = Path.Combine(appFolder, "last-root-folder.txt");
            if (File.Exists(_lastRootFolderFilePath))
            {
                _rootPath = File.ReadAllText(_lastRootFolderFilePath).Trim();
                if (string.IsNullOrWhiteSpace(_rootPath))
                    _rootPath = null;
            }
        }

        public void SetRootFolder(string newRootPath)
        {
            _rootPath = newRootPath;
            _sessionsByFile.Clear();

            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataFolder, "fs-studio");
            if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);
            File.WriteAllText(_lastRootFolderFilePath, newRootPath);
        }

        public ExecutionSession CreateOrGetSession(string fromFile)
        {
            var absolutePath = GetAbsolutePath(fromFile + ".fsp");
            if (_sessionsByFile.TryGetValue(absolutePath, out var existingSession))
            {
                return existingSession;
            }

            var newSession = new ExecutionSession(absolutePath, _remoteLogger);
            _sessionsByFile[absolutePath] = newSession;
            return newSession;
        }

        public bool UnloadSession(Guid sessionId)
        {
            foreach (var kvp in _sessionsByFile)
            {
                if (kvp.Value.SessionId == sessionId)
                {
                    return _sessionsByFile.TryRemove(kvp.Key, out _);
                }
            }
            return false;
        }

        public ExecutionSession? GetSession(Guid sessionId)
        {
            foreach (var session in _sessionsByFile.Values)
            {
                if (session.SessionId == sessionId)
                {
                    return session;
                }
            }
            return null;
        }

        public (string[] Directories, string[] Files) ListSubFoldersAndFiles(string relativePath)
        {
            string fullPath = GetAbsolutePath(relativePath);
            if (!Directory.Exists(fullPath))
                throw new DirectoryNotFoundException("Directory not found.");

            var directories = Directory.GetDirectories(fullPath)
                                       .Select(Path.GetFileName)
                                       .ToArray();

            var files = Directory.GetFiles(fullPath, "*.fsp").Select(n =>
            {
                var f = new FileInfo(n);
                return f.Name.Substring(0, f.Name.Length - f.Extension.Length);
            }).ToArray();

            return (directories, files);
        }

        public void CreateFolder(string relativePath, string folderName)
        {
            string fullPath = GetAbsolutePath(Path.Combine(relativePath, folderName));
            if (Directory.Exists(fullPath))
                throw new IOException($"Folder '{folderName}' already exists.");

            Directory.CreateDirectory(fullPath);
        }

        public void CreateFile(string relativePath, string fileName)
        {
            string fullPath = GetAbsolutePath(Path.Combine(relativePath, fileName + ".fsp"));
            if (File.Exists(fullPath))
                throw new IOException($"File '{fileName}.fsp' already exists.");

            using var stream = File.CreateText(fullPath);
            stream.Write("[]");
        }

        public void DuplicateFile(string relativePath, string newFileName)
        {
            string sourceFullPath = GetAbsolutePath(relativePath) + ".fsp";
            if (!File.Exists(sourceFullPath))
                throw new FileNotFoundException($"Source file '{relativePath}.fsp' not found.");

            string directory = Path.GetDirectoryName(sourceFullPath)!;
            string targetFullPath = Path.Combine(directory, newFileName + ".fsp");
            if (File.Exists(targetFullPath))
                throw new IOException($"File '{newFileName}.fsp' already exists in the target folder.");

            File.Copy(sourceFullPath, targetFullPath);
        }

        public void DeleteItem(string relativePath)
        {
            string filePath = GetAbsolutePath(relativePath) + ".fsp";
            string dirPath = GetAbsolutePath(relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _sessionsByFile.TryRemove(filePath, out _);
            }
            else if (Directory.Exists(dirPath))
            {
                Directory.Delete(dirPath, true);

                var keysToRemove = _sessionsByFile.Keys
                    .Where(k => Path.GetDirectoryName(k)?.TrimEnd(Path.DirectorySeparatorChar) ==
                                dirPath.TrimEnd(Path.DirectorySeparatorChar))
                    .ToList();
                foreach (var key in keysToRemove)
                    _sessionsByFile.TryRemove(key, out _);
            }
            else
            {
                throw new FileNotFoundException($"Path '{relativePath}' not found.");
            }
        }

        public void RenameItem(string relativePath, string newName)
        {
            string fullPath = GetAbsolutePath(relativePath);
            if (Directory.Exists(fullPath))
            {
                string directory = Directory.GetParent(fullPath)!.FullName;
                string newPath = Path.Combine(directory, newName);
                if (Directory.Exists(newPath))
                    throw new IOException($"Path {newPath} already exists.");
                Directory.Move(fullPath, newPath);

                var keysToRename = _sessionsByFile.Keys
                    .Where(k => Path.GetDirectoryName(k)?.TrimEnd(Path.DirectorySeparatorChar) ==
                                fullPath.TrimEnd(Path.DirectorySeparatorChar))
                    .ToList();
                foreach (var oldKey in keysToRename)
                {
                    var fileName = Path.GetFileName(oldKey);
                    var updatedKey = Path.Combine(newPath, fileName);
                    if (_sessionsByFile.TryRemove(oldKey, out var session))
                    {
                        _sessionsByFile[updatedKey] = session;
                    }
                }
            }
            else
            {
                fullPath += ".fsp";
                if (File.Exists(fullPath))
                {
                    string directory = new FileInfo(fullPath).DirectoryName!;
                    string newPath = Path.Combine(directory, newName) + ".fsp";
                    if (File.Exists(newPath))
                        throw new IOException($"Path {newPath} already exists.");
                    File.Move(fullPath, newPath);

                    if (_sessionsByFile.TryRemove(fullPath, out var session))
                    {
                        _sessionsByFile[newPath] = session;
                    }
                }
                else
                {
                    throw new FileNotFoundException($"Path {relativePath} not found.");
                }
            }
        }
    }
}
