import type { AuthCredentials, AuthPublicResponseData, User } from '../types/auth.types';
import { decodeJwt } from '../utils/jwt';

interface BuildUserOptions {
  email?: string;
  displayName?: string;
}

/**
 * Builds the in-memory user profile from JWT claims (display only — API enforces authorization).
 */
export function buildUserFromAccessToken(
  accessToken: string,
  userId: string,
  options?: BuildUserOptions,
): User | null {
  const decoded = decodeJwt(accessToken);
  if (!decoded) {
    return null;
  }

  return {
    id: userId,
    email: decoded.email || options?.email || '',
    fullName: options?.displayName || decoded.email?.split('@')[0] || 'User',
    role: decoded.role || 'User',
  };
}

export function mapAuthResponseToCredentials(
  data: AuthPublicResponseData,
  options?: BuildUserOptions,
): AuthCredentials | null {
  const user = buildUserFromAccessToken(data.accessToken, data.userId, options);
  if (!user) {
    return null;
  }

  return {
    user,
    accessToken: data.accessToken,
    tenantId: data.tenantId,
  };
}
