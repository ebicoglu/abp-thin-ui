using AbpUiCli.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(config =>
{
    config.SetApplicationName("abp-ui");
    config.AddCommand<InitCommand>("init")
        .WithDescription("Scaffold the base UI template into a folder.");
    config.AddCommand<TransformCommand>("transform")
        .WithDescription("Apply layout (sidebar|topnav) and branding to the current project.");
});
return app.Run(args);
