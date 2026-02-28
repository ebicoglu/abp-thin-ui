# Test Transform with ALL available parameters
# Run from repo root: .\run-transform-test.ps1
# Or run the command below from the project folder (e.g. MyFirstAbpReactApp) without --project.

$ErrorActionPreference = "Stop"
$projectPath = "MyFirstAbpReactApp"
$cliProject = "ui-scaffolding-generator-cli"

# All transform options:
#   --project <path>     Project folder (default: current directory)
#   --layout <sidebar|topnav>
#   --primary <color>   HSL e.g. "262 83% 58%"
#   --accent <color>    HSL e.g. "220 15% 92%"
#   --radius <value>   e.g. "0.75rem"
#   --font-sans <font>  e.g. "Inter" or "DM Sans"
#   --app-name <name>   Display name in shell
#   --logo <path>       Relative to project, e.g. public/placeholder.svg

dotnet run --project $cliProject -- transform `
  --project $projectPath `
  --layout topnav `
  --primary "262 83% 58%" `
  --accent "220 15% 92%" `
  --radius "0.75rem" `
  --font-sans "DM Sans" `
  --app-name "My App" `
  --logo "public/placeholder.svg"

Write-Host ""
Write-Host "Done. Check $projectPath/template.meta/applied.json and run the app to see layout and branding."
