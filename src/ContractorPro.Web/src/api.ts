/**
 * API client helper — hand-typed fetch wrapper for /api/v1 endpoints.
 * Credentials included for session cookies (1.1+).
 */
export async function apiCall<T>(
  endpoint: string,
  options?: RequestInit
): Promise<T> {
  const url = `/api/v1${endpoint}`;
  const response = await fetch(url, {
    ...options,
    credentials: 'include',
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
  });

  if (!response.ok) {
    throw new Error(`API error: ${response.status} ${response.statusText}`);
  }

  return response.json();
}

export interface TeamMeResponse {
  user: {
    id: string
    displayName: string
    email: string
  }
  teamMember: {
    id: string
    role: string
    isOwner: boolean
  }
  contractor: {
    id: string
    name: string
    status: string
  }
}

export function getTeamMe(): Promise<TeamMeResponse> {
  return apiCall<TeamMeResponse>('/team/me')
}

export async function logout(): Promise<void> {
  await fetch('/api/v1/auth/logout', {
    method: 'POST',
    credentials: 'include',
  })
}
