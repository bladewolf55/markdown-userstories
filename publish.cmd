nircmd elevate %systemroot%\system32\inetsrv\AppCmd.exe recycle apppool "MD User Stories"
dotnet publish "source\MarkdownUserStories\MarkdownUserStories.csproj" /p:PublishProfile=LocalWebDeploy"