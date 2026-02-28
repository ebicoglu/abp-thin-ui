using Spectre.Console.Cli;

namespace AbpUiCli.Commands;

public sealed class ConfigSettings : CommandSettings
{
    [CommandOption("--set-openai-key [key]")]
    public bool SetOpenAIKey { get; init; }

    [CommandOption("--openai-key <key>")]
    public string? OpenAIKeyValue { get; init; }
}
