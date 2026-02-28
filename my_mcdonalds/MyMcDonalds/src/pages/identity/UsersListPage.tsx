import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Plus, Pencil, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { fetcher, AppError } from "@/lib/http/fetcher";
import { getPagedItems } from "@/lib/http/paged";
import { hasApiConfigured } from "@/lib/config/env";
import { toast } from "@/components/ui/use-toast";
import { UserFormDialog, type UserFormValues } from "./UserFormDialog";

export type UserListItem = {
  id: string;
  userName: string;
  email?: string;
  name?: string;
  surname?: string;
};

const MOCK_USERS: UserListItem[] = [
  { id: "1", userName: "admin", email: "admin@example.com", name: "Admin" },
  { id: "2", userName: "john", email: "john@example.com", name: "John Doe" },
];

async function fetchUsers(): Promise<UserListItem[]> {
  if (!hasApiConfigured()) return MOCK_USERS;
  const data = await fetcher<{ items?: UserListItem[]; Items?: UserListItem[] }>(
    "/api/identity/users"
  );
  return getPagedItems(data);
}

const UsersListPage = () => {
  const queryClient = useQueryClient();
  const [formOpen, setFormOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<UserListItem | null>(null);
  const [deletingUser, setDeletingUser] = useState<UserListItem | null>(null);

  const { data: users = [], isLoading, error } = useQuery({
    queryKey: ["identity", "users"],
    queryFn: fetchUsers,
  });

  const createMutation = useMutation({
    mutationFn: async (body: UserFormValues) => {
      await fetcher("/api/identity/users", {
        method: "POST",
        body: JSON.stringify({
          userName: body.userName,
          name: body.name ?? "",
          surname: body.surname ?? "",
          email: body.email ?? "",
          password: "password" in body ? body.password : "",
        }),
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["identity", "users"] });
      queryClient.invalidateQueries({ queryKey: ["identity", "users", "count"] });
      toast({ title: "User created" });
      setFormOpen(false);
    },
    onError: (e: Error) => {
      toast({ title: "Failed to create user", description: e.message, variant: "destructive" });
    },
  });

  const updateMutation = useMutation({
    mutationFn: async ({ id, ...body }: UserFormValues & { id: string }) => {
      await fetcher(`/api/identity/users/${id}`, {
        method: "PUT",
        body: JSON.stringify({
          userName: body.userName,
          name: body.name ?? "",
          surname: body.surname ?? "",
          email: body.email ?? "",
        }),
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["identity", "users"] });
      queryClient.invalidateQueries({ queryKey: ["identity", "users", "count"] });
      toast({ title: "User updated" });
      setFormOpen(false);
      setEditingUser(null);
    },
    onError: (e: Error) => {
      toast({ title: "Failed to update user", description: e.message, variant: "destructive" });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await fetcher(`/api/identity/users/${id}`, { method: "DELETE" });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["identity", "users"] });
      queryClient.invalidateQueries({ queryKey: ["identity", "users", "count"] });
      toast({ title: "User deleted" });
      setDeletingUser(null);
    },
    onError: (e: Error) => {
      toast({ title: "Failed to delete user", description: e.message, variant: "destructive" });
    },
  });

  const handleFormSubmit = async (values: UserFormValues) => {
    if (editingUser) {
      await updateMutation.mutateAsync({ ...values, id: editingUser.id });
    } else {
      await createMutation.mutateAsync(values);
    }
  };

  const canMutate = hasApiConfigured();

  return (
    <div>
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold tracking-tight text-foreground">Users</h1>
        {canMutate && (
          <Button
            size="sm"
            onClick={() => {
              setEditingUser(null);
              setFormOpen(true);
            }}
          >
            <Plus className="mr-2 h-4 w-4" />
            Add user
          </Button>
        )}
      </div>
      <p className="mt-1 text-sm text-muted-foreground">
        Identity users from ABP API. {!canMutate && "Configure API to add or edit."}
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
                <TableHead className="w-[120px]">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {users.map((u) => (
                <TableRow key={u.id}>
                  <TableCell className="font-medium">{u.userName}</TableCell>
                  <TableCell>{u.email ?? "—"}</TableCell>
                  <TableCell>{[u.name, u.surname].filter(Boolean).join(" ") || "—"}</TableCell>
                  <TableCell>
                    {canMutate && (
                      <>
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => {
                            setEditingUser(u);
                            setFormOpen(true);
                          }}
                        >
                          <Pencil className="h-4 w-4" />
                        </Button>
                        <Button
                          variant="ghost"
                          size="sm"
                          className="text-destructive hover:text-destructive"
                          onClick={() => setDeletingUser(u)}
                        >
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </>
                    )}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </div>

      {canMutate && (
        <>
          <UserFormDialog
            key={editingUser?.id ?? "new"}
            open={formOpen}
            onOpenChange={(open) => {
              setFormOpen(open);
              if (!open) setEditingUser(null);
            }}
            defaultValues={
              editingUser
                ? {
                    id: editingUser.id,
                    userName: editingUser.userName,
                    email: editingUser.email ?? "",
                    name: editingUser.name ?? "",
                    surname: editingUser.surname ?? "",
                  }
                : undefined
            }
            onSubmit={handleFormSubmit}
            title={editingUser ? "Edit user" : "Add user"}
            submitLabel={editingUser ? "Save" : "Create"}
            isEdit={!!editingUser}
          />

          <AlertDialog open={!!deletingUser} onOpenChange={(open) => !open && setDeletingUser(null)}>
            <AlertDialogContent>
              <AlertDialogHeader>
                <AlertDialogTitle>Delete user</AlertDialogTitle>
                <AlertDialogDescription>
                  Are you sure you want to delete &quot;{deletingUser?.userName}&quot;? This cannot be
                  undone.
                </AlertDialogDescription>
              </AlertDialogHeader>
              <AlertDialogFooter>
                <AlertDialogCancel>Cancel</AlertDialogCancel>
                <AlertDialogAction
                  className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
                  onClick={() => deletingUser && deleteMutation.mutate(deletingUser.id)}
                >
                  Delete
                </AlertDialogAction>
              </AlertDialogFooter>
            </AlertDialogContent>
          </AlertDialog>
        </>
      )}
    </div>
  );
};

export default UsersListPage;
