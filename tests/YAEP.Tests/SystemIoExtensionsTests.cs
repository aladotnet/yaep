using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace YAEPTests
{
    internal class FakeSystemIoProvider : ISystemIoProvider
    {
        private readonly IFileSystem _fileSystem;

        public FakeSystemIoProvider(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public char AltDirectorySeparatorChar => _fileSystem.Path.AltDirectorySeparatorChar;

        public char PathSeparator => _fileSystem.Path.PathSeparator;

        public char DirectorySeparatorChar => _fileSystem.Path.DirectorySeparatorChar;

        public void CopyFile(string sourcePath, string destinationPath, bool overwrite = false) => _fileSystem.File.Copy(sourcePath, destinationPath, overwrite);

        public void CreateDirectory(string path) => _fileSystem.Directory.CreateDirectory(path);

        public bool DirectoryExists(string path) => _fileSystem.Directory.Exists(path);

        public bool EndsInDirectorySeparator(string path) => _fileSystem.Path.EndsInDirectorySeparator(path);

        public bool FileExists(string path) => _fileSystem.File.Exists(path);

        public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) => _fileSystem.Directory.GetDirectories(path, searchPattern, searchOption);

        public string GetDirectoryName(string filePath) => _fileSystem.Path.GetDirectoryName(filePath);

        public string GetFileName(string filePath) => _fileSystem.Path.GetFileName(filePath);

        public string GetFullPath(string path) => _fileSystem.Path.GetFullPath(path);

        public char[] GetInvalidFileNameChars() => _fileSystem.Path.GetInvalidFileNameChars();

        public char[] GetInvalidPathChars() => _fileSystem.Path.GetInvalidPathChars();

        public string GetPathRoot(string path) => _fileSystem.Path.GetPathRoot(path);

        public string PathCombine(string path1, string path2) => _fileSystem.Path.Combine(path1, path2);

        public string PathCombine(string path1, string path2, string path3) => _fileSystem.Path.Combine(path1, path2, path3);

        public string PathCombine(string path1, string path2, string path3, string path4) => _fileSystem.Path.Combine(path1, path2, path3, path4);

        public string PathCombine(string path1, string path2, string path3, string path4, string path5) => _fileSystem.Path.Combine(path1, path2, path3, path4, path5);

        public string PathCombine(string path1, string path2, string path3, string path4, string path5, string path6) => _fileSystem.Path.Combine(path1, path2, path3, path4, path5, path6);

        public string PathCombine(string path1, string path2, string path3, string path4, string path5, string path6, string path7) => _fileSystem.Path.Combine(path1, path2, path3, path4, path5, path6, path7);
    }

    [UsesVerify]
    public class SystemIoExtensionsTests
    {
        private readonly char[] _invalidPathChars = Path.GetInvalidPathChars();

        [Theory]
        [InlineData(@"c:\myfile1.txt", @"c:\parent1\sub1\")]
        [InlineData(@"c:\f1\f2\f4\f5\myfile2.txt", @"c:\parent2\sub1\")]
        [InlineData(@"c:/myfile3.txt", @"c:/parent3/sub1/")]
        [InlineData(@"c:\myfile4.txt", @"c:\parent4\sub1")]    //sub 1 is habdled as a file (a directory should always habe a trailing separator)
        [InlineData(@"c:\sub100\myfile5", @"c:\parent5\sub1")] 
        public async Task CopyFile_copies_given_file_to_destinationDirectory(string sourceFilePath, string destinationDir)
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
            });

            
            fileSystem.AddDirectory(destinationDir);
            fileSystem.AddFile(sourceFilePath, new MockFileData(""));
            SystemIOExtensions.SystemIoProvider = new FakeSystemIoProvider(fileSystem);

            var addedFilesAndDirs = sourceFilePath.CopyFile(destinationDir);

            await Verifier.Verify(new { fileSystem.AllPaths, addedFilesAndDirs })
                .UseParameters(sourceFilePath, destinationDir);
        }


        [Theory]
        [InlineData(@"c:\myfile1.txt", @"c:\parent1\sub1\")]
        [InlineData(@"c:\f1\f2\f4\f5\myfile2.txt", @"c:\parent2\sub1\")]
        [InlineData(@"c:/myfile3.txt", @"c:/parent3/sub1/")]
        [InlineData(@"c:\myfile4.txt", @"c:\parent4\sub1")]    //sub 1 is habdled as a file (a directory should always habe a trailing separator)
        [InlineData(@"c:\sub100\myfile5", @"c:\parent5\sub1")]
        [InlineData(@"c:\myfile1.txt\", @"c:\parent1\sub1\")]
        public async Task CopyFile_copies_given_invalid_paths_throws_ioexception(string sourceFilePath, string destinationDir)
        {
            var rand = new Random();
            var index = rand.Next(0,_invalidPathChars.Length - 1);

            var invalidChar = _invalidPathChars[index];

            destinationDir = $"{destinationDir}{invalidChar}" ;
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
            });


            fileSystem.AddDirectory(destinationDir);
            fileSystem.AddFile(sourceFilePath, new MockFileData(""));
            SystemIOExtensions.SystemIoProvider = new FakeSystemIoProvider(fileSystem);


            sourceFilePath.Invoking(s=>  s.CopyFile(destinationDir))
                .Should().ThrowExactly<IOException>();
        }

    }
}
