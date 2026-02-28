namespace AbpUiCli.Config;

/// <summary>
/// Application configuration (API keys, etc.). Read from environment variables
/// and/or config file. Config file path: %APPDATA%\abp-factory\settings.json (Windows)
/// or ~/.config/abp-factory/settings.json (Linux/macOS).
/// </summary>
public sealed class AbpUiConfig
{
    private const string ConfigFileName = "settings.json";
    private static readonly string ConfigDir = GetConfigDirectory();
    private static readonly string ConfigPath = Path.Combine(ConfigDir, ConfigFileName);

    public string? OpenAIApiKey { get; set; }

    /// <summary>
    /// Resolves OpenAI API key: env ABP_UI_OPENAI_API_KEY or OPENAI_API_KEY, then config file.
    /// </summary>
    public static string? GetOpenAIApiKey()
    {
        var fromEnv = Environment.GetEnvironmentVariable("ABP_UI_OPENAI_API_KEY")
            ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (!string.IsNullOrWhiteSpace(fromEnv))
            return fromEnv.Trim();

        var config = Load();
        return config?.OpenAIApiKey;
    }

    /// <summary>
    /// Saves the OpenAI API key to the config file (and creates directory if needed).
    /// </summary>
    public static void SetOpenAIApiKey(string apiKey)
    {
        var config = Load() ?? new AbpUiConfig();
        config.OpenAIApiKey = apiKey?.Trim();
        Save(config);
    }

    public static AbpUiConfig? Load()
    {
        if (!File.Exists(ConfigPath))
            return null;

        try
        {
            var json = File.ReadAllText(ConfigPath);
            return System.Text.Json.JsonSerializer.Deserialize<AbpUiConfig>(json);
        }
        catch
        {
            return null;
        }
    }

    public static void Save(AbpUiConfig config)
    {
        Directory.CreateDirectory(ConfigDir);
        var json = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }

    public static string GetConfigFilePath() => ConfigPath;

    private static string GetConfigDirectory()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (!string.IsNullOrEmpty(appData))
            return Path.Combine(appData, "abp-factory");

        var home = Environment.GetEnvironmentVariable("HOME") ?? Environment.GetEnvironmentVariable("USERPROFILE") ?? ".";
        return Path.Combine(home, ".config", "abp-factory");
    }
}
