namespace AbpUiCli.Transformer;

public sealed class BrandingTransformer
{
    private const string StartMarker = "/* @abp-brand:start */";
    private const string EndMarker = "/* @abp-brand:end */";

    public void Apply(string projectRoot, BrandTokens brand)
    {
        var indexPath = Path.Combine(projectRoot, "src", "index.css");
        if (!File.Exists(indexPath))
            return;

        var content = File.ReadAllText(indexPath);
        var startIdx = content.IndexOf(StartMarker, StringComparison.Ordinal);
        var endIdx = content.IndexOf(EndMarker, StringComparison.Ordinal);

        if (startIdx == -1 || endIdx == -1 || endIdx <= startIdx)
            return;

        var tokenBlock = BuildTokenBlock(brand);
        var indent = "    ";
        var newContent = content[..startIdx]
            + StartMarker
            + "\n" + indent + tokenBlock.Replace("\n", "\n" + indent)
            + "\n" + indent + EndMarker
            + content[(endIdx + EndMarker.Length)..];

        File.WriteAllText(indexPath, newContent);
    }

    private static string BuildTokenBlock(BrandTokens brand)
    {
        var lines = new List<string>
        {
            $"--brand-primary: {brand.Primary};",
            $"--brand-accent: {brand.Accent};",
            $"--radius: {brand.Radius};"
        };
        if (!string.IsNullOrWhiteSpace(brand.FontSans))
            lines.Add($"--font-sans: {brand.FontSans};");
        return string.Join("\n", lines);
    }
}
