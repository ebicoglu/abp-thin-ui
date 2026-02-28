using AbpUiCli.Transformer;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AbpUiCli.Commands;

public sealed class TransformCommand : Command<TransformSettings>
{
    public override int Execute(CommandContext context, TransformSettings settings)
    {
        var projectRoot = string.IsNullOrWhiteSpace(settings.Project)
            ? Environment.CurrentDirectory
            : (Path.IsPathRooted(settings.Project)
                ? settings.Project
                : Path.Combine(Environment.CurrentDirectory, settings.Project));

        var packageJson = Path.Combine(projectRoot, "package.json");
        var appShellDir = Path.Combine(projectRoot, "src", "app-shell");
        if (!File.Exists(packageJson) || !Directory.Exists(appShellDir))
        {
            AnsiConsole.MarkupLine("[red]Not a transformer-ready project.[/] Run from a folder created with [yellow]abp-factory init[/], or use [yellow]--project <path>[/].");
            return 1;
        }

        var layout = settings.Layout?.ToLowerInvariant() switch
        {
            "sidebar" => "sidebar",
            "topnav" => "topnav",
            _ => "sidebar"
        };

        var brand = new BrandTokens
        {
            Primary = settings.Primary ?? "220 70% 50%",
            Accent = settings.Accent ?? "220 15% 92%",
            Radius = settings.Radius ?? "0.5rem",
            FontSans = settings.FontSans,
            AppName = settings.AppName,
            LogoPath = settings.Logo
        };

        try
        {
            var orchestrator = new TransformOrchestrator();
            orchestrator.Apply(projectRoot, layout, brand);
            AnsiConsole.MarkupLine("[green]Transformed[/] layout=[bold]{0}[/], branding updated. [dim]See template.meta/applied.json[/]", layout);
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }
}
