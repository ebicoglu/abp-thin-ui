using System.Text.RegularExpressions;

namespace AbpUiCli;

/// <summary>
/// Tries to download a site's favicon for use as the app logo.
/// </summary>
public static class FaviconFetcher
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10),
        DefaultRequestHeaders = { { "User-Agent", "ABP-Factory/1.0" } }
    };

    /// <summary>
    /// Tries to extract a plausible URL from a user description (e.g. "apple.com.tr gibi" -> https://apple.com.tr).
    /// </summary>
    public static string? TryExtractUrlFromDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description)) return null;
        // Match domain-like patterns: domain.com, domain.com.tr, www.domain.com, or https://...
        var m = Regex.Match(description, @"(https?://[^\s""']+)|(?:^|[\s,])((?:www\.)?[a-zA-Z0-9][-a-zA-Z0-9]*(?:\.[a-zA-Z]{2,})+(?:/[^\s""']*)?)", RegexOptions.IgnoreCase);
        if (m.Success)
        {
            var url = m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
            url = url.Trim();
            if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                url = "https://" + url;
            return url;
        }
        return null;
    }

    /// <summary>
    /// Downloads the favicon from the given site URL. Tries /favicon.ico then parses HTML for link rel="icon".
    /// Returns the path to the saved file, or null if failed.
    /// </summary>
    public static async Task<string?> DownloadFaviconAsync(string siteUrl, string saveToDirectory)
    {
        try
        {
            var uri = new Uri(siteUrl);
            var baseUrl = uri.Scheme + "://" + uri.Host;

            // 1. Try common favicon path
            var faviconUrl = baseUrl.TrimEnd('/') + "/favicon.ico";
            var bytes = await DownloadBytesAsync(faviconUrl).ConfigureAwait(false);
            if (bytes != null && bytes.Length > 0)
                return await SaveFaviconAsync(bytes, saveToDirectory, ".ico").ConfigureAwait(false);

            // 2. Fetch HTML and look for <link rel="icon" href="...">
            var html = await HttpClient.GetStringAsync(siteUrl).ConfigureAwait(false);
            var linkMatch = Regex.Match(html, @"<link[^>]+rel\s*=\s*[""']?(?:shortcut\s+)?icon[""']?[^>]+href\s*=\s*[""']([^""']+)[""']", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!linkMatch.Success)
                linkMatch = Regex.Match(html, @"<link[^>]+href\s*=\s*[""']([^""']+)[""'][^>]+rel\s*=\s*[""']?(?:shortcut\s+)?icon[""']?", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (linkMatch.Success)
            {
                var href = linkMatch.Groups[1].Value.Trim();
                if (!href.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    href = new Uri(new Uri(baseUrl), href).ToString();
                bytes = await DownloadBytesAsync(href).ConfigureAwait(false);
                if (bytes != null && bytes.Length > 0)
                {
                    var ext = Path.GetExtension(new Uri(href).LocalPath);
                    if (string.IsNullOrEmpty(ext) || ext.Length > 4) ext = ".png";
                    return await SaveFaviconAsync(bytes, saveToDirectory, ext).ConfigureAwait(false);
                }
            }

            // 3. Try /favicon.png
            bytes = await DownloadBytesAsync(baseUrl.TrimEnd('/') + "/favicon.png").ConfigureAwait(false);
            if (bytes != null && bytes.Length > 0)
                return await SaveFaviconAsync(bytes, saveToDirectory, ".png").ConfigureAwait(false);
        }
        catch
        {
            // ignore
        }

        return null;
    }

    private static async Task<byte[]?> DownloadBytesAsync(string url)
    {
        try
        {
            var response = await HttpClient.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
    }

    private static Task<string> SaveFaviconAsync(byte[] bytes, string directory, string extension)
    {
        Directory.CreateDirectory(directory);
        var fileName = "logo" + extension;
        var path = Path.Combine(directory, fileName);
        return File.WriteAllBytesAsync(path, bytes).ContinueWith(_ => path, TaskScheduler.Default);
    }
}
