import type {
    AuthResponse,
    CurrentUser,
    LoginRequest,
    RegisterRequest,
} from '../types/auth';

const API_URL = "http://localhost:5158/api/auth";
const TOKEN_KEY = "authToken"

export function getStoredToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
}

export function storeToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token);
}

export function clearToken(): void {
    localStorage.removeItem(TOKEN_KEY);
}

export async function login(data: LoginRequest): Promise<AuthResponse> {
    const response: Response = await fetch(`${API_URL}/login`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    });
    if(!response.ok) {
        const error = await response.json();
        throw new Error(error.message ?? 'Login failed');
    }

    return await response.json() as AuthResponse;
}

export async function register(data: RegisterRequest): Promise<AuthResponse> {
  const response = await fetch(`${API_URL}/register`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(data)
  })

  if (!response.ok) {
    const error = await response.json()
    throw new Error(error.message ?? "Registratie mislukt.")
  }

  return await response.json()
}

export async function getCurrentUser(token: string): Promise<CurrentUser> {
  const response = await fetch(`${API_URL}/me`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (!response.ok) {
    throw new Error("Sessie ongeldig.")
  }

  return await response.json()
}