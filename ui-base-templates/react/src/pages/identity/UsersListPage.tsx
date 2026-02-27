import { useQuery } from "@tanstack/react-query";
import { Plus } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { fetcher, AppError } from "@/lib/http/fetcher";
import { hasApiConfigured } from "@/lib/config/env";

export type UserListItem = {
  id: string;
  userName: string;
  email?: string;
  name?: string;
};

/** Mock data when API is not configured or request fails */
const MOCK_USERS: UserListItem[] = [
  { id: "1", userName: "admin", email: "admin@example.com", name: "Admin" },
  { id: "2", userName: "john", email: "john@example.com", name: "John Doe" },
];

async function fetchUsers(): Promise<UserListItem[]> {
  if (!hasApiConfigured()) return MOCK_USERS;
  try {
    const data = await fetcher<{ items?: UserListItem[] }>("/api/identity/users");
    return data?.items ?? MOCK_USERS;
  } catch (_e) {
    return MOCK_USERS;
  }
}

const UsersListPage = () => {
  const { data: users = MOCK_USERS, isLoading, error } = useQuery({
    queryKey: ["identity", "users"],
    queryFn: fetchUsers,
  });

  return (
    <div>
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold tracking-tight text-foreground">Users</h1>
        <Button size="sm">
          <Plus className="mr-2 h-4 w-4" />
          Add user
        </Button>
      </div>
      <p className="mt-1 text-sm text-muted-foreground">
        Identity module – users from ABP API or mock data when not configured.
      </p>
      <div className="mt-6">
        {error && (
          <p className="text-sm text-destructive">
            {error instanceof AppError ? error.message : String(error)}
          </p>
        )}
        {isLoading ? (
          <p className="text-sm text-muted-foreground">Loading…</p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>User name</TableHead>
                <TableHead>Email</TableHead>
                <TableHead>Name</TableHead>
                <TableHead className="w-[80px]">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {users.map((u) => (
                <TableRow key={u.id}>
                  <TableCell className="font-medium">{u.userName}</TableCell>
                  <TableCell>{u.email ?? "—"}</TableCell>
                  <TableCell>{u.name ?? "—"}</TableCell>
                  <TableCell>
                    <Button variant="ghost" size="sm">
                      Edit
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </div>
    </div>
  );
};

export default UsersListPage;
