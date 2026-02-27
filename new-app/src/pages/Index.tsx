const Index = () => {
  return (
    <div>
      <h1 className="text-2xl font-semibold tracking-tight text-foreground">Dashboard</h1>
      <p className="mt-1 text-sm text-muted-foreground">Welcome to your application.</p>

      <div className="mt-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {["Tenants", "Users", "Roles", "Audit Logs"].map((title) => (
          <div key={title} className="rounded-lg border border-border bg-card p-5">
            <p className="text-xs font-medium uppercase tracking-wider text-muted-foreground">{title}</p>
            <p className="mt-2 text-2xl font-semibold text-foreground" style={{ fontFamily: 'var(--font-display)' }}>—</p>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Index;
