# ABP Factory

CLI for the ABP Thin UI base template (Vite + React). Built with C# and Spectre.Console.Cli. Supports an interactive AI-assisted flow via the [GitHub Copilot SDK](https://github.com/github/copilot-sdk) (BYOK: use your own OpenAI API key).

## Commands

- **init** – Scaffold the base UI template into a folder.
- **transform** – Apply layout and branding to a project created with `init`.
- **config** – Show or set configuration (e.g. OpenAI API key for AI-assisted design).

## Interactive mode (no arguments)

If you run `abp-factory` with no arguments, you get:

**"Let's build something. Tell me what'd you like to build today?"**

1. **Design based on an existing web site** – Describe the site you want (e.g. "apple.com.tr gibi bir site"). The CLI uses the Copilot SDK with your OpenAI API key to analyze your request, optionally fetch the URL to extract styles, and generate a similar React UI (runs `abp-factory init` and `abp-factory transform` with inferred parameters).
2. **Design from an existing Figma project** – Paste a Figma file link in the form `https://www.figma.com/file/XXXXXXXXXXXX/Project-Name`. The AI infers design direction from the project and runs `abp-factory init` and `abp-factory transform` with matching parameters.
3. **Choose your custom design** – You are prompted for all options (target folder, layout, logo, primary/accent colors, radius, font, app name, backend path); then the project is created and transformed.

For options 1 and 2 you must set an OpenAI API key (see **Config** below).

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

## Config (OpenAI API key for AI-assisted design)

To use **Design based on an existing web site**, set your OpenAI API key:

```bash
abp-factory config --set-openai-key
# You will be prompted to enter the key (sk-...). It is stored in your user config folder.
```

Or set one of these environment variables: `ABP_UI_OPENAI_API_KEY` or `OPENAI_API_KEY`.

Config file location: `%APPDATA%\abp-factory\settings.json` (Windows) or `~/.config/abp-factory/settings.json` (Linux/macOS).

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
# Run: ./publish/abp-factory.exe init my-app
```
