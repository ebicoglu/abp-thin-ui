# Getting Started – Build Your First App

Use these steps to create and run your first app (e.g. **MyFirstAbpReactApp**) with the ABP Thin UI CLI.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) and npm

## Commands (from repo root)

**1. Go to the solution root**
```powershell
cd D:\github\alper\abp-thin-ui
```

**2. Create the app and connect it to your ABP backend**
```powershell
dotnet run --project ui-scaffolding-generator-cli -- init MyFirstAbpReactApp --backend-path backend/MultiLayerAbp
```
This creates the app, sets `.env` with the backend API URL, and configures the backend CORS so the React app can call the API. No manual configuration needed.

**3. Enter the new app folder**
```powershell
cd MyFirstAbpReactApp
```

**4. (Optional) Change layout or branding**
```powershell
dotnet run --project ..\ui-scaffolding-generator-cli -- transform --layout=topnav --primary=#22c55e
```
Skip this to keep the default sidebar layout.

**Transform – all parameters (run from repo root):**
```powershell
dotnet run --project ui-scaffolding-generator-cli -- transform `
  --project MyFirstAbpReactApp `
  --layout topnav `
  --primary "262 83% 58%" `
  --accent "220 15% 92%" `
  --radius "0.75rem" `
  --font-sans "DM Sans" `
  --app-name "My App" `
  --logo "public/placeholder.svg"
```
Or use the script: `.\run-transform-test.ps1`

**5. Install dependencies and start the dev server**
```powershell
npm install
npm run dev
```
Or run the helper script:
```powershell
.\run.ps1
```

**6. Open the app**  
In your browser go to: **http://localhost:8080**

---

## Short version (ready to connect to backend)

From the solution root:

```powershell
cd D:\github\alper\abp-thin-ui
dotnet run --project ui-scaffolding-generator-cli -- init MyFirstAbpReactApp --backend-path backend/MultiLayerAbp
cd MyFirstAbpReactApp
npm install
npm run dev
```

Start your ABP backend (e.g. run `MultiLayerAbp.HttpApi.Host`) so the API is available; the React app is already configured to use it.

---

## Backend connection (done by CLI when using `--backend-path`)

When you run `init` with `--backend-path backend/MultiLayerAbp`, the CLI:

1. Creates `.env` in the new app with `VITE_ABP_API_URL` from the backend’s appsettings (or default `https://localhost:44317`).
2. Updates the backend’s `appsettings.Development.json` with `App:CorsOrigins` and `App:RedirectAllowedUrls` for `http://localhost:8080` and `http://localhost:5173`.

If you create the app without `--backend-path`, copy `.env.example` to `.env` and set `VITE_ABP_API_URL`, and add CORS in the backend manually.

## Project structure

| Path | Description |
|------|-------------|
| `ui-base-templates/react` | Base UI template (Vite + React, shadcn/ui) |
| `ui-scaffolding-generator-cli` | CLI to scaffold and transform apps (C# / Spectre.Console.Cli) |
| `backend/MultiLayerAbp` | ABP backend; startup: `MultiLayerAbp.HttpApi.Host` |
