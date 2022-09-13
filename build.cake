var target = Argument("target", "Test");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .WithCriteria(c => HasArgument("rebuild"))
    .Does(() =>
{
    CleanDirectory("./WesternSpace/bin");
    CleanDirectory("./XnaXmlContentPipeline/bin");
    CleanDirectory("./XnaXmlContentReader/bin");
});

Task("DownloadDependencies")
    .WithCriteria(() => IsRunningOnMacOs())
    .Does(() =>
{
    StartProcess("sh", new ProcessSettings{
        Arguments = "./mgfxc_build_dependencies.sh"
    });

    StartProcess("sh", new ProcessSettings{
        Arguments = "./mgfxc_wine_setup.sh"
    });

    // Set MGFXC_WINE_PATH for building shaders on macOS and Linux
    System.Environment.SetEnvironmentVariable("MGFXC_WINE_PATH", EnvironmentVariable("HOME") + "/.winemonogame");
});

Task("BuildXnaXmlContentReader")
    //.IsDependentOn("Clean")
    .Does(() =>
{
    DotNetRestore("./XnaXmlContentReader/XnaXmlContentReader.csproj");
    DotNetBuild("./XnaXmlContentReader/XnaXmlContentReader.csproj", new DotNetBuildSettings
    {
        Configuration = configuration,
    });
    //PackDotnet(".XnaXmlContentReader/XnaXmlContentReader.csproj");
});

Task("BuildXnaXmlContentPipeline")
    .IsDependentOn("BuildXnaXmlContentReader")
    .Does(() =>
{
    DotNetRestore("./XnaXmlContentPipeline/XnaXmlContentPipeline.csproj");
    DotNetBuild("./XnaXmlContentPipeline/XnaXmlContentPipeline.csproj", new DotNetBuildSettings
    {
        Configuration = configuration,
    });
    //PackDotnet("./XnaXmlContentPipeline/XnaXmlContentPipeline.csproj.csproj");
});

Task("Build")
    .IsDependentOn("DownloadDependencies")
    .IsDependentOn("BuildXnaXmlContentReader")
    .IsDependentOn("BuildXnaXmlContentPipeline")
    .Does(() =>
{
    DotNetRestore("./WesternSpace/WesternSpace.csproj");
    DotNetBuild("./WesternSpace/WesternSpace.csproj", new DotNetBuildSettings
    {
        Configuration = configuration,
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    // DotNetTest("./Ironstag.sln", new DotNetTestSettings
    // {
    //     Configuration = configuration,
    //     NoBuild = true,
    // });
});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
