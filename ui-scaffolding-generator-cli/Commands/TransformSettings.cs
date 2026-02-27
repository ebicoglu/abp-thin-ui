using Spectre.Console.Cli;

namespace AbpUiCli.Commands;

public sealed class TransformSettings : CommandSettings
{
    [CommandOption("--project <path>")]
    public string? Project { get; init; }

    [CommandOption("--layout <sidebar|topnav>")]
    public string Layout { get; init; } = "sidebar";

    [CommandOption("--primary <color>")]
    public string? Primary { get; init; }

    [CommandOption("--accent <color>")]
    public string? Accent { get; init; }

    [CommandOption("--radius <value>")]
    public string? Radius { get; init; }

    [CommandOption("--font-sans <font>")]
    public string? FontSans { get; init; }

    [CommandOption("--app-name <name>")]
    public string? AppName { get; init; }

    [CommandOption("--logo <path>")]
    public string? Logo { get; init; }
}
