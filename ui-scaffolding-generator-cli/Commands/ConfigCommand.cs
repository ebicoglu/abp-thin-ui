using AbpUiCli.Config;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AbpUiCli.Commands;

public sealed class ConfigCommand : Command<ConfigSettings>
{
    public override int Execute(CommandContext context, ConfigSettings settings)
    {
        if (settings.SetOpenAIKey)
        {
            var key = settings.OpenAIKeyValue;
            if (string.IsNullOrWhiteSpace(key))
            {
                key = AnsiConsole.Prompt(
                    new TextPrompt<string>("[yellow]OpenAI API Key[/] (sk-...):")
                        .Secret()
                        .AllowEmpty());
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                AnsiConsole.MarkupLine("[yellow]No key entered; config unchanged.[/]");
                return 0;
            }

            AbpUiConfig.SetOpenAIApiKey(key);
            AnsiConsole.MarkupLine("[green]OpenAI API key saved[/] to [dim]{0}[/]", AbpUiConfig.GetConfigFilePath());
            return 0;
        }

        // Show current config status
        var path = AbpUiConfig.GetConfigFilePath();
        var fromEnv = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ABP_UI_OPENAI_API_KEY"))
            || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

        AnsiConsole.MarkupLine("[bold]ABP Factory config[/]");
        AnsiConsole.MarkupLine("  Config file: [dim]{0}[/]", path);
        AnsiConsole.MarkupLine("  OpenAI API Key: {0}",
            fromEnv ? "[green]set via environment[/]" : (AbpUiConfig.GetOpenAIApiKey() != null ? "[green]set[/]" : "[yellow]not set[/]"));
        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("To set OpenAI API key (for AI-assisted design):");
        AnsiConsole.MarkupLine("  [cyan]abp-factory config --set-openai-key[/]");
        AnsiConsole.MarkupLine("  or set [cyan]ABP_UI_OPENAI_API_KEY[/] or [cyan]OPENAI_API_KEY[/] environment variable.");
        return 0;
    }
}
