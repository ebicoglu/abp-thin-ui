import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { loginWithPassword, refreshAccessToken } from "./api";
import { setAccessToken } from "./session";
import type { AuthState } from "./types";

const STORAGE_KEY = "abp_token";
const REFRESH_KEY = "abp_refresh";
const EXPIRES_KEY = "abp_expires";

function loadStored(): Partial<AuthState> {
  if (typeof window === "undefined") return {};
  try {
    const token = localStorage.getItem(STORAGE_KEY);
    const refresh = localStorage.getItem(REFRESH_KEY);
    const expires = localStorage.getItem(EXPIRES_KEY);
    if (!token) return {};
    return {
      accessToken: token,
      refreshToken: refresh || null,
      expiresAt: expires ? parseInt(expires, 10) : null,
      isAuthenticated: true,
    };
  } catch {
    return {};
  }
}

function saveStored(accessToken: string, refreshToken?: string, expiresIn?: number) {
  try {
    localStorage.setItem(STORAGE_KEY, accessToken);
    if (refreshToken) localStorage.setItem(REFRESH_KEY, refreshToken);
    if (expiresIn != null)
      localStorage.setItem(EXPIRES_KEY, String(Date.now() + expiresIn * 1000));
  } catch {
    // ignore
  }
}

function clearStored() {
  try {
    localStorage.removeItem(STORAGE_KEY);
    localStorage.removeItem(REFRESH_KEY);
    localStorage.removeItem(EXPIRES_KEY);
  } catch {
    // ignore
  }
}

type AuthContextValue = AuthState & {
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  getAccessToken: () => string | null;
};

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>(() => {
    const stored = loadStored();
    if (stored.accessToken) setAccessToken(stored.accessToken);
    return {
      accessToken: null,
      refreshToken: null,
      expiresAt: null,
      isAuthenticated: false,
      ...stored,
    };
  });

  const login = useCallback(async (username: string, password: string) => {
    const data = await loginWithPassword(username, password);
    saveStored(data.access_token, data.refresh_token, data.expires_in);
    setAccessToken(data.access_token);
    setState({
      accessToken: data.access_token,
      refreshToken: data.refresh_token ?? null,
      expiresAt: data.expires_in ? Date.now() + data.expires_in * 1000 : null,
      isAuthenticated: true,
    });
  }, []);

  const logout = useCallback(() => {
    setAccessToken(null);
    clearStored();
    setState({
      accessToken: null,
      refreshToken: null,
      expiresAt: null,
      isAuthenticated: false,
    });
  }, []);

  useEffect(() => {
    const refresh = state.refreshToken;
    const expiresAt = state.expiresAt;
    if (!refresh || !expiresAt) return;
    const margin = 60 * 1000;
    if (Date.now() < expiresAt - margin) return;
    refreshAccessToken(refresh)
      .then((data) => {
        setAccessToken(data.access_token);
        saveStored(data.access_token, data.refresh_token, data.expires_in);
        setState((s) => ({
          ...s,
          accessToken: data.access_token,
          refreshToken: data.refresh_token ?? s.refreshToken,
          expiresAt: data.expires_in ? Date.now() + data.expires_in * 1000 : null,
        }));
      })
      .catch(() => logout());
  }, [state.refreshToken, state.expiresAt, logout]);

  const getAccessToken = useCallback(() => state.accessToken, [state.accessToken]);

  const value = useMemo<AuthContextValue>(
    () => ({ ...state, login, logout, getAccessToken }),
    [state, login, logout, getAccessToken]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
