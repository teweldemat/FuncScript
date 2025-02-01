using FuncScript;
using FuncScript.Core;
using FuncScript.Model;

namespace FsStudio.Server.FileSystem.Exec.Funcs;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class MemoryCommandExecutor:IFsFunction
{
    public static object PerformMemoryCommand(string rootPath, string commandHeader, string payload)
    {
        try
        {
            if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

            using var doc = JsonDocument.Parse(commandHeader);
            var root = doc.RootElement;
            var command = root.GetProperty("command").GetString();
            var pathProp = root.GetProperty("path");
            var path = new List<string>();
            foreach (var segment in pathProp.EnumerateArray())
                path.Add(segment.GetString());

            switch (command)
            {
                case "get-value":
                    return GetValue(rootPath, path);
                case "count-children":
                    return CountChildren(rootPath, path);
                case "list-children":
                    return ListChildren(rootPath, path);
                case "search-keys":
                    return SearchKeys(rootPath, path, root.GetProperty("query").GetString(), 
                                      root.GetProperty("query_type").GetString());
                case "set-value":
                    return SetValue(rootPath, path, payload);
                case "remove-key":
                    return RemoveKey(rootPath, path);
                case "duplicate-key":
                {
                    var destParentProp = root.GetProperty("destination_parent");
                    var destPath = new List<string>();
                    foreach (var seg in destParentProp.EnumerateArray())
                        destPath.Add(seg.GetString());
                    return DuplicateKey(rootPath, path, destPath);
                }
                case "move-key":
                {
                    var destParentProp = root.GetProperty("destination_parent");
                    var destPath = new List<string>();
                    foreach (var seg in destParentProp.EnumerateArray())
                        destPath.Add(seg.GetString());
                    return MoveKey(rootPath, path, destPath);
                }
                default:
                    return new FsError($"Error: Unknown command '{command}'");
            }
        }
        catch (Exception ex)
        {
            return new FsError($"Error: {ex.Message}", FsError.ERROR_TYPE_INVALID_PARAMETER);
        }
    }

    static object GetValue(string rootPath, List<string> path)
    {
        var file = BuildFilePath(rootPath, path);
        if (!File.Exists(file)) return new FsError($"Error: Path {JsonSerializer.Serialize(path)} does not exist",FsError.ERROR_TYPE_INVALID_PARAMETER);
        return File.ReadAllText(file);
    }

    static object CountChildren(string rootPath, List<string> path)
    {
        var dir = BuildDirectoryPath(rootPath, path);
        if (!Directory.Exists(dir)) return new FsError($"Error: Path {JsonSerializer.Serialize(path)} does not exist",FsError.ERROR_TYPE_INVALID_PARAMETER);
        var dirs = Directory.GetDirectories(dir).Length;
        var files = Directory.GetFiles(dir, "*.txt").Length;
        return dirs + files;
    }

    static object ListChildren(string rootPath, List<string> path)
    {
        var dir = BuildDirectoryPath(rootPath, path);
        if (!Directory.Exists(dir)) return new FsError($"Error: Path {JsonSerializer.Serialize(path)} does not exist",FsError.ERROR_TYPE_INVALID_PARAMETER);
        var dirs = Directory.GetDirectories(dir).Select(Path.GetFileName).ToList();
        var files = Directory.GetFiles(dir, "*.txt").Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
        var merged = dirs.Concat(files).ToList();
        return JsonSerializer.Serialize(merged);
    }

    static object SearchKeys(string rootPath, List<string> path, string query, string queryType)
    {
        var dir = BuildDirectoryPath(rootPath, path);
        if (!Directory.Exists(dir)) return new FsError($"Error: Path {JsonSerializer.Serialize(path)} does not exist",FsError.ERROR_TYPE_INVALID_PARAMETER);
        var dirs = Directory.GetDirectories(dir).Select(Path.GetFileName);
        var files = Directory.GetFiles(dir, "*.txt").Select(x => Path.GetFileNameWithoutExtension(x));
        var all = dirs.Concat(files);

        IEnumerable<string> filtered = all;
        if (queryType == "starts-with") filtered = all.Where(x => x.StartsWith(query));
        else if (queryType == "ends-with") filtered = all.Where(x => x.EndsWith(query));
        else if (queryType == "contains") filtered = all.Where(x => x.Contains(query));
        return JsonSerializer.Serialize(filtered);
    }

    static object SetValue(string rootPath, List<string> path, string value)
    {
        var file = BuildFilePath(rootPath, path);
        var dir = Path.GetDirectoryName(file);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(file, value ?? "");
        return "ok";
    }

    static object RemoveKey(string rootPath, List<string> path)
    {
        var file = BuildFilePath(rootPath, path);
        var dir = BuildDirectoryPath(rootPath, path);
        if (File.Exists(file))
        {
            File.Delete(file);
            return "ok";
        }
        else if (Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
            return "ok";
        }
        return  new FsError($"Error: Path {JsonSerializer.Serialize(path)} does not exist",FsError.ERROR_TYPE_INVALID_PARAMETER);
    }

    static object DuplicateKey(string rootPath, List<string> sourcePath, List<string> destParent)
    {
        var sourceFile = BuildFilePath(rootPath, sourcePath);
        var sourceDir = BuildDirectoryPath(rootPath, sourcePath);
        var destDir = BuildDirectoryPath(rootPath, destParent);

        if (!Directory.Exists(destDir)) return new FsError( $"Error: Destination parent {JsonSerializer.Serialize(destParent)} does not exist",FsError.ERROR_TYPE_INVALID_PARAMETER);

        if (File.Exists(sourceFile))
        {
            var newFile = Path.Combine(destDir, Path.GetFileName(sourceFile));
            File.Copy(sourceFile, newFile, true);
            return "ok";
        }
        else if (Directory.Exists(sourceDir))
        {
            var newSubDir = Path.Combine(destDir, Path.GetFileName(sourceDir));
            CopyDirectory(sourceDir, newSubDir);
            return "ok";
        }
        return new FsError( $"Error: Source path {JsonSerializer.Serialize(sourcePath)} does not exist",FsError.ERROR_TYPE_INVALID_PARAMETER);
    }

    static object MoveKey(string rootPath, List<string> sourcePath, List<string> destParent)
    {
        var result = DuplicateKey(rootPath, sourcePath, destParent);
        if (result is string s && s == "ok")
        {
            RemoveKey(rootPath, sourcePath);
            return "ok";
        }
        return result;
    }

    static void CopyDirectory(string sourceDir, string destDir)
    {
        if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }
        foreach (var folder in Directory.GetDirectories(sourceDir))
        {
            var destFolder = Path.Combine(destDir, Path.GetFileName(folder));
            CopyDirectory(folder, destFolder);
        }
    }

    static string BuildFilePath(string rootPath, List<string> path)
    {
        var fileName = path.Count > 0 ? path[path.Count - 1] + ".txt" : ".txt";
        var dirs = path.Count > 0 ? path.GetRange(0, path.Count - 1) : new List<string>();
        var dirPath = Path.Combine(new[] { rootPath }.Concat(dirs).ToArray());
        return Path.Combine(dirPath, fileName);
    }

    static string BuildDirectoryPath(string rootPath, List<string> path)
    {
        return Path.Combine(new[] { rootPath }.Concat(path).ToArray());
    }

    public object EvaluateList(KeyValueCollection context, FsList pars)
    {
        if (pars.Length < 2 || pars.Length > 3)
            return new FsError(
                FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                $"{Symbol} function: invalid parameter count. 2-3 expected, got {pars.Length}"
            );

        var rootPath = pars[0]?.ToString();
        var commandHeader = pars[1];
        var payload = pars.Length == 3 ? pars[2]?.ToString() : "";

        if (rootPath == null)
            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, "Root path cannot be null");
        if (commandHeader == null)
            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, "Command header cannot be null");

        return PerformMemoryCommand(rootPath, Helpers.FormatToJson(commandHeader), payload ?? "");
    }

    public CallType CallType { get; } = CallType.Infix;
    public string Symbol { get; } = "AgentMem";
}