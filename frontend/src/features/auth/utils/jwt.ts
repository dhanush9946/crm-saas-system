export interface DecodedToken {
  sub: string;             // userId
  email: string;
  tenantId: string;
  sessionId: string;
  role?: string;
  exp?: number;
}

/**
 * Decodes a JWT token securely using pure JavaScript to avoid adding dependency weight.
 * Normalizes Microsoft C# claim keys into friendly frontend tokens.
 */
export function decodeJwt(token: string): DecodedToken | null {
  try {
    const parts = token.split('.');
    if (parts.length !== 3) return null;

    // Decode base64url payload segment safely
    const base64Url = parts[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      window.atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );

    const parsed = JSON.parse(jsonPayload);

    // Normalize roles: Microsoft C# uses full XML schema URLs for role claims
    const microsoftRoleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
    const rawRole = parsed[microsoftRoleClaim] || parsed.role;
    
    // If multiple roles are provided, take the primary one, otherwise fall back to User
    const normalizedRole = Array.isArray(rawRole)
      ? rawRole[0]
      : typeof rawRole === 'string'
      ? rawRole
      : 'User';

    return {
      sub: parsed.sub || '',
      email: parsed.email || parsed.unique_name || '',
      tenantId: parsed.tenantId || '',
      sessionId: parsed.sessionId || '',
      role: normalizedRole,
      exp: parsed.exp,
    };
  } catch (error) {
    console.error('Error decoding JWT token:', error);
    return null;
  }
}
