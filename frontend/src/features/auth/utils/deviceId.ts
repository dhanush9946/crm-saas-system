const DEVICE_ID_STORAGE_KEY = 'crm_device_id';

/**
 * Stable per-browser identifier. Stored in localStorage only — never used for secrets.
 * Generated once with crypto-safe APIs and reused across sessions.
 */
export function getOrCreateDeviceId(): string {
  try {
    const existing = localStorage.getItem(DEVICE_ID_STORAGE_KEY);
    if (existing) {
      return existing;
    }

    const deviceId = generateSecureDeviceId();
    localStorage.setItem(DEVICE_ID_STORAGE_KEY, deviceId);
    return deviceId;
  } catch {
    // Private browsing / storage blocked — ephemeral id for this tab only
    return generateSecureDeviceId();
  }
}

function generateSecureDeviceId(): string {
  if (typeof crypto !== 'undefined' && typeof crypto.randomUUID === 'function') {
    return crypto.randomUUID();
  }

  if (typeof crypto !== 'undefined' && crypto.getRandomValues) {
    const bytes = new Uint8Array(16);
    crypto.getRandomValues(bytes);
    bytes[6] = (bytes[6] & 0x0f) | 0x40;
    bytes[8] = (bytes[8] & 0x3f) | 0x80;
    const hex = Array.from(bytes, (b) => b.toString(16).padStart(2, '0')).join('');
    return `${hex.slice(0, 8)}-${hex.slice(8, 12)}-${hex.slice(12, 16)}-${hex.slice(16, 20)}-${hex.slice(20)}`;
  }

  throw new Error('Secure random generation is not available in this browser.');
}
