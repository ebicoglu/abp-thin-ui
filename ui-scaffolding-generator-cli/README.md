# ABP Thin UI CLI (abp-ui)

Template transformer CLI for the ABP Thin UI base template (Vite + React). Built with C# and Spectre.Console.Cli.

## Commands

- **init** – Scaffold the base UI template into a folder.
- **transform** – Apply layout and branding to a project created with `init`.

## Usage

From the repository root (so the default template path `ui-base-templates/react` exists):

```bash
# Create a new app from the base template
dotnet run --project ui-scaffolding-generator-cli -- init new-app

# Change layout and branding (run from the new project folder)
cd new-app
dotnet run --project ../ui-scaffolding-generator-cli -- transform --layout=topnav --primary=#22c55e
```

Or from any directory, specify paths:

```bash
dotnet run --project ui-scaffolding-generator-cli -- init ./my-app --template-path /path/to/ui-base-templates/react
dotnet run --project ui-scaffolding-generator-cli -- transform --project ./my-app --layout=topnav
```

## Transform options

| Option        | Description                    |
|---------------|--------------------------------|
| `--layout`    | `sidebar` (default) or `topnav` |
| `--primary`   | Primary brand color             |
| `--accent`    | Accent color                   |
| `--radius`    | Border radius (e.g. 0.5rem)    |
| `--font-sans` | Sans-serif font family         |
| `--app-name`  | Application display name       |
| `--logo`      | Path to logo image             |

## Publish

```bash
dotnet publish -c Release -o ./publish
# Run: ./publish/abp-ui.exe init my-app
```
