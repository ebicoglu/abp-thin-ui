namespace AbpUiCli.Transformer;

public sealed class TransformOrchestrator
{
    private readonly LayoutTransformer _layoutTransformer = new();
    private readonly BrandingTransformer _brandingTransformer = new();

    public void Apply(string projectRoot, string layout, BrandTokens brand)
    {
        _layoutTransformer.Apply(projectRoot, layout);
        _brandingTransformer.Apply(projectRoot, brand);

        var appliedPath = Path.Combine(projectRoot, "template.meta", "applied.json");
        Directory.CreateDirectory(Path.GetDirectoryName(appliedPath)!);

        var applied = new AppliedManifest
        {
            Layout = layout,
            Brand = new AppliedBrand
            {
                Primary = brand.Primary,
                Accent = brand.Accent,
                Radius = brand.Radius,
                FontSans = brand.FontSans,
                AppName = brand.AppName
            },
            AppliedAt = DateTime.UtcNow.ToString("O")
        };

        var json = System.Text.Json.JsonSerializer.Serialize(applied, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(appliedPath, json);
    }

    private sealed class AppliedManifest
    {
        public string Layout { get; set; } = "";
        public AppliedBrand Brand { get; set; } = new();
        public string AppliedAt { get; set; } = "";
    }

    private sealed class AppliedBrand
    {
        public string Primary { get; set; } = "";
        public string Accent { get; set; } = "";
        public string Radius { get; set; } = "";
        public string? FontSans { get; set; }
        public string? AppName { get; set; }
    }
}
