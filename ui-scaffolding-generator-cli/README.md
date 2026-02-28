# ABP Thin UI CLI (abp-ui)

Template transformer CLI for the ABP Thin UI base template (Vite + React). Built with C# and Spectre.Console.Cli.

## Commands

- **init** – Scaffold the base UI template into a folder.
- **transform** – Apply layout and branding to a project created with `init`.

## Usage

From the repository root (so the default template path `ui-base-templates/react` exists):

```bash
# Create a new app and connect it to your ABP backend (recommended)
dotnet run --project ui-scaffolding-generator-cli -- init MyFirstAbpReactApp --backend-path backend/MultiLayerAbp

# The CLI will: copy the template, create .env with the backend API URL, and update the backend CORS for the React dev server.
```

Init options:

| Option | Description |
|--------|-------------|
| `--backend-path <path>` | Path to your ABP backend (solution folder or HttpApi.Host project). The CLI reads the API URL from appsettings.json and updates appsettings.Development.json with CORS for the React app. |
| `--api-url <url>` | Override API URL in .env (default: https://localhost:44317, or from backend if --backend-path is set). |
| `--template-path <path>` | Base template folder (default: ui-base-templates/react relative to current directory). |

```bash
# Create app only (no backend connection)
dotnet run --project ui-scaffolding-generator-cli -- init new-app

# Change layout or branding (run from the new project folder)
cd new-app
dotnet run --project ../ui-scaffolding-generator-cli -- transform --layout=topnav --primary=#22c55e
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
