import { ABP_API_URL } from "@/lib/config/env";
import type { TokenResponse } from "./types";

const TOKEN_ENDPOINT = "/connect/token";
const CLIENT_ID = "MultiLayerAbp_App";
/** Scope MultiLayerAbp ensures the token gets audience "MultiLayerAbp" so API calls are accepted. */
const SCOPE = "openid profile email roles MultiLayerAbp";

export async function loginWithPassword(username: string, password: string): Promise<TokenResponse> {
  const url = `${ABP_API_URL}${TOKEN_ENDPOINT}`;
  const body = new URLSearchParams({
    grant_type: "password",
    username,
    password,
    client_id: CLIENT_ID,
    scope: SCOPE,
  });

  const res = await fetch(url, {
    method: "POST",
    headers: { "Content-Type": "application/x-www-form-urlencoded" },
    body: body.toString(),
  });

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.error_description || err.error || res.statusText);
  }

  return res.json() as Promise<TokenResponse>;
}

export async function refreshAccessToken(refreshToken: string): Promise<TokenResponse> {
  const url = `${ABP_API_URL}${TOKEN_ENDPOINT}`;
  const body = new URLSearchParams({
    grant_type: "refresh_token",
    refresh_token: refreshToken,
    client_id: CLIENT_ID,
    scope: SCOPE,
  });

  const res = await fetch(url, {
    method: "POST",
    headers: { "Content-Type": "application/x-www-form-urlencoded" },
    body: body.toString(),
  });

  if (!res.ok) {
    const err = await res.json().catch(() => ({}));
    throw new Error(err.error_description || err.error || "Refresh failed");
  }

  return res.json() as Promise<TokenResponse>;
}
