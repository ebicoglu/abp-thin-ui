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
import { toast } from "@/components/ui/use-toast";
import { RoleFormDialog, type RoleFormValues } from "./RoleFormDialog";

export type RoleListItem = {
  id: string;
  name: string;
  description?: string;
  isPublic?: boolean;
};

type RolesResponse = { items?: RoleListItem[]; Items?: RoleListItem[] };

async function fetchRoles(): Promise<RoleListItem[]> {
  const data = await fetcher<RolesResponse>("/api/identity/roles");
  return getPagedItems(data);
}

const RolesListPage = () => {
  const queryClient = useQueryClient();
  const [formOpen, setFormOpen] = useState(false);
  const [editingRole, setEditingRole] = useState<RoleListItem | null>(null);
  const [deletingRole, setDeletingRole] = useState<RoleListItem | null>(null);

  const { data: roles = [], isLoading, error } = useQuery({
    queryKey: ["identity", "roles"],
    queryFn: fetchRoles,
  });

  const createMutation = useMutation({
    mutationFn: async (body: RoleFormValues) => {
      await fetcher("/api/identity/roles", {
        method: "POST",
        body: JSON.stringify({
          name: body.name,
          description: body.description ?? "",
          isPublic: body.isPublic ?? true,
        }),
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["identity", "roles"] });
      queryClient.invalidateQueries({ queryKey: ["identity", "roles", "count"] });
      toast({ title: "Role created" });
      setFormOpen(false);
    },
    onError: (e: Error) => {
      toast({ title: "Failed to create role", description: e.message, variant: "destructive" });
    },
  });

  const updateMutation = useMutation({
    mutationFn: async ({ id, ...body }: RoleFormValues & { id: string }) => {
      await fetcher(`/api/identity/roles/${id}`, {
        method: "PUT",
        body: JSON.stringify({
          name: body.name,
          description: body.description ?? "",
          isPublic: body.isPublic ?? true,
        }),
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["identity", "roles"] });
      queryClient.invalidateQueries({ queryKey: ["identity", "roles", "count"] });
      toast({ title: "Role updated" });
      setFormOpen(false);
      setEditingRole(null);
    },
    onError: (e: Error) => {
      toast({ title: "Failed to update role", description: e.message, variant: "destructive" });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await fetcher(`/api/identity/roles/${id}`, { method: "DELETE" });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["identity", "roles"] });
      queryClient.invalidateQueries({ queryKey: ["identity", "roles", "count"] });
      toast({ title: "Role deleted" });
      setDeletingRole(null);
    },
    onError: (e: Error) => {
      toast({ title: "Failed to delete role", description: e.message, variant: "destructive" });
    },
  });

  const handleFormSubmit = async (values: RoleFormValues) => {
    if (editingRole) {
      await updateMutation.mutateAsync({ ...values, id: editingRole.id });
    } else {
      await createMutation.mutateAsync(values);
    }
  };

  return (
    <div>
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold tracking-tight text-foreground">Roles</h1>
        <Button
          size="sm"
          onClick={() => {
            setEditingRole(null);
            setFormOpen(true);
          }}
        >
          <Plus className="mr-2 h-4 w-4" />
          Add role
        </Button>
      </div>
      <p className="mt-1 text-sm text-muted-foreground">Manage identity roles from ABP API.</p>
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
                <TableHead>Name</TableHead>
                <TableHead>Description</TableHead>
                <TableHead>Public</TableHead>
                <TableHead className="w-[120px]">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {roles.map((r) => (
                <TableRow key={r.id}>
                  <TableCell className="font-medium">{r.name}</TableCell>
                  <TableCell>{r.description ?? "—"}</TableCell>
                  <TableCell>{r.isPublic ? "Yes" : "No"}</TableCell>
                  <TableCell>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => {
                        setEditingRole(r);
                        setFormOpen(true);
                      }}
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      className="text-destructive hover:text-destructive"
                      onClick={() => setDeletingRole(r)}
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </div>

      <RoleFormDialog
        key={editingRole?.id ?? "new"}
        open={formOpen}
        onOpenChange={(open) => {
          setFormOpen(open);
          if (!open) setEditingRole(null);
        }}
        defaultValues={
          editingRole
            ? {
                id: editingRole.id,
                name: editingRole.name,
                description: editingRole.description ?? "",
                isPublic: editingRole.isPublic ?? true,
              }
            : undefined
        }
        onSubmit={handleFormSubmit}
        title={editingRole ? "Edit role" : "Add role"}
        submitLabel={editingRole ? "Save" : "Create"}
      />

      <AlertDialog open={!!deletingRole} onOpenChange={(open) => !open && setDeletingRole(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete role</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete &quot;{deletingRole?.name}&quot;? This cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
              onClick={() => deletingRole && deleteMutation.mutate(deletingRole.id)}
            >
              Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
};

export default RolesListPage;
