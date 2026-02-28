/**
 * Session token store. AuthContext updates this when user logs in/out or token refreshes.
 * Fetcher uses getAccessToken() so API calls include the Bearer token.
 */
let currentToken: string | null = null;

export function setAccessToken(token: string | null): void {
  currentToken = token;
}

export function getAccessToken(): string | null {
  return currentToken;
}

export function isAuthenticated(): boolean {
  return currentToken != null;
}
