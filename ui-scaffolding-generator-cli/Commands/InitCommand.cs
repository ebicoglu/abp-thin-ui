using Spectre.Console;
using Spectre.Console.Cli;

namespace AbpUiCli.Commands;

public sealed class InitCommand : Command<InitSettings>
{
    public override int Execute(CommandContext context, InitSettings settings)
    {
        var templatePath = settings.TemplatePath ?? Path.Combine(Environment.CurrentDirectory, "ui-base-templates", "react");
        var targetDir = Path.IsPathRooted(settings.TargetDir)
            ? settings.TargetDir
            : Path.Combine(Environment.CurrentDirectory, settings.TargetDir);

        if (!Directory.Exists(templatePath))
        {
            AnsiConsole.MarkupLine("[red]Template not found:[/] {0}", templatePath);
            AnsiConsole.MarkupLine("Use [yellow]--template-path <path>[/] to specify the base template (e.g. ui-base-templates/react).");
            return 1;
        }

        if (Directory.Exists(targetDir) && Directory.EnumerateFileSystemEntries(targetDir).Any())
        {
            AnsiConsole.MarkupLine("[red]Target directory is not empty:[/] {0}", targetDir);
            return 1;
        }

        try
        {
            CopyDirectory(templatePath, targetDir);
            AnsiConsole.MarkupLine("[green]Created[/] project at [bold]{0}[/]", targetDir);
            AnsiConsole.MarkupLine("Run [yellow]abp-ui transform --layout=topnav[/] (from the new project folder) to change layout or branding.");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }

    private static void CopyDirectory(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);
        foreach (var entry in new DirectoryInfo(sourceDir).EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly))
        {
            var destPath = Path.Combine(targetDir, entry.Name);
            if (entry is DirectoryInfo di)
            {
                CopyDirectory(di.FullName, destPath);
            }
            else
            {
                File.Copy(entry.FullName, destPath);
            }
        }
    }
}
