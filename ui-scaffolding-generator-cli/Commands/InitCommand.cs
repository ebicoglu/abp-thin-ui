using Spectre.Console.Cli;

namespace AbpUiCli.Commands;

public sealed class InitCommand : Command<InitSettings>
{
    public override int Execute(CommandContext context, InitSettings settings) => InitRunner.Run(settings);
}
