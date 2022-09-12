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
    //CleanDirectory($"./WesternSpace/bin/{configuration}");
    //CleanDirectory($"./XnaXmlContentPipeline/bin/{configuration}");
    //CleanDirectory($"./XnaXmlContentReader/bin/{configuration}");
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
