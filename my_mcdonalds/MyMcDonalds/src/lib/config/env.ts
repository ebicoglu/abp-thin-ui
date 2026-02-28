/**
 * Environment configuration for ABP API. Vite exposes env vars prefixed with VITE_.
 */
const raw = import.meta.env.VITE_ABP_API_URL;
export const ABP_API_URL = typeof raw === "string" && raw.length > 0 ? raw.replace(/\/$/, "") : "";

export function hasApiConfigured(): boolean {
  return ABP_API_URL.length > 0;
}
