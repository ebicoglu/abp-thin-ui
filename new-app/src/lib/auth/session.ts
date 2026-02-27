/**
 * Auth session stub. Replace with OIDC/NextAuth when implementing full login.
 * POC: no token; fetcher will call API without Authorization when token is null.
 */
export function getAccessToken(): string | null {
  return null;
}

export function isAuthenticated(): boolean {
  return getAccessToken() != null;
}
