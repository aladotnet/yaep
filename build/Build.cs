using System;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.ChangeLog;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using ParameterAttribute = Nuke.Common.ParameterAttribute;
using System.Threading.Tasks;
using Octokit;
using Octokit.Internal;

[GitHubActions("build-test-publish",
GitHubActionsImage.UbuntuLatest,
AutoGenerate = true,
FetchDepth = 0,
    OnPushBranches = new[]
    {
        "main",
        "dev",
        "releases/**"
    },
    OnPullRequestBranches = new[]
    {
        "releases/**"
    },
    InvokedTargets = new[]
    {
        nameof(Pack),
    },
    EnableGitHubToken = true,
    ImportSecrets = new[]
    {
        nameof(NuGetApiKey)
    }
)]

[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    //terminal command : nuke Pack --nuget-api-key apiKey --configuration 'Release'

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Nuget Feed Url for Public Access of Pre Releases")]
    readonly string NugetFeed;
    [Parameter("Nuget API key to publish the artifacts"), Secret]
    readonly string NuGetApiKey;
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    [Parameter("Copyright Details")]
    readonly string Copyright;

    [Parameter("Artifacts Type")]
    readonly string ArtifactsType;

    [Parameter("Excluded Artifacts Type")]
    readonly string ExcludedArtifactsType;

    [Solution(GenerateProjects = true)] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;
    static GitHubActions GitHubActions => GitHubActions.Instance;
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath TestResultDirectory => OutputDirectory / "test-results";
    AbsolutePath PackagesDirectory => OutputDirectory / "packages";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    static readonly string PackageContentType = "application/octet-stream";
    static string ChangeLogFile => RootDirectory / "CHANGELOG.md";
    string GithubNugetFeed => GitHubActions != null
      ? $"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json"
      : null;

    Target Clean => _ => _
        .Before(Restore)
        .Description($"Cleaning.")
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Description($"Restoring Project Dependencies.")
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution.Extensions.CoreExtensions));
        });

    Target Compile => _ => _
        .Description($"Building Project with the version.")
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution.Extensions.CoreExtensions)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
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
      .Description($"Packing Project with the version.")
      .Requires(() => Configuration.Equals(Configuration.Release))
      .Requires(() => !string.IsNullOrWhiteSpace(NuGetApiKey))
      .Produces(ArtifactsDirectory / ArtifactsType)
      //.Triggers(PublishToGithub, PublishToNuGet)
      .Triggers(PublishToNuGet)
      .DependsOn(Compile, Test)
      .Executes(() =>
      {
          Serilog.Log.Information("Solution ");

          DotNetPack(s => s
              .SetProject(Solution.Extensions.CoreExtensions)
              .SetPackageId("yaep")
              .SetOutputDirectory(ArtifactsDirectory)
              .SetNoBuild(SucceededTargets.Contains(Compile))
              .SetRepositoryUrl(GitRepository.HttpsUrl)
              .SetVersion(GitVersion.NuGetVersionV2)
              .SetConfiguration(Configuration)
              .EnableNoRestore()
              .EnableNoBuild());
      });

    Target PublishToGithub => _ => _
       .Description($"Publishing to Github for Development only.")
       //.Triggers(CreateRelease)
       .Requires(() => Configuration.Equals(Configuration.Release))
       .OnlyWhenStatic(() => GitRepository.IsOnDevelopBranch() || GitHubActions.IsPullRequest)
       .Executes(() =>
       {
           GlobFiles(ArtifactsDirectory, ArtifactsType)
               .Where(x => !x.EndsWith(ExcludedArtifactsType))
               .ForEach(x =>
               {
                   DotNetNuGetPush(s => s
                       .SetTargetPath(x)
                       .SetSource(GithubNugetFeed)
                       .SetApiKey(GitHubActions.Token)
                       .EnableSkipDuplicate()
                   );
               });
       });
    Target PublishToNuGet => _ => _
       .Description($"Publishing to NuGet with the version.")
       //.Triggers(CreateRelease)
       .Requires(() => Configuration.Equals(Configuration.Release))
       .OnlyWhenStatic(() => GitRepository.IsOnMainOrMasterBranch())
       .Executes(() =>
       {
           Serilog.Log.Information("ApiKey {0}", NuGetApiKey);

           if (NuGetApiKey.IsNullOrEmpty())
           {
               Serilog.Log.Warning("No API Key found");
               return;
           }

           GlobFiles(ArtifactsDirectory, ArtifactsType)
               .Where(x => !x.EndsWith(ExcludedArtifactsType))
               .ForEach(x =>
               {
                   DotNetNuGetPush(s => s
                       .SetTargetPath(x)
                       .SetSource(NugetFeed)
                       .SetApiKey(NuGetApiKey)                       
                       .EnableSkipDuplicate()
                   );
               });
       });

    Target CreateRelease => _ => _
       .Description($"Creating release for the publishable version.")
       .Requires(() => Configuration.Equals(Configuration.Release))
       .OnlyWhenStatic(() => GitRepository.IsOnMainOrMasterBranch() || GitRepository.IsOnReleaseBranch())
       .Executes(async () =>
       {
           var credentials = new Credentials(GitHubActions.Token);
           GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue(nameof(NukeBuild)),
               new InMemoryCredentialStore(credentials));

           var (owner, name) = (GitRepository.GetGitHubOwner(), GitRepository.GetGitHubName());

           var releaseTag = GitVersion.NuGetVersionV2;


           var changeLogSectionEntries = ChangelogTasks.ExtractChangelogSectionNotes(ChangeLogFile);
           var latestChangeLog = changeLogSectionEntries
               .Aggregate((c, n) => c + Environment.NewLine + n);

           var newRelease = new NewRelease(releaseTag)
           {
               TargetCommitish = GitVersion.Sha,
               Draft = true,
               Name = $"v{releaseTag}",
               Prerelease = !string.IsNullOrEmpty(GitVersion.PreReleaseTag),
               Body = latestChangeLog
           };

           var release = GitHubTasks
                         .GitHubClient
                         .Repository
                         .Release;

           if (release is null)
           {
               Serilog.Log.Error("Release client is null");
               return;
           }


           var createdRelease = await GitHubTasks
                                       .GitHubClient
                                       .Repository
                                       .Release.Create(owner, name, newRelease);

           if (string.IsNullOrEmpty(ArtifactsType))
           {
               Serilog.Log.Error("ArtifactsType is null");
               return;
           }
           var globalfiles = GlobFiles(ArtifactsDirectory, ArtifactsType)
                         .Where(x => !x.EndsWith(ExcludedArtifactsType)).ToArray();

           if (globalfiles.IsNullOrEmpty())
           {
               Serilog.Log.Error("GlobalFiles is null");
               return;
           }

           foreach (var item in globalfiles)
           {
               Serilog.Log.Information("Globalfile : {0}", item);
           }

           GlobFiles(ArtifactsDirectory, ArtifactsType)
              .Where(x => !x.EndsWith(ExcludedArtifactsType))
              .ForEach(async x => await UploadReleaseAssetToGithub(createdRelease, x));

           await GitHubTasks
                      .GitHubClient
                      .Repository
                      .Release
              .Edit(owner, name, createdRelease.Id, new ReleaseUpdate { Draft = false });
       });

    private static async Task UploadReleaseAssetToGithub(Release release, string asset)
    {
        await using var artifactStream = File.OpenRead(asset);
        var fileName = Path.GetFileName(asset);
        var assetUpload = new ReleaseAssetUpload
        {
            FileName = fileName,
            ContentType = PackageContentType,
            RawData = artifactStream,
        };
        await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(release, assetUpload);
    }
}
