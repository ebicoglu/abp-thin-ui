using Spectre.Console.Cli;

namespace AbpUiCli.Commands;

public sealed class InitSettings : CommandSettings
{
    [CommandArgument(0, "<targetDir>")]
    public required string TargetDir { get; init; }

    [CommandOption("--template-path <path>")]
    public string? TemplatePath { get; init; }

    [CommandOption("--api-url <url>")]
    public string? ApiUrl { get; init; }

    [CommandOption("--backend-path <path>")]
    public string? BackendPath { get; init; }
}
