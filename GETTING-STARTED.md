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

**2. Create the app from the base template**
```powershell
dotnet run --project ui-scaffolding-generator-cli -- init MyFirstAbpReactApp
```

**3. Enter the new app folder**
```powershell
cd MyFirstAbpReactApp
```

**4. (Optional) Change layout or branding**
```powershell
dotnet run --project ..\ui-scaffolding-generator-cli -- transform --layout=topnav --primary=#22c55e
```
Skip this to keep the default sidebar layout.

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

## Short version (default layout)

From the solution root:

```powershell
cd D:\github\alper\abp-thin-ui
dotnet run --project ui-scaffolding-generator-cli -- init MyFirstAbpReactApp
cd MyFirstAbpReactApp
npm install
npm run dev
```

---

## Connect to ABP backend

1. Copy `.env.example` to `.env` in your app folder.
2. Set `VITE_ABP_API_URL` to your HttpApi.Host URL (e.g. `https://localhost:44317`).

## Project structure

| Path | Description |
|------|-------------|
| `ui-base-templates/react` | Base UI template (Vite + React, shadcn/ui) |
| `ui-scaffolding-generator-cli` | CLI to scaffold and transform apps (C# / Spectre.Console.Cli) |
| `backend/MultiLayerAbp` | ABP backend; startup: `MultiLayerAbp.HttpApi.Host` |
