using System.Reflection;
using AbpUiCli.Config;
using AbpUiCli.Transformer;
using Spectre.Console;

namespace AbpUiCli.Commands;

/// <summary>
/// Interactive flow when abp-factory is run with no arguments: "Let's build something..."
/// Option 1: Design based on existing website (AI via Copilot SDK).
/// Option 2: Design from an existing Figma project (Figma file link).
/// Option 3: Choose your custom design (manual prompts then init + transform).
/// </summary>
public static class BuildWizard
{
    /// <summary>Figma file URL pattern: https://www.figma.com/file/XXXXXXXXXXXX/Project-Name </summary>
    private static readonly System.Text.RegularExpressions.Regex FigmaFileUrlRegex = new(
        @"^https?://(www\.)?figma\.com/file/([a-zA-Z0-9]+)/[^\s]+$",
        System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    public static int Run()
    {
        AnsiConsole.Write(new FigletText("ABP Factory").Color(Color.Cyan1));
        AnsiConsole.MarkupLine("[bold]Let's build something. Tell me what'd you like to build today?[/]");
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose how to create your UI:")
                .AddChoices(
                    "1. Design based on an existing web site",
                    "2. Design from an existing Figma project",
                    "3. Choose your custom design"
                ));

        if (choice.StartsWith("1"))
            return RunDesignFromWebsite();
        if (choice.StartsWith("2"))
            return RunDesignFromFigma();
        return RunCustomDesign();
    }

    private static int RunDesignFromWebsite()
    {
        AnsiConsole.MarkupLine("[dim]Describe the site you want to mimic (e.g. \"apple.com.tr gibi bir site\" or \"minimal like stripe.com\").[/]");
        var description = AnsiConsole.Prompt(new TextPrompt<string>("[yellow]Your description:[/]").AllowEmpty());
        if (string.IsNullOrWhiteSpace(description))
        {
            AnsiConsole.MarkupLine("[yellow]No description entered. Exiting.[/]");
            return 0;
        }

        var (projectName, parentPath, backendPath) = PromptProjectNameAndLocation();
        if (projectName == null)
            return 0;

        var apiKey = GetOrPromptOpenAIApiKey();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            AnsiConsole.MarkupLine("[yellow]AI flow cancelled (no API key).[/]");
            return 0;
        }

        AnsiConsole.MarkupLine("[dim]Analyzing your request with AI to infer design parameters...[/]");
        var designParams = GetDesignParamsFromAgentAsync(description.Trim(), apiKey, CopilotFlowMode.Website).GetAwaiter().GetResult();
        if (designParams == null)
        {
            AnsiConsole.MarkupLine("[yellow]Using default design parameters.[/]");
            designParams = DesignParams.Default;
        }

        var siteUrlForFavicon = designParams.SiteUrl ?? FaviconFetcher.TryExtractUrlFromDescription(description);
        return RunInitAndTransformWithProgress(projectName, parentPath, backendPath, designParams, siteUrlForFavicon);
    }

    private static int RunDesignFromFigma()
    {
        AnsiConsole.MarkupLine("[dim]Paste your Figma file link (e.g. https://www.figma.com/file/XXXXXXXXXXXX/Project-Name).[/]");
        var figmaUrl = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Figma file URL:[/]")
                .Validate(url =>
                {
                    if (string.IsNullOrWhiteSpace(url)) return ValidationResult.Error("URL cannot be empty.");
                    if (!FigmaFileUrlRegex.IsMatch(url.Trim()))
                        return ValidationResult.Error("Use format: https://www.figma.com/file/XXXXXXXXXXXX/Project-Name");
                    return ValidationResult.Success();
                })
                .AllowEmpty());
        if (string.IsNullOrWhiteSpace(figmaUrl))
        {
            AnsiConsole.MarkupLine("[yellow]No URL entered. Exiting.[/]");
            return 0;
        }

        figmaUrl = figmaUrl.Trim();

        var (projectName, parentPath, backendPath) = PromptProjectNameAndLocation();
        if (projectName == null)
            return 0;

        var apiKey = GetOrPromptOpenAIApiKey();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            AnsiConsole.MarkupLine("[yellow]AI flow cancelled (no API key).[/]");
            return 0;
        }

        AnsiConsole.MarkupLine("[dim]Analyzing Figma project with AI to infer design parameters...[/]");
        var designParams = GetDesignParamsFromAgentAsync(figmaUrl, apiKey, CopilotFlowMode.Figma).GetAwaiter().GetResult();
        if (designParams == null)
        {
            AnsiConsole.MarkupLine("[yellow]Using default design parameters.[/]");
            designParams = DesignParams.Default;
        }

        return RunInitAndTransformWithProgress(projectName, parentPath, backendPath, designParams);
    }

    /// <summary>
    /// Asks for project name (folder name) and optional parent path. Returns (projectName, parentPath, backendPath) or (null, null, null) if cancelled.
    /// </summary>
    private static (string? projectName, string parentPath, string? backendPath) PromptProjectNameAndLocation()
    {
        var projectName = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Project name[/] (folder name, e.g. my-app):")
                .DefaultValue("my-app")
                .AllowEmpty());
        if (string.IsNullOrWhiteSpace(projectName))
            projectName = "my-app";
        projectName = projectName.Trim();

        var parentPath = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Create project in[/] (path; leave empty for current directory):")
                .DefaultValue("")
                .AllowEmpty());
        parentPath = string.IsNullOrWhiteSpace(parentPath) ? Environment.CurrentDirectory : parentPath.Trim();
        if (!Path.IsPathRooted(parentPath))
            parentPath = Path.Combine(Environment.CurrentDirectory, parentPath);

        var backendPath = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Backend path[/] (optional, for CORS/API URL):")
                .AllowEmpty());
        if (string.IsNullOrWhiteSpace(backendPath))
            backendPath = null;

        return (projectName, parentPath, backendPath);
    }

    private sealed record DesignParams(string Layout, string Primary, string Accent, string Radius, string? FontSans, string AppName, string? SiteUrl)
    {
        public static readonly DesignParams Default = new("sidebar", "220 70% 50%", "220 15% 92%", "0.5rem", null, "My App", null);
    }

    /// <summary>
    /// Returns OpenAI API key from config/env. If not set, prompts the user and saves it for next time.
    /// </summary>
    private static string? GetOrPromptOpenAIApiKey()
    {
        var key = AbpUiConfig.GetOpenAIApiKey();
        if (!string.IsNullOrWhiteSpace(key))
            return key;

        AnsiConsole.MarkupLine("[yellow]OpenAI API key is not set.[/] Enter it now (it will be saved for next time).");
        key = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]OpenAI API Key[/] (sk-...):")
                .Secret()
                .AllowEmpty());

        if (string.IsNullOrWhiteSpace(key))
            return null;

        key = key.Trim();
        AbpUiConfig.SetOpenAIApiKey(key);
        AnsiConsole.MarkupLine("[green]API key saved.[/]");
        return key;
    }

    private enum CopilotFlowMode { Website, Figma }

    /// <summary>
    /// Calls AI to analyze the request and return design parameters only. Does not run any commands.
    /// </summary>
    private static async Task<DesignParams?> GetDesignParamsFromAgentAsync(string userPrompt, string apiKey, CopilotFlowMode mode)
    {
        var systemMessage = mode == CopilotFlowMode.Figma
            ? BuildFigmaParamsOnlySystemMessage(userPrompt)
            : BuildWebsiteParamsOnlySystemMessage(userPrompt);

        string? lastContent = null;
        try
        {
            await using var client = new GitHub.Copilot.SDK.CopilotClient(new GitHub.Copilot.SDK.CopilotClientOptions { Cwd = Environment.CurrentDirectory, AutoStart = true });
            await client.StartAsync();

            await using var session = await client.CreateSessionAsync(new GitHub.Copilot.SDK.SessionConfig
            {
                Model = "gpt-4.1",
                OnPermissionRequest = GitHub.Copilot.SDK.PermissionHandler.ApproveAll,
                Provider = new GitHub.Copilot.SDK.ProviderConfig { Type = "openai", BaseUrl = "https://api.openai.com/v1", ApiKey = apiKey },
                SystemMessage = new GitHub.Copilot.SDK.SystemMessageConfig { Mode = GitHub.Copilot.SDK.SystemMessageMode.Append, Content = systemMessage },
                Streaming = true
            });

            var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            session.On(evt =>
            {
                switch (evt)
                {
                    case GitHub.Copilot.SDK.AssistantMessageDeltaEvent delta:
                        Console.Write(delta.Data.DeltaContent);
                        break;
                    case GitHub.Copilot.SDK.AssistantMessageEvent msg:
                        if (!string.IsNullOrEmpty(msg.Data.Content))
                            lastContent = msg.Data.Content;
                        break;
                    case GitHub.Copilot.SDK.SessionIdleEvent:
                        done.TrySetResult();
                        break;
                    case GitHub.Copilot.SDK.SessionErrorEvent err:
                        AnsiConsole.MarkupLine("[red]Error:[/] {0}", err.Data?.Message ?? "Unknown");
                        done.TrySetResult();
                        break;
                }
            });

            var prompt = mode == CopilotFlowMode.Figma
                ? "Output only the JSON object with design parameters (layout, primary, accent, radius, fontSans, appName). No other text."
                : "Output only the JSON object with design parameters (layout, primary, accent, radius, fontSans, appName). No other text.";
            await session.SendAsync(new GitHub.Copilot.SDK.MessageOptions { Prompt = prompt });
            await done.Task;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[red]{0}[/]", ex.Message);
            return null;
        }

        return ParseDesignParamsFromResponse(lastContent);
    }

    private static DesignParams? ParseDesignParamsFromResponse(string? content)
    {
        if (string.IsNullOrWhiteSpace(content)) return null;
        var json = content;
        var jsonBlock = System.Text.RegularExpressions.Regex.Match(content, @"```(?:json)?\s*(\{[\s\S]*?\})\s*```");
        if (jsonBlock.Success)
            json = jsonBlock.Groups[1].Value;
        else
        {
            var first = content.IndexOf('{');
            var last = content.LastIndexOf('}');
            if (first >= 0 && last > first)
                json = content.Substring(first, last - first + 1);
        }
        try
        {
            var doc = System.Text.Json.JsonDocument.Parse(json);
            var r = doc.RootElement;
            var layout = r.TryGetProperty("layout", out var l) ? l.GetString()?.ToLowerInvariant() : null;
            if (layout != "sidebar" && layout != "topnav") layout = "sidebar";
            var siteUrl = r.TryGetProperty("siteUrl", out var su) ? su.GetString() : null;
            return new DesignParams(
                layout,
                r.TryGetProperty("primary", out var p) ? p.GetString() ?? DesignParams.Default.Primary : DesignParams.Default.Primary,
                r.TryGetProperty("accent", out var a) ? a.GetString() ?? DesignParams.Default.Accent : DesignParams.Default.Accent,
                r.TryGetProperty("radius", out var r2) ? r2.GetString() ?? DesignParams.Default.Radius : DesignParams.Default.Radius,
                r.TryGetProperty("fontSans", out var f) ? f.GetString() : null,
                r.TryGetProperty("appName", out var n) ? n.GetString() ?? DesignParams.Default.AppName : DesignParams.Default.AppName,
                siteUrl
            );
        }
        catch
        {
            return null;
        }
    }

    private static string BuildWebsiteParamsOnlySystemMessage(string userPrompt) => $@"You are a design analyst. The user wants a React UI that looks as much as possible like the site they described: ""{userPrompt}"".

Do NOT run any commands. Do NOT use any tools. Only analyze the request and respond with a single JSON object and nothing else.

IMPORTANT: If the user mentioned a specific site (e.g. apple.com.tr, stripe.com, https://example.com), you MUST include that exact URL in your response as ""siteUrl"" so we can fetch its favicon and use it as the app logo. Infer the full URL (https://...) when the user only gave a domain.

Your response must be ONLY valid JSON with these keys (no markdown, no code block label):
  layout (string: ""sidebar"" or ""topnav"" – choose what best matches the site),
  primary (string: HSL e.g. ""220 70% 50%"" – match the site's main brand color),
  accent (string: HSL e.g. ""220 15% 92%"" – match secondary/background tones),
  radius (string: e.g. ""0.5rem""),
  fontSans (string or null – match the site's font family if recognizable),
  appName (string – e.g. the site's name or a short title),
  siteUrl (string or null – the full URL of the site, e.g. ""https://apple.com.tr"", when the user referred to a specific site)

Make the design parameters match the referenced site as closely as possible. Example: {{""layout"":""topnav"",""primary"":""0 0% 9%"",""accent"":""0 0% 96%"",""radius"":""0.5rem"",""fontSans"":""SF Pro Display, sans-serif"",""appName"":""Apple"",""siteUrl"":""https://apple.com.tr""}}";

    private static string BuildFigmaParamsOnlySystemMessage(string figmaUrl) => $@"You are a design analyst. The user wants a React UI design based on this Figma project: {figmaUrl}

Do NOT run any commands. Do NOT use any tools. Only infer design direction from the project name/URL and respond with a single JSON object and nothing else.

Your response must be ONLY valid JSON with these exact keys (no markdown, no code block label):
  layout (string: ""sidebar"" or ""topnav""),
  primary (string: HSL e.g. ""220 70% 50%""),
  accent (string: HSL e.g. ""220 15% 92%""),
  radius (string: e.g. ""0.5rem""),
  fontSans (string or null),
  appName (string)

Example: {{""layout"":""topnav"",""primary"":""220 70% 50%"",""accent"":""220 15% 92%"",""radius"":""0.5rem"",""fontSans"":null,""appName"":""My App""}}";

    /// <summary>
    /// Runs init then transform with visible step-by-step progress and prints final project path.
    /// When siteUrlForFavicon is set (website flow), downloads the site's favicon and uses it as the app logo.
    /// </summary>
    private static int RunInitAndTransformWithProgress(string projectName, string parentPath, string? backendPath, DesignParams designParams, string? siteUrlForFavicon = null)
    {
        var targetDir = Path.Combine(parentPath, projectName);
        var fullPath = Path.GetFullPath(targetDir);

        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("[bold]Step 1/2:[/] Creating project folder and copying template...");
        var initSettings = new InitSettings
        {
            TargetDir = targetDir,
            BackendPath = backendPath
        };
        if (InitRunner.Run(initSettings) != 0)
            return 1;

        string? logoRelativePath = null;
        if (!string.IsNullOrWhiteSpace(siteUrlForFavicon))
        {
            AnsiConsole.MarkupLine("[dim]Fetching site favicon for logo...[/]");
            var publicDir = Path.Combine(fullPath, "public");
            var logoPath = FaviconFetcher.DownloadFaviconAsync(siteUrlForFavicon, publicDir).GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(logoPath))
            {
                logoRelativePath = "/" + Path.GetFileName(logoPath);
                AnsiConsole.MarkupLine("[green]Using site favicon as logo.[/]");
            }
        }

        AnsiConsole.MarkupLine("[bold]Step 2/2:[/] Applying layout and branding...");
        var brand = new BrandTokens
        {
            Primary = designParams.Primary,
            Accent = designParams.Accent,
            Radius = designParams.Radius,
            FontSans = designParams.FontSans,
            AppName = designParams.AppName,
            LogoPath = logoRelativePath
        };
        try
        {
            var orchestrator = new TransformOrchestrator();
            orchestrator.Apply(fullPath, designParams.Layout, brand);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }

        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("[green]Done.[/] Project created at: [bold]{0}[/]", fullPath);
        AnsiConsole.MarkupLine("Run: [yellow]cd \"{0}\"[/] then [yellow]npm install && npm run dev[/]", fullPath);
        return 0;
    }

    private static int RunCustomDesign()
    {
        AnsiConsole.MarkupLine("[bold]Choose your custom design[/] – we'll ask each option.");
        AnsiConsole.WriteLine();

        var targetDir = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Target folder name[/] (e.g. my-app):")
                .DefaultValue("my-app")
                .AllowEmpty());
        if (string.IsNullOrWhiteSpace(targetDir))
            targetDir = "my-app";

        var layout = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Layout:")
                .AddChoices("sidebar", "topnav"));

        var appName = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]App name[/] (for branding):")
                .DefaultValue("My App")
                .AllowEmpty());
        if (string.IsNullOrWhiteSpace(appName))
            appName = "My App";

        var primary = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Primary color[/] (HSL e.g. 220 70% 50%):")
                .DefaultValue("220 70% 50%")
                .AllowEmpty());
        if (string.IsNullOrWhiteSpace(primary))
            primary = "220 70% 50%";

        var accent = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Accent color[/] (HSL e.g. 220 15% 92%):")
                .DefaultValue("220 15% 92%")
                .AllowEmpty());
        if (string.IsNullOrWhiteSpace(accent))
            accent = "220 15% 92%";

        var radius = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Border radius[/] (e.g. 0.5rem):")
                .DefaultValue("0.5rem")
                .AllowEmpty());
        if (string.IsNullOrWhiteSpace(radius))
            radius = "0.5rem";

        var fontSans = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Sans font[/] (leave empty for default):")
                .AllowEmpty());

        var logoPath = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Logo path[/] (optional, leave empty to skip):")
                .AllowEmpty());

        var backendPath = AnsiConsole.Prompt(
            new TextPrompt<string>("[yellow]Backend path[/] (optional, for CORS/API URL):")
                .AllowEmpty());

        var initSettings = new InitSettings
        {
            TargetDir = targetDir,
            BackendPath = string.IsNullOrWhiteSpace(backendPath) ? null : backendPath
        };

        if (InitRunner.Run(initSettings) != 0)
            return 1;

        var projectRoot = Path.IsPathRooted(targetDir) ? targetDir : Path.Combine(Environment.CurrentDirectory, targetDir);
        var brand = new BrandTokens
        {
            Primary = primary,
            Accent = accent,
            Radius = radius,
            FontSans = string.IsNullOrWhiteSpace(fontSans) ? null : fontSans,
            AppName = appName,
            LogoPath = string.IsNullOrWhiteSpace(logoPath) ? null : logoPath
        };

        try
        {
            var orchestrator = new TransformOrchestrator();
            orchestrator.Apply(projectRoot, layout, brand);
            AnsiConsole.MarkupLine("[green]Transformed[/] layout=[bold]{0}[/], branding updated.", layout);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }

        AnsiConsole.MarkupLine("[green]Done.[/] Run [yellow]cd {0}[/] then [yellow]npm install && npm run dev[/].", targetDir);
        return 0;
    }
}
