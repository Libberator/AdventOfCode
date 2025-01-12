//#define PRINT_DIRECTORY

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AoC.Solutions.Y2022.D07;

public class Solution : ISolver
{
    private const string ListFiles = "$ ls";
    private const string UpOneFolder = "..";
    private const string CdPattern = @"\$ cd (.+)";
    private const string FilePattern = @"(\d+) (.+)";
    private const string FolderPattern = "dir (.+)";
    private const int MaxSystemSize = 40_000_000;
    private const int MaxDirectorySize = 100_000;

    private readonly List<Folder> _allFolders = [];
    private readonly Folder _root = new("/");

    public void Setup(string[] input)
    {
        var cwd = _root; // current working directory

        foreach (var line in input.Skip(1))
        {
            if (line == ListFiles) continue;

            var cdMatch = Regex.Match(line, CdPattern);
            if (cdMatch.Success)
            {
                var nextFolder = cdMatch.Groups[1].Value;
                cwd = nextFolder == UpOneFolder
                    ? cwd.Parent!
                    : cwd.SubFolders.First(f => f.Name == nextFolder);
                continue;
            }

            var fileMatch = Regex.Match(line, FilePattern);
            if (fileMatch.Success)
            {
                var size = int.Parse(fileMatch.Groups[1].ValueSpan);
                cwd.AddFile(fileMatch.Groups[2].Value, size);
                continue;
            }

            var folderMatch = Regex.Match(line, FolderPattern);
            if (!folderMatch.Success) continue;

            var folderName = folderMatch.Groups[1].Value;
            var newFolder = new Folder(folderName, cwd);
            cwd.AddSubFolder(newFolder);
            _allFolders.Add(newFolder);
        }
#if PRINT_DIRECTORY
        PrettyPrint();
#endif
    }

    public object SolvePart1() => _allFolders.Sum(f => f.Size > MaxDirectorySize ? 0 : f.Size);

    public object SolvePart2()
    {
        var amountOver = _root.Size - MaxSystemSize;
        return _allFolders.Where(f => f.Size >= amountOver).Min(f => f.Size);
    }

#if PRINT_DIRECTORY
    public void PrettyPrint()
    {
        var depth = 0;
        var sb = new System.Text.StringBuilder();
        Recurse(_root);

        System.Console.WriteLine(sb.ToString());
        return;

        void Recurse(Folder dir)
        {
            sb.AppendLine($"{new string(' ', depth)}- {dir.Name} (dir)");
            depth++;
            foreach (var subFolder in dir.SubFolders)
                Recurse(subFolder);
            foreach (var file in dir.Files)
                sb.AppendLine($"{new string(' ', depth)}- {file.Name} (file, size={file.Size})");
            depth--;
        }
    }
#endif

    private class Folder(string name, Folder? parent = null)
    {
        public readonly HashSet<File> Files = [];
        public readonly string Name = name;
        public readonly Folder? Parent = parent;
        public readonly HashSet<Folder> SubFolders = [];
        private int _folderSize;
        private bool _isDirty = true;
        private int _totalFileSize;

        public int Size => GetSize();

        public void AddSubFolder(Folder newFolder) => SubFolders.Add(newFolder);

        public void AddFile(string fileName, int fileSize)
        {
            Files.Add(new File(fileName, fileSize));
            _totalFileSize += fileSize;
        }

        private int GetSize()
        {
            if (!_isDirty) return _folderSize;

            _folderSize = _totalFileSize + SubFolders.Sum(f => f.Size);
            _isDirty = false;
            return _folderSize;
        }

        public readonly record struct File(string Name, int Size);
    }
}