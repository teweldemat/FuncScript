// SessionManager.cs
using System.Collections.Concurrent;
using funcscript.funcs.misc;
using funcscript.host;
using Microsoft.Extensions.Configuration;

namespace fsstudio.server.fileSystem.exec
{
    public class SessionManager
    {
        public static String GetAbsolutePath(string rootPath, string relativePath)
        {
            if (relativePath.StartsWith("/"))
                return GetAbsolutePath(rootPath, relativePath.Substring(1));
            if(relativePath.EndsWith("/"))
                return GetAbsolutePath(rootPath, relativePath.Substring(0,relativePath.Length-1));
            return Path.Combine(rootPath, relativePath);
        }

        class RemoteLoggerForFs(RemoteLogger rl) : FsLogger
        {
            public override void WriteLine(string text)
            {
                rl.WriteLine(text);
            }

            public override void Clear()
            {
                Console.WriteLine("Clearing console");
                rl.Clear();
            }
        }

        private readonly ConcurrentDictionary<string, ExecutionSession> _sessionsByFile = new();
        private readonly string _rootPath;
        private readonly RemoteLogger _remoteLogger;

        public string RootPath => _rootPath;

        public SessionManager(IConfiguration configuration, RemoteLogger remoteLogger)
        {
            _remoteLogger = remoteLogger;
            _rootPath = configuration.GetValue<string>("RootFolder")!;
            FsLogger.SetDefaultLogger(new RemoteLoggerForFs(remoteLogger));
        }

        public ExecutionSession CreateOrGetSession(string fromFile)
        {
            var absolutePath =GetAbsolutePath(_rootPath, fromFile + ".fsp");
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
            string fullPath = GetAbsolutePath(_rootPath, relativePath);
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
            string fullPath = GetAbsolutePath(_rootPath, Path.Combine(relativePath, folderName));
            if (Directory.Exists(fullPath))
                throw new IOException($"Folder '{folderName}' already exists.");

            Directory.CreateDirectory(fullPath);
        }

        public void CreateFile(string relativePath, string fileName)
        {
            string fullPath = GetAbsolutePath(_rootPath, Path.Combine(relativePath, fileName + ".fsp"));
            if (File.Exists(fullPath))
                throw new IOException($"File '{fileName}.fsp' already exists.");

            using var stream = File.CreateText(fullPath);
            stream.Write("[]");
        }

        public void DuplicateFile(string relativePath, string newFileName)
        {
            string sourceFullPath = GetAbsolutePath(_rootPath, relativePath) + ".fsp";
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
            string filePath = GetAbsolutePath(_rootPath, relativePath) + ".fsp";
            string dirPath = GetAbsolutePath(_rootPath, relativePath);

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
            string fullPath = GetAbsolutePath(_rootPath, relativePath);
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