using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile);


    [Parameter("Nuget API key to publish the artifacts")]
    readonly string NuGetApiKey;
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {            
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild());
        });

    Target Pack => _ => _
      .DependsOn(Clean, Compile,Test)
      .Executes(() =>
      {
          DotNetPack(s => s
              .SetProject(Solution.GetProject("CoreExtensions"))
              .SetOutputDirectory(ArtifactsDirectory)
              .SetIncludeSymbols(true)
              .SetConfiguration(Configuration)
              .EnableNoRestore()
              .EnableNoBuild());
      });



    Target Publish => _ => _
      .DependsOn(Pack)
      .Requires(() => NuGetApiKey)
      .Executes(() =>
      {
          (string feedUrl, string symbolsFeedUrl, string apiKey) = GetPublishTargetSettings();
          var packageFile = GlobFiles(ArtifactsDirectory, "*.nupkg")
                            .First(f => !f.EndsWith("symbols.nupkg"));

          DotNetNuGetPush(s => s
              .SetTargetPath(packageFile)
              .SetSource(feedUrl)
              .SetSymbolSource(symbolsFeedUrl)
              .SetApiKey(apiKey)
          );
      });

    (string feedUrl, string symbolsFeedUrl, string apiKey) GetPublishTargetSettings()
        => GitRepository.Branch switch
        {
            "main" => (
                "https://api.nuget.org/v3/index.json",
                "https://api.nuget.org/v3/index.json",
                NuGetApiKey),

            _ => throw new InvalidOperationException($"Current branch \"{GitRepository.Branch}\" should not be publishing packages!")
        };

}
