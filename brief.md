# Thin & AI-Ready UI

With Next.js POC with a CLI “template transformer” that can brand + assemble layouts while staying a clean consumer of ABP APIs.

What I want:

> # Role & Context
>
> I am the owner of the ABP Framework. We are pivoting our UI strategy from a heavy, pre-built theme (LeptonX) to a "Thin & AI-Ready UI" approach, similar to modern platforms like Bolt, Lovable, Replit. The goal is to provide developers with a "vanilla" but high-quality starter that they can radically customize.
>
> 
>
> # The Vision: The "Thin UI" Philosophy
>
> Instead of a monolithic UI, we will provide a base UI layer that acts as a headless-ready consumer of ABP Framework APIs. This layer must be extremely clean and modular so that AI coding assistants can easily understand and modify it.
>
> # Technical Stack (The "Modern SaaS" Stack)
>
> The POC will be built using:
>
> - Framework: Next.js (App Router)
> - Styling: Tailwind CSS
> - Components: shadcn/ui (Radix UI based)
> - Icons: Lucide Icons
> - Forms/Validation: React Hook Form + Zod
>
>
> # The Customization Mechanism: CLI Scaffolding
>
> We will implement a "Template Transformer" logic via a CLI Tool (similar to Hygen or Plop.js). 
>
> - The tool will take a "Base UI Template."
> - It will perform build-time transformation (scaffolding) based on user inputs (Logo, Brand Colors via CSS variables, Font styles, etc.).
> - Layout Flexibility: The UI will be built with atomic components (Sidebar, Navbar, MobileNav). The transformer will structurally assemble these based on the user's layout preference (e.g., "Sidebar Layout" vs "Top Menu Layout").
>
> # ABP Module Integration Strategy
>
> Each ABP module (Identity, SaaS, etc.) will have its own "Base UI Template." 
>
> - These templates will be transformed using the same CLI logic.
> - We will use pre-defined prompt-friendly structures. Example: For the "Organization Tree" in the SaaS module, the template will define a standard Tree component structure that the generator can easily customize or swap.
>
> # The Task
>
> As an expert software architect, please provide a detailed Technical Implementation Plan for this React (Next.js) POC:
>
> 1. Architecture: Define the folder structure of the "Base UI Template" to make it "AI-friendly" and modular.
> 2. Transformation Logic: Design the CLI workflow. How should it handle CSS Variable injection and structural Layout switching?
> 3. ABP Bridge: How should the UI handle authentication (OpenIddict) and API communication while remaining "thin"?
> 4. Component Mapping: Create a strategy for mapping ABP's core module requirements (SaaS, Identity) into shadcn/ui components.
> 5. Provide a proof-of-concept code snippet for the "Layout Transformer" logic.

In the below section you can find the implementation details:

---

## 1) Architecture: AI-friendly folder structure (Base UI Template)

Design goals:

* **Predictable** paths and naming (AI assistants love convention).
* **Atomic layout primitives** (Sidebar, TopNav, MobileNav) composed by a single “shell”.
* **Feature modules** aligned with ABP modules (Identity, SaaS) but still “thin” (mostly UI + API calls).
* **Zero magic**: minimal metaprogramming, obvious exports, shallow call chains.

**Recommended structure (Next.js App Router):**

```
apps/
  web/                             # The Base UI Template (Next.js)
    app/
      (public)/
        layout.tsx                 # Public shell (marketing/auth screens)
        page.tsx
      (protected)/
        layout.tsx                 # Protected shell wraps chosen AppShell
        dashboard/page.tsx
        identity/...
        saas/...
      api/                         # (optional) Next route handlers for proxy/token refresh
        auth/...
    src/
      app-shell/                   # Layout system (transformer target)
        AppShell.tsx               # One entry; chooses layout primitives
        layouts/
          sidebar/
            SidebarShell.tsx
            Sidebar.tsx
            SidebarNav.tsx
          topnav/
            TopNavShell.tsx
            TopNav.tsx
            TopNavMenu.tsx
          shared/
            MobileNav.tsx
            Breadcrumbs.tsx
            PageHeader.tsx
      components/
        ui/                         # shadcn/ui (kept pristine)
        abp/                        # ABP-aware components (thin wrappers)
          DataTable.tsx
          EntityForm.tsx
          PermissionGuard.tsx
          ResourceSelect.tsx
      features/                     # Thin “screens” per module
        identity/
          pages/
            users/
              UsersListPage.tsx
              UserEditSheet.tsx
            roles/...
          api/
            identity.client.ts      # typed client for Identity endpoints
          models/
            identity.types.ts
        saas/
          pages/
            tenants/...
            editions/...
          api/
            saas.client.ts
          models/
            saas.types.ts
      lib/
        http/
          fetcher.ts                # base fetch + interceptors
          tokens.ts                 # token storage/refresh strategy
          errors.ts
        auth/
          oidc.ts                   # OpenIddict/OIDC client config
          session.ts                # server helpers (cookies/headers)
        config/
          env.ts                    # zod-validated env
          branding.ts               # css var contract + defaults
        utils/
          cn.ts
          routes.ts
      styles/
        globals.css                 # tailwind + css variables
        themes/
          default.css               # optional presets
          dark.css
      prompts/                      # prompt-friendly docs for AI assistants
        README.ai.md                # “how to modify safely”
        conventions.md
    template.meta/
      manifest.json                 # transformer inputs + file operations
      layout.map.json               # which files activate per layout
      brand.tokens.json             # css var schema, fonts, radii, etc.
```

**AI-friendly conventions**

* Every folder has a short `README.md` explaining purpose and safe-edit areas.
* “Thin” means: `features/*/api/*.client.ts` should be the only place that knows endpoint paths.
* Keep `components/ui` untouched (pure shadcn). Put ABP opinions in `components/abp`.

---

## 2) Transformation Logic: CLI workflow (branding + layout switching)

### CLI commands (minimal but extensible)

* `abp-ui init` → scaffold base template into a folder
* `abp-ui transform` → apply branding/layout/module selections to an existing template
* `abp-ui add module <identity|saas|...>` → copy module template and register nav/routes
* `abp-ui eject` (optional) → remove transformer markers; keep final code

### Inputs (prompt or flags)

* Branding:

  * `--logo` (path), `--appName`
  * `--primary`, `--accent`, `--radius`
  * `--fontSans`, `--fontMono` (or preset: “modern”, “elegant serif”, etc.)
* Layout:

  * `--layout sidebar|topnav`
  * `--navDensity cozy|compact`
* Modules:

  * `--modules identity,saas`

### How to do CSS Variable injection :

Keep a **single contract** in `src/lib/config/branding.ts` and `styles/globals.css`.

* `styles/globals.css` contains **placeholder tokens** (or an always-valid default).
* Transformer updates:

  1. `styles/globals.css` CSS variables under `:root` / `.dark`
  2. `src/lib/config/branding.ts` with derived values (e.g., font import names)
  3. optionally adds `app/icon.png` and replaces `public/logo.svg`

Example contract (conceptually):

* `--brand-primary`, `--brand-accent`, `--radius`, `--font-sans`, etc.
  This works well with Tailwind + shadcn conventions.

### Layout switching (structural assembly)

Avoid runtime “if layout === …” everywhere. Do it at **transform time**:

* `src/app-shell/AppShell.tsx` is a stable entry point.
* Transformer will:

  * copy the chosen shell implementation into `src/app-shell/ActiveShell.tsx`
  * update `AppShell.tsx` to export that shell
  * remove unused layout folders (optional) or keep them (dev friendly)

Also update:

* `src/lib/utils/routes.ts` (nav structure)
* `src/app/(protected)/layout.tsx` wraps `AppShell`

**This is AI-friendly:** after transform, codebase looks like a normal Next app with one chosen layout, not a matrix of conditionals.

---

## 3) ABP Bridge: auth (OpenIddict) + API communication while staying “thin”

### Principle: keep ABP knowledge in **two layers**

1. `lib/auth/*` and `lib/http/*` (generic infra)
2. `features/<module>/api/*.client.ts` (endpoint paths + DTOs)

### Authentication approach (pragmatic for POC)

Use **OIDC Authorization Code + PKCE** against OpenIddict.

Two viable implementation patterns:

**A) NextAuth (fastest POC)**

* Configure OIDC provider (OpenIddict) with PKCE.
* Store tokens in session (JWT/cookie) and expose `access_token` server-side for fetch.
* Pros: quick, robust sessions.
* Cons: extra abstraction; some teams prefer no NextAuth.

**B) Custom OIDC (thinnest)**

* Implement `/app/api/auth/login`, `/callback`, `/logout`
* Use encrypted httpOnly cookies for `access_token`, `refresh_token` (if allowed)
* Build `lib/http/fetcher.ts` that:

  * injects bearer token
  * if 401 and refresh token exists → refresh → retry
* Pros: minimal dependency, very explicit.
* Cons: more effort.

### API communication (clean + typed)

* Generate a lightweight client:

  * Option 1: OpenAPI generator (if ABP exposes swagger) into `src/lib/api/generated`
  * Option 2: manually typed clients per module (POC-friendly)
* Base fetcher:

  * supports `baseUrl` from env
  * adds `Authorization` header from server session/cookies
  * normalizes ABP error payloads into a simple `AppError`

**Thin UI rule:** no business logic in UI; only:

* call client
* render shadcn components
* handle optimistic UI + toasts

---

## 4) Component Mapping: ABP modules → shadcn/ui strategy

Create a small “ABP UI Kit” on top of shadcn: `src/components/abp/*`.
This is where you encode ABP patterns once, then reuse.

### Identity module mapping

* Users list:

  * `DataTable` (shadcn table + sorting + pagination)
  * `Input` for search, `DropdownMenu` for actions
  * `Dialog`/`Sheet` for create/edit user
* User form:

  * `React Hook Form + Zod`
  * `Form` primitives from shadcn
  * `Tabs` for “Profile / Roles / Permissions”
* Roles:

  * `DataTable` + `Dialog` for create/edit
* Permissions:

  * `Accordion` / `Tree` (Radix) + `Checkbox`
  * Wrap into `PermissionTree` component in `components/abp/`

### SaaS module mapping

* Tenants:

  * `DataTable` list
  * `Sheet` for create/edit tenant
  * `Badge` for status, `Switch` for activation
* Editions/Plans:

  * `Card` grid layout for plans
  * `Dialog` for editing features
* Organization Tree (your example):

  * Build `components/abp/Tree.tsx` (Radix-based pattern)
  * Keep the “node renderer” as a prop so generator/AI can swap visuals easily

### Cross-cutting ABP needs

* “Entity CRUD” baseline:

  * `EntityForm` (form scaffold)
  * `EntityTable` (table scaffold)
  * `EntityActions` (edit/delete/impersonate hooks)
* Multi-tenancy/impersonation indicator:

  * `TenantSwitcher` in app shell using `Popover + Command`
* Authorization in UI:

  * `PermissionGuard` component that hides/disables actions based on granted policies

---

## 5) POC code snippet: “Layout Transformer” (file ops + simple templating)

Below is a small Node script style snippet (works as the core of a CLI command). It:

* chooses a layout (“sidebar” vs “topnav”)
* writes `ActiveShell.tsx`
* updates branding CSS variables in `globals.css`

> For a real CLI, wrap this with `commander` or `yargs`, and use a proper templating lib if needed—but keep it boring.

```ts
// tools/transformer/layout-transformer.ts
import fs from "node:fs";
import path from "node:path";

type Layout = "sidebar" | "topnav";

type BrandTokens = {
  primary: string; // e.g. "#22c55e"
  accent: string;  // e.g. "#2dd4bf"
  radius: string;  // e.g. "0.75rem"
  fontSans?: string; // e.g. "Inter"
};

function read(filePath: string) {
  return fs.readFileSync(filePath, "utf8");
}

function write(filePath: string, content: string) {
  fs.mkdirSync(path.dirname(filePath), { recursive: true });
  fs.writeFileSync(filePath, content, "utf8");
}

function copyFile(src: string, dest: string) {
  fs.mkdirSync(path.dirname(dest), { recursive: true });
  fs.copyFileSync(src, dest);
}

function replaceBetween(content: string, startMarker: string, endMarker: string, replacement: string) {
  const start = content.indexOf(startMarker);
  const end = content.indexOf(endMarker);
  if (start === -1 || end === -1 || end < start) {
    throw new Error(`Markers not found or invalid: ${startMarker} ... ${endMarker}`);
  }
  return (
    content.slice(0, start + startMarker.length) +
    "\n" +
    replacement +
    "\n" +
    content.slice(end)
  );
}

export function transformLayoutAndBranding(opts: {
  projectRoot: string;
  layout: Layout;
  brand: BrandTokens;
}) {
  const { projectRoot, layout, brand } = opts;

  // 1) Layout selection: create ActiveShell.tsx from chosen layout shell
  const layoutsDir = path.join(projectRoot, "src", "app-shell", "layouts");
  const activeShellDest = path.join(projectRoot, "src", "app-shell", "ActiveShell.tsx");

  const shellSrc =
    layout === "sidebar"
      ? path.join(layoutsDir, "sidebar", "SidebarShell.tsx")
      : path.join(layoutsDir, "topnav", "TopNavShell.tsx");

  // Keep this explicit: ActiveShell is the single chosen entry for the shell.
  copyFile(shellSrc, activeShellDest);

  // 2) Ensure AppShell exports ActiveShell (AI-friendly single import point)
  const appShellPath = path.join(projectRoot, "src", "app-shell", "AppShell.tsx");
  const appShell = read(appShellPath);

  const nextAppShell = `
/**
 * AppShell is the stable entry. "ActiveShell" is generated by the transformer.
 * To switch layouts, re-run: abp-ui transform --layout=...
 */
export { default } from "./ActiveShell";
`.trimStart();

  // Overwrite AppShell.tsx entirely to avoid subtle merge issues.
  write(appShellPath, nextAppShell);

  // 3) Branding: inject CSS variables into styles/globals.css
  const globalsCssPath = path.join(projectRoot, "src", "styles", "globals.css");
  const globals = read(globalsCssPath);

  const tokenBlock = `
:root {
  --brand-primary: ${brand.primary};
  --brand-accent: ${brand.accent};
  --radius: ${brand.radius};
  ${brand.fontSans ? `--font-sans: ${brand.fontSans};` : ""}
}

.dark {
  /* keep defaults or compute dark variants in a smarter version */
}
`.trim();

  const updatedGlobals = replaceBetween(
    globals,
    "/* @abp-brand:start */",
    "/* @abp-brand:end */",
    tokenBlock
  );

  write(globalsCssPath, updatedGlobals);

  // 4) (Optional) write a small manifest of what was applied (helps support + AI)
  const appliedPath = path.join(projectRoot, "template.meta", "applied.json");
  write(
    appliedPath,
    JSON.stringify({ layout, brand, appliedAt: new Date().toISOString() }, null, 2)
  );

  console.log(`✅ Transformed layout="${layout}" and updated branding tokens.`);
}
```

And your `globals.css` just needs markers once:

```css
@tailwind base;
@tailwind components;
@tailwind utilities;

/* @abp-brand:start */
/* transformer will inject :root tokens here */
/* @abp-brand:end */
```

---

I want the POC to feel like Bolt/Lovable/Replit quickly: prioritize **(1) fast shell + nav**, **(2) auth + fetcher**, **(3) one CRUD screen per module**, and **(4) the transformer**.
