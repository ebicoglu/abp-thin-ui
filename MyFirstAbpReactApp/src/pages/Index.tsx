import { useQuery } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { Users, Shield, BookOpen } from "lucide-react";
import { fetcher } from "@/lib/http/fetcher";
import { getPagedTotalCount } from "@/lib/http/paged";
import { hasApiConfigured } from "@/lib/config/env";
import { Card, CardContent, CardHeader } from "@/components/ui/card";

type Paged = { items?: unknown[]; totalCount?: number; Items?: unknown[]; TotalCount?: number };

async function fetchCount(path: string): Promise<number> {
  const data = await fetcher<Paged>(`${path}?MaxResultCount=1&SkipCount=0`);
  return getPagedTotalCount(data);
}

const Dashboard = () => {
  const { data: usersCount } = useQuery({
    queryKey: ["identity", "users", "count"],
    queryFn: () => fetchCount("/api/identity/users"),
    enabled: hasApiConfigured(),
  });
  const { data: rolesCount } = useQuery({
    queryKey: ["identity", "roles", "count"],
    queryFn: () => fetchCount("/api/identity/roles"),
    enabled: hasApiConfigured(),
  });
  const { data: booksCount } = useQuery({
    queryKey: ["books", "count"],
    queryFn: () => fetchCount("/api/app/book"),
    enabled: hasApiConfigured(),
  });

  const cards = [
    { title: "Users", value: hasApiConfigured() ? (usersCount ?? "…") : "—", href: "/identity", icon: Users },
    { title: "Roles", value: hasApiConfigured() ? (rolesCount ?? "…") : "—", href: "/identity/roles", icon: Shield },
    { title: "Books", value: hasApiConfigured() ? (booksCount ?? "…") : "—", href: "/books", icon: BookOpen },
  ];

  return (
    <div>
      <h1 className="text-2xl font-semibold tracking-tight text-foreground">Dashboard</h1>
      <p className="mt-1 text-sm text-muted-foreground">Overview of your application. Click a card to manage.</p>
      <div className="mt-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {cards.map(({ title, value, href, icon: Icon }) => (
          <Link key={title} to={href}>
            <Card className="transition-colors hover:bg-muted/50">
              <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                <span className="text-sm font-medium text-muted-foreground">{title}</span>
                <Icon className="h-4 w-4 text-muted-foreground" />
              </CardHeader>
              <CardContent>
                <p className="text-2xl font-semibold text-foreground" style={{ fontFamily: "var(--font-display)" }}>
                  {value}
                </p>
              </CardContent>
            </Card>
          </Link>
        ))}
      </div>
    </div>
  );
};

export default Dashboard;
