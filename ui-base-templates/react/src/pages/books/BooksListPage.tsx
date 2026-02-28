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
import { BookFormDialog, type BookFormValues } from "./BookFormDialog";

export type BookListItem = {
  id: string;
  name: string;
  type: number;
  publishDate: string;
  price: number;
};

type BooksResponse = { items?: BookListItem[]; Items?: BookListItem[] };

async function fetchBooks(): Promise<BookListItem[]> {
  const data = await fetcher<BooksResponse>("/api/app/book");
  return getPagedItems(data);
}

const BooksListPage = () => {
  const queryClient = useQueryClient();
  const [formOpen, setFormOpen] = useState(false);
  const [editingBook, setEditingBook] = useState<BookListItem | null>(null);
  const [deletingBook, setDeletingBook] = useState<BookListItem | null>(null);

  const { data: books = [], isLoading, error } = useQuery({
    queryKey: ["books"],
    queryFn: fetchBooks,
  });

  const createMutation = useMutation({
    mutationFn: async (body: BookFormValues) => {
      await fetcher("/api/app/book", {
        method: "POST",
        body: JSON.stringify({
          name: body.name,
          type: body.type,
          publishDate: body.publishDate,
          price: body.price,
        }),
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["books"] });
      queryClient.invalidateQueries({ queryKey: ["books", "count"] });
      toast({ title: "Book created" });
      setFormOpen(false);
    },
    onError: (e: Error) => {
      toast({ title: "Failed to create book", description: e.message, variant: "destructive" });
    },
  });

  const updateMutation = useMutation({
    mutationFn: async ({ id, ...body }: BookFormValues & { id: string }) => {
      await fetcher(`/api/app/book/${id}`, {
        method: "PUT",
        body: JSON.stringify({
          name: body.name,
          type: body.type,
          publishDate: body.publishDate,
          price: body.price,
        }),
      });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["books"] });
      queryClient.invalidateQueries({ queryKey: ["books", "count"] });
      toast({ title: "Book updated" });
      setFormOpen(false);
      setEditingBook(null);
    },
    onError: (e: Error) => {
      toast({ title: "Failed to update book", description: e.message, variant: "destructive" });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: async (id: string) => {
      await fetcher(`/api/app/book/${id}`, { method: "DELETE" });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["books"] });
      queryClient.invalidateQueries({ queryKey: ["books", "count"] });
      toast({ title: "Book deleted" });
      setDeletingBook(null);
    },
    onError: (e: Error) => {
      toast({ title: "Failed to delete book", description: e.message, variant: "destructive" });
    },
  });

  const handleFormSubmit = async (values: BookFormValues) => {
    if (editingBook) {
      await updateMutation.mutateAsync({ ...values, id: editingBook.id });
    } else {
      await createMutation.mutateAsync(values);
    }
  };

  return (
    <div>
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-semibold tracking-tight text-foreground">Books</h1>
        <Button
          size="sm"
          onClick={() => {
            setEditingBook(null);
            setFormOpen(true);
          }}
        >
          <Plus className="mr-2 h-4 w-4" />
          Add book
        </Button>
      </div>
      <p className="mt-1 text-sm text-muted-foreground">Manage books from ABP API.</p>
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
                <TableHead>Type</TableHead>
                <TableHead>Publish date</TableHead>
                <TableHead>Price</TableHead>
                <TableHead className="w-[120px]">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {books.map((b) => (
                <TableRow key={b.id}>
                  <TableCell className="font-medium">{b.name}</TableCell>
                  <TableCell>{b.type}</TableCell>
                  <TableCell>
                    {b.publishDate ? new Date(b.publishDate).toLocaleDateString() : "—"}
                  </TableCell>
                  <TableCell>{b.price}</TableCell>
                  <TableCell>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => {
                        setEditingBook(b);
                        setFormOpen(true);
                      }}
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      className="text-destructive hover:text-destructive"
                      onClick={() => setDeletingBook(b)}
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

      <BookFormDialog
        key={editingBook?.id ?? "new"}
        open={formOpen}
        onOpenChange={(open) => {
          setFormOpen(open);
          if (!open) setEditingBook(null);
        }}
        defaultValues={
          editingBook
            ? {
                id: editingBook.id,
                name: editingBook.name,
                type: editingBook.type,
                publishDate: editingBook.publishDate,
                price: editingBook.price,
              }
            : undefined
        }
        onSubmit={handleFormSubmit}
        title={editingBook ? "Edit book" : "Add book"}
        submitLabel={editingBook ? "Save" : "Create"}
      />

      <AlertDialog open={!!deletingBook} onOpenChange={(open) => !open && setDeletingBook(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete book</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete &quot;{deletingBook?.name}&quot;? This cannot be undone.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
              onClick={() => deletingBook && deleteMutation.mutate(deletingBook.id)}
            >
              Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
};

export default BooksListPage;
