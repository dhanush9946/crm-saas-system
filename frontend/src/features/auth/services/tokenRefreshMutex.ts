/**
 * Ensures only one silent refresh runs when multiple API calls receive 401 at once.
 */
let refreshPromise: Promise<boolean> | null = null;

export function runExclusiveRefresh(refreshFn: () => Promise<boolean>): Promise<boolean> {
  if (!refreshPromise) {
    refreshPromise = refreshFn().finally(() => {
      refreshPromise = null;
    });
  }

  return refreshPromise;
}
