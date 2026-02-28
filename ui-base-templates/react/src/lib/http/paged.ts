/**
 * Normalize ABP paged response. Backend may return camelCase (items, totalCount)
 * or PascalCase (Items, TotalCount) depending on serialization.
 */
export type PagedResponse<T> = {
  items?: T[];
  totalCount?: number;
  Items?: T[];
  TotalCount?: number;
};

export function getPagedItems<T>(data: PagedResponse<T> | null | undefined): T[] {
  if (!data) return [];
  const list = data.items ?? data.Items;
  return Array.isArray(list) ? list : [];
}

export function getPagedTotalCount(data: PagedResponse<unknown> | null | undefined): number {
  if (!data) return 0;
  const n = data.totalCount ?? data.TotalCount;
  return typeof n === "number" ? n : 0;
}
