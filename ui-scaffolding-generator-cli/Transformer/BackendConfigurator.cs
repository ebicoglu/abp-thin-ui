using System.Text.Json;
using System.Text.Json.Nodes;

namespace AbpUiCli.Transformer;

/// <summary>
/// Configures the ABP backend so the generated React app can connect (CORS, RedirectAllowedUrls).
/// </summary>
public sealed class BackendConfigurator
{
    private const string DevOrigins = "http://localhost:8080,http://localhost:8081,http://localhost:5173,http://127.0.0.1:8080,http://127.0.0.1:8081,http://127.0.0.1:5173";

    /// <summary>
    /// Resolve the directory that contains appsettings.json (HttpApi.Host project).
    /// If path is solution root, search for *HttpApi.Host* folder.
    /// </summary>
    public static string? ResolveHostDirectory(string backendPath)
    {
        var dir = Path.GetFullPath(backendPath);
        if (!Directory.Exists(dir))
            return null;

        var appsettings = Path.Combine(dir, "appsettings.json");
        if (File.Exists(appsettings))
            return dir;

        foreach (var sub in Directory.EnumerateDirectories(dir, "*HttpApi.Host*", SearchOption.AllDirectories))
        {
            if (File.Exists(Path.Combine(sub, "appsettings.json")))
                return sub;
        }

        return null;
    }

    /// <summary>
    /// Read API base URL from backend appsettings.json (App:SelfUrl or AuthServer:Authority).
    /// </summary>
    public static string? GetApiUrl(string hostDir)
    {
        var path = Path.Combine(hostDir, "appsettings.json");
        if (!File.Exists(path))
            return null;

        try
        {
            var json = JsonSerializer.Deserialize<JsonElement>(File.ReadAllText(path));
            var url = TryGetString(json, "App", "SelfUrl")
                ?? TryGetString(json, "AuthServer", "Authority");
            return url?.TrimEnd('/');
        }
        catch
        {
            return null;
        }
    }

    private static string? TryGetString(JsonElement root, string section, string key)
    {
        if (root.TryGetProperty(section, out var sectionEl) && sectionEl.TryGetProperty(key, out var value))
            return value.GetString();
        return null;
    }

    /// <summary>
    /// Add or update App:CorsOrigins and App:RedirectAllowedUrls in appsettings.Development.json.
    /// </summary>
    public static void AddCorsForReactApp(string hostDir)
    {
        var path = Path.Combine(hostDir, "appsettings.Development.json");
        JsonElement root;
        try
        {
            var text = File.Exists(path) ? File.ReadAllText(path) : "{}";
            root = JsonSerializer.Deserialize<JsonElement>(text);
        }
        catch
        {
            root = JsonSerializer.Deserialize<JsonElement>("{}");
        }

        var node = JsonNode.Parse(File.Exists(path) ? File.ReadAllText(path) : "{}");
        var obj = node?.AsObject() ?? new JsonObject();
        var app = obj["App"]?.AsObject() ?? new JsonObject();
        app["CorsOrigins"] = DevOrigins;
        app["RedirectAllowedUrls"] = DevOrigins;
        obj["App"] = app;

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, obj.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }
}
