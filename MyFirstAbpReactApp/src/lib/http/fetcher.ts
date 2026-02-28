import { ABP_API_URL } from "@/lib/config/env";
import { getAccessToken } from "@/lib/auth/session";

export class AppError extends Error {
  constructor(
    message: string,
    public status?: number,
    public code?: string
  ) {
    super(message);
    this.name = "AppError";
  }
}

export async function fetcher<T>(
  path: string,
  options: RequestInit = {}
): Promise<T> {
  const baseUrl = ABP_API_URL || "";
  const url = path.startsWith("http") ? path : `${baseUrl}${path.startsWith("/") ? path : `/${path}`}`;

  const token = getAccessToken();
  const headers: HeadersInit = {
    "Content-Type": "application/json",
    ...options.headers,
  };
  if (token) {
    (headers as Record<string, string>)["Authorization"] = `Bearer ${token}`;
  }

  const res = await fetch(url, { ...options, headers });

  if (!res.ok) {
    let message = res.statusText;
    try {
      const body = await res.json();
      if (body.error?.message) message = body.error.message;
      else if (body.error?.details) message = body.error.details;
    } catch {
      // ignore
    }
    throw new AppError(message, res.status, (res as unknown as { code?: string }).code);
  }

  const contentType = res.headers.get("content-type");
  if (contentType?.includes("application/json")) {
    return res.json() as Promise<T>;
  }
  return res.text() as Promise<T>;
}
