using AbpUiCli.Transformer;
using Spectre.Console;

namespace AbpUiCli.Commands;

/// <summary>
/// Shared init logic so both InitCommand and BuildWizard can scaffold a project.
/// </summary>
public static class InitRunner
{
    private const string DefaultApiUrl = "https://localhost:44317";

    /// <summary>
    /// Resolves the template path: use explicit path if set; otherwise look for ui-base-templates/react
    /// in the current directory or in any parent directory (so it works when run from repo root or from ui-scaffolding-generator-cli).
    /// </summary>
    private static string ResolveDefaultTemplatePath()
    {
        var dir = new DirectoryInfo(Environment.CurrentDirectory);
        for (var i = 0; i < 10 && dir != null; i++, dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "ui-base-templates", "react");
            if (Directory.Exists(candidate))
                return candidate;
        }
        return Path.Combine(Environment.CurrentDirectory, "ui-base-templates", "react");
    }

    public static int Run(InitSettings settings)
    {
        var templatePath = settings.TemplatePath ?? ResolveDefaultTemplatePath();
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

            var apiUrl = DefaultApiUrl;
            if (!string.IsNullOrWhiteSpace(settings.BackendPath))
            {
                var hostDir = BackendConfigurator.ResolveHostDirectory(settings.BackendPath);
                if (hostDir != null)
                {
                    var fromBackend = BackendConfigurator.GetApiUrl(hostDir);
                    if (!string.IsNullOrEmpty(fromBackend))
                        apiUrl = fromBackend;
                    BackendConfigurator.AddCorsForReactApp(hostDir);
                    AnsiConsole.MarkupLine("[green]Backend[/] CORS updated at [dim]{0}[/]", hostDir);
                }
                else
                {
                    AnsiConsole.MarkupLine("[yellow]Backend path not found or no appsettings.json:[/] {0}", settings.BackendPath);
                }
            }

            if (!string.IsNullOrWhiteSpace(settings.ApiUrl))
                apiUrl = settings.ApiUrl.TrimEnd('/');

            WriteEnvFile(targetDir, apiUrl);
            EnsureEnvInGitignore(targetDir);

            AnsiConsole.MarkupLine("[green]Created[/] project at [bold]{0}[/]", targetDir);
            AnsiConsole.MarkupLine("[green]Connected[/] to API: [bold]{0}[/] ([dim].env[/])", apiUrl);
            AnsiConsole.MarkupLine("Run [yellow]abp-factory transform --layout=topnav[/] (from the new project folder) to change layout or branding.");
            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }

    internal static void WriteEnvFile(string targetDir, string apiUrl)
    {
        var envPath = Path.Combine(targetDir, ".env");
        var content = "# ABP API base URL – set by ABP Factory (abp-factory init) (no trailing slash)" + Environment.NewLine
            + "VITE_ABP_API_URL=" + apiUrl + Environment.NewLine;
        File.WriteAllText(envPath, content);
    }

    private static void EnsureEnvInGitignore(string targetDir)
    {
        var gitignorePath = Path.Combine(targetDir, ".gitignore");
        if (!File.Exists(gitignorePath)) return;
        var content = File.ReadAllText(gitignorePath);
        if (content.Contains(".env")) return;
        var line = content.EndsWith("\n") ? ".env\n" : "\n.env\n";
        File.AppendAllText(gitignorePath, line);
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
