import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import type { BaseQueryFn, FetchArgs, FetchBaseQueryError } from '@reduxjs/toolkit/query/react';
import type { RootState } from '@app/store';
import { setCredentials, logout } from '@features/auth/store/authSlice';
import { mapAuthResponseToCredentials } from '@features/auth/services/authSession';
import { runExclusiveRefresh } from '@features/auth/services/tokenRefreshMutex';
import { getOrCreateDeviceId } from '@features/auth/utils/deviceId';
import type { ApiResponse, AuthPublicResponseData } from '@features/auth/types/auth.types';

const API_BASE_URL = import.meta.env.VITE_API_URL || '/api/v1';

const AUTH_REFRESH_PATH = '/auth/refresh';

function getRequestUrl(args: string | FetchArgs): string {
  return typeof args === 'string' ? args : args.url;
}

function isAuthRefreshRequest(args: string | FetchArgs): boolean {
  return getRequestUrl(args).includes(AUTH_REFRESH_PATH);
}

const baseQuery = fetchBaseQuery({
  baseUrl: API_BASE_URL,
  credentials: 'include',
  prepareHeaders: (headers, { getState }) => {
    const state = getState() as RootState;
    const token = state.auth.accessToken;
    const tenantId = state.auth.tenantId;

    if (token) {
      headers.set('authorization', `Bearer ${token}`);
    }

    if (tenantId) {
      headers.set('X-Tenant-Id', tenantId);
    }

    headers.set('X-Device-Id', getOrCreateDeviceId());

    return headers;
  },
});

async function performSilentRefresh(
  api: Parameters<BaseQueryFn>[1],
  extraOptions: Parameters<BaseQueryFn>[2],
): Promise<boolean> {
  const refreshResult = await baseQuery(
    {
      url: AUTH_REFRESH_PATH,
      method: 'POST',
      body: {},
    },
    api,
    extraOptions,
  );

  const response = refreshResult.data as ApiResponse<AuthPublicResponseData> | undefined;

  if (response?.success && response.data) {
    const credentials = mapAuthResponseToCredentials(response.data);
    if (credentials) {
      api.dispatch(setCredentials(credentials));
      return true;
    }
  }

  api.dispatch(logout());
  return false;
}

const baseQueryWithReauth: BaseQueryFn<string | FetchArgs, unknown, FetchBaseQueryError> = async (
  args,
  api,
  extraOptions,
) => {
  let result = await baseQuery(args, api, extraOptions);

  if (result.error?.status !== 401 || isAuthRefreshRequest(args)) {
    return result;
  }

  const refreshed = await runExclusiveRefresh(() => performSilentRefresh(api, extraOptions));

  if (refreshed) {
    result = await baseQuery(args, api, extraOptions);
  }

  return result;
};

export const baseApi = createApi({
  reducerPath: 'api',
  baseQuery: baseQueryWithReauth,
  tagTypes: ['User', 'Customer', 'Lead', 'Deal', 'Activity', 'Analytics'],
  endpoints: () => ({}),
});
