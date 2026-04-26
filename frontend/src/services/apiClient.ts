import { getStoredToken } from "./authService"

export async function apiFetch(url: string, options: RequestInit = {}): Promise<Response> {
  const token = getStoredToken()

  const headers = new Headers(options.headers ?? {})

  if (!headers.has("Content-Type") && options.body) {
    headers.set("Content-Type", "application/json")
  }

  if (token) {
    headers.set("Authorization", `Bearer ${token}`)
  }

  return fetch(url, {
    ...options,
    headers
  })
}