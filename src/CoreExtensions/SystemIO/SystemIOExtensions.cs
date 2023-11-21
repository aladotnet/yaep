using System.Runtime.InteropServices;

namespace System.IO
{
    public interface ISystemIoProvider
    {
        char AltDirectorySeparatorChar { get; }
        char PathSeparator { get; }
        char DirectorySeparatorChar { get; }

        bool FileExists(string path);
        bool DirectoryExists(string path);
        string GetPathRoot(string path);

        void CreateDirectory(string path);

        void CopyFile(string sourcePath, string destinationPath, bool overwrite = false);

        char[] GetInvalidPathChars();
        char[] GetInvalidFileNameChars();

        string GetDirectoryName(string filePath);
        string[] GetDirectories(string path, string searchPattern, SearchOption searchOption);
        string GetFileName(string filePath);
        bool EndsInDirectorySeparator(string path);
        string GetFullPath(string path);
        string PathCombine(string path1, string path2);
        string PathCombine(string path1, string path2, string path3);
        string PathCombine(string path1, string path2, string path3, string path4);
        string PathCombine(string path1, string path2, string path3, string path4, string path5);
        string PathCombine(string path1, string path2, string path3, string path4, string path5, string path6);
        string PathCombine(string path1, string path2, string path3, string path4, string path5, string path6, string path7);
    }

    internal class DefaultSystemIoProvider : ISystemIoProvider
    {
        public char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;

        public char PathSeparator => Path.PathSeparator;

        public char DirectorySeparatorChar => Path.DirectorySeparatorChar;

        public bool DirectoryExists(string path)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string path)
        {
            throw new NotImplementedException();
        }

        public string GetPathRoot(string path)
        {
            throw new NotImplementedException();
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void CopyFile(string sourcePath, string destinationPath, bool overwrite = false)
        {
            File.Copy(sourcePath, destinationPath, overwrite);
        }
        public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) => Directory.GetDirectories(path, searchPattern, searchOption);
        public char[] GetInvalidPathChars() => System.IO.Path.GetInvalidPathChars();

        public char[] GetInvalidFileNameChars() => System.IO.Path.GetInvalidFileNameChars();

        public string GetDirectoryName(string filePath) => Path.GetDirectoryName(filePath);
        public string GetFileName(string filePath) => Path.GetFileName(filePath);

        public bool EndsInDirectorySeparator(string path) => Path.EndsInDirectorySeparator(path);

        public string GetFullPath(string path) => Path.GetFullPath(path);

        public string PathCombine(string path1, string path2) => Path.Combine(path1: path1, path2: path2);

        public string PathCombine(string path1, string path2, string path3) => Path.Combine(path1: path1, path2: path2, path3: path3);

        public string PathCombine(string path1, string path2, string path3, string path4) => Path.Combine(path1: path1, path2: path2, path3: path3, path4: path4);

        public string PathCombine(string path1, string path2, string path3, string path4, string path5)
        {
            var path = Path.Combine(path1: path1, path2: path2, path3: path3, path4: path4);
            return Path.Combine(path1: path, path2: path5);
        }

        public string PathCombine(string path1, string path2, string path3, string path4, string path5, string path6)
        {
            var path = Path.Combine(path1: path1, path2: path2, path3: path3, path4: path4);
            return Path.Combine(path1: path, path2: path5, path3: path6);
        }

        public string PathCombine(string path1, string path2, string path3, string path4, string path5, string path6, string path7)
        {
            var path = Path.Combine(path1: path1, path2: path2, path3: path3, path4: path4);
            return Path.Combine(path1: path, path2: path5, path3: path6, path4: path7);
        }

        // add all needed io methods here
    }

    //public record PathBase
    //{
    //    private ISystemIoProvider systemIoProvider;

    //    public string Path { get; init; }

    //    public PathBase(string path)
    //    {
    //        Path = path;
    //    }

    //    // this will help me out testing the extension methods 
    //    internal ISystemIoProvider SystemIoProvider
    //    {
    //        get => systemIoProvider ?? new DefaultSystemIoProvider();
    //        set => systemIoProvider = value;
    //    }
    //    public static implicit operator string(PathBase status) => status.Path;
    //    public static explicit operator PathBase(string value) => new PathBase(value);
    //}

    //public record FilePath : PathBase
    //{
    //    public FilePath(string path) : base(path)
    //    {
    //        var invalidPathChars = SystemIoProvider.GetInvalidPathChars();
    //        var invalidFilenameChars = SystemIoProvider.GetInvalidFileNameChars();

    //        var invalidPath = SystemIoProvider.GetDirectoryName(path).ToCharArray().Any(c => invalidPathChars.Contains(c));
    //        invalidPath |= SystemIoProvider.GetFileName(path).ToCharArray().Any(c => invalidFilenameChars.Contains(c));

    //        Path = path
    //           .GuardAgainstNull(nameof(path))
    //           .GuardAgainst(v => invalidPath, new ArgumentException($"The given value is not a valid file path {path}", nameof(path)));
    //    }

    //    public static FilePath From(string path) => new FilePath(path);
    //    public static FilePath From(string path, ISystemIoProvider systemIoProvider)
    //    {
    //        var filePath = new FilePath(path);
    //        filePath.SystemIoProvider = systemIoProvider;
    //        return filePath;
    //    }

    //    public static implicit operator string(FilePath status) => status.Path;
    //    public static explicit operator FilePath(string value) => new FilePath(value);
    //}

    //public record DirectoryPath : PathBase
    //{
    //    public DirectoryPath(string path) : base(path)
    //    {
    //        var invalidPathChars = SystemIoProvider.GetInvalidPathChars();

    //        var invalidPath = path.ToCharArray().Any(c => invalidPathChars.Contains(c));

    //        Path = path
    //               .GuardAgainstNull(nameof(path))
    //               .GuardAgainst(v => invalidPath, new ArgumentException($"The given value is not a valid file path {path}", nameof(path)));
    //    }

    //    public static DirectoryPath From(string path) => new DirectoryPath(path);
    //    public static DirectoryPath From(string path, ISystemIoProvider systemIoProvider)
    //    {
    //        var dirPath = new DirectoryPath(path);
    //        dirPath.SystemIoProvider = systemIoProvider;
    //        return dirPath;
    //    }

    //    public static implicit operator string(DirectoryPath status) => status.Path;
    //    public static explicit operator DirectoryPath(string value) => new DirectoryPath(value);
    //}

    public static class SystemIOExtensions
    {
        private static ISystemIoProvider systemIoProvider;
        internal static ISystemIoProvider SystemIoProvider
        {
            get => systemIoProvider ?? new DefaultSystemIoProvider();
            set => systemIoProvider = value;
        }

        public static HashSet<string> CopyFile(this string sourceFile, string destinationDir, bool overwrite = false)
        {
            destinationDir = destinationDir
                             .GuardAgainstInvalidDirectoryPath()
                             .GetDirectoryName()
                             .GetDirectoryPathWithTrailingSeparator();

            sourceFile = sourceFile
                         .GuardAgainstInvalidFilePath()
                         .GetFullPath();

            var sourceRootDir = sourceFile.GetPathRoot();

            var destinationFullPath = string.Empty;
            HashSet<string> createdPaths = new HashSet<string>();

            var directories = SystemIoProvider.GetDirectories(sourceRootDir, "*", SearchOption.AllDirectories);

            // create directories first
            foreach (var directory in directories)
            {
                if (!sourceFile.Contains(directory))
                    continue;

                destinationFullPath = directory.Replace(sourceRootDir, destinationDir);
                SystemIoProvider.CreateDirectory(destinationFullPath);
                createdPaths.Add(destinationFullPath);
            }

            //normalize path to get it crossplaform
            destinationFullPath = sourceFile.Replace(sourceRootDir, destinationDir);

            SystemIoProvider.CopyFile(sourceFile, destinationFullPath, overwrite);
            createdPaths.Add(destinationFullPath);

            return createdPaths;
        }

        public static string GuardAgainstInvalidFilePath(this string path)
        {
            var invalidPathChars = SystemIoProvider.GetInvalidPathChars();
            var invalidFilenameChars = SystemIoProvider.GetInvalidFileNameChars();

            var invalidPath = SystemIoProvider.GetDirectoryName(path).ToCharArray().Any(c => invalidPathChars.Contains(c));
            invalidPath |= SystemIoProvider.GetFileName(path).ToCharArray().Any(c => invalidFilenameChars.Contains(c));

            return
            path
            .GuardAgainstNull(nameof(path))
            .GuardAgainst(v => invalidPath, new IOException($"The given value is not a valid file path {path}"));
        }

        public static string GuardAgainstInvalidDirectoryPath(this string path)
        {
            var invalidPathChars = SystemIoProvider.GetInvalidPathChars();

            var invalidPath = path.ToCharArray().Any(c => invalidPathChars.Contains(c));

            return
            path
            .GuardAgainstNull(new IOException("The Given path is null."))
            .GuardAgainst(v => invalidPath, new IOException($"The given value is not a valid file path {path}"));
        }

        public static string GetFullPath(this string path)
            => SystemIoProvider.GetFullPath(path);

        public static string GetDirectoryName(this string path)
            => SystemIoProvider.GetDirectoryName(path);
        public static string GetDirectoryPathWithTrailingSeparator(this string directoryPath)
            => SystemIoProvider.EndsInDirectorySeparator(directoryPath) ? directoryPath : $"{directoryPath}{SystemIoProvider.DirectorySeparatorChar}";
        public static string[] GetDirectories(this string directory, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
            => SystemIoProvider.GetDirectories(directory.GuardAgainstInvalidDirectoryPath(), searchPattern, searchOption);

        public static string GetPathRoot(this string path) => SystemIoProvider.GetPathRoot(path);
    }
}
