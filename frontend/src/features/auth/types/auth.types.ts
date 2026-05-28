export interface User {
  id: string;
  email: string;
  fullName: string;
  role: string;
}

/** Redux auth lifecycle — avoids redirecting before silent refresh completes. */
export type AuthStatus = 'initializing' | 'authenticated' | 'unauthenticated';

export interface AuthPublicResponseData {
  tenantId: string;
  userId: string;
  sessionId: string;
  accessToken: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  traceId: string;
}

export interface LoginRequest {
  tenantSlug: string;
  email: string;
  password: string;
}

export interface RegisterRequest {
  tenantName: string;
  tenantSlug: string;
  email: string;
  password: string;
  displayName: string;
}

export interface AuthCredentials {
  user: User;
  accessToken: string;
  tenantId: string;
}
