import { Home, Settings, Users, Shield, LayoutDashboard, BookOpen, UserCog } from "lucide-react";
import { NavLink, useLocation } from "react-router-dom";
import { cn } from "@/lib/utils";
import { useBrand } from "@/lib/useBrand";

const navItems = [
  { label: "Dashboard", icon: LayoutDashboard, to: "/" },
  { label: "Users", icon: Users, to: "/identity" },
  { label: "Roles", icon: UserCog, to: "/identity/roles" },
  { label: "Books", icon: BookOpen, to: "/books" },
  { label: "Administration", icon: Shield, to: "/administration" },
  { label: "Settings", icon: Settings, to: "/settings" },
];

const AppSidebar = () => {
  const location = useLocation();
  const { appName, logo } = useBrand();

  return (
    <aside className="flex h-screen w-60 flex-col bg-sidebar text-sidebar-foreground border-r border-sidebar-border">
      <div className="flex items-center gap-2 px-5 py-5">
        {logo ? (
          <img src={logo} alt="" className="h-8 w-8 object-contain" />
        ) : (
          <div className="flex h-8 w-8 items-center justify-center rounded-md bg-sidebar-primary">
            <Home className="h-4 w-4 text-sidebar-primary-foreground" />
          </div>
        )}
        <span className="text-lg font-semibold tracking-tight" style={{ fontFamily: 'var(--font-display)' }}>
          {appName}
        </span>
      </div>

      <nav className="flex-1 space-y-1 px-3 py-4">
        {navItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={cn(
              "flex items-center gap-3 rounded-md px-3 py-2.5 text-sm font-medium transition-colors",
              location.pathname === item.to
                ? "bg-sidebar-accent text-sidebar-primary"
                : "text-sidebar-foreground/70 hover:bg-sidebar-accent hover:text-sidebar-foreground"
            )}
          >
            <item.icon className="h-4 w-4" />
            {item.label}
          </NavLink>
        ))}
      </nav>

      <div className="border-t border-sidebar-border px-4 py-4">
        <p className="text-xs text-sidebar-foreground/40">ABP Framework</p>
      </div>
    </aside>
  );
};

export default AppSidebar;
