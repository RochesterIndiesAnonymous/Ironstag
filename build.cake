var target = Argument("target", "Test");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .WithCriteria(c => HasArgument("rebuild"))
    .Does(() =>
{
    CleanDirectory($"./WesternSpace/bin/{configuration}");
    //CleanDirectory($"./XnaXmlContentPipeline/bin/{configuration}");
    //CleanDirectory($"./XnaXmlContentReader/bin/{configuration}");
});

Task("BuildXnaXmlContentReaderDesktopGL")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetRestore("./XnaXmlContentReader/.csproj");
    //PackDotnet(".XnaXmlContentReader/XnaXmlContentReader.csproj");
});

Task("BuildXnaXmlContentPipelineDesktopGL")
    .IsDependentOn("BuildXnaXmlContentReaderDesktopGL")
    .Does(() =>
{
    DotNetRestore("./XnaXmlContentPipeline/XnaXmlContentPipeline.csproj.csproj");
    //PackDotnet("./XnaXmlContentPipeline/XnaXmlContentPipeline.csproj.csproj");
});

// Task("Build")
//     .IsDependentOn("Clean")
//     .Does(() =>
// {
//     DotNetBuild("./Ironstag.sln", new DotNetBuildSettings
//     {
//         Configuration = configuration,
//     });
// });

// Task("Test")
//     .IsDependentOn("Build")
//     .Does(() =>
// {
//     DotNetTest("./Ironstag.sln", new DotNetTestSettings
//     {
//         Configuration = configuration,
//         NoBuild = true,
//     });
// });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
