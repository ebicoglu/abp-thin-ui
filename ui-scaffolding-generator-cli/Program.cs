using AbpUiCli.Commands;
using Spectre.Console.Cli;

// No arguments: interactive "Let's build something..." wizard
if (args.Length == 0)
    return BuildWizard.Run();

var app = new CommandApp();
app.Configure(config =>
{
    config.SetApplicationName("abp-factory");
    config.AddCommand<InitCommand>("init")
        .WithDescription("Scaffold the base UI template into a folder.");
    config.AddCommand<TransformCommand>("transform")
        .WithDescription("Apply layout (sidebar|topnav) and branding to the current project.");
    config.AddCommand<ConfigCommand>("config")
        .WithDescription("Show or set configuration (e.g. OpenAI API key for AI-assisted design).");
});
return app.Run(args);
