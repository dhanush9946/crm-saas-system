import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import type { BaseQueryFn, FetchArgs, FetchBaseQueryError } from '@reduxjs/toolkit/query/react';
import type { RootState } from '@app/store';
import { setCredentials, logout } from '@features/auth/store/authSlice';

// Retrieve API Base URL from Vite Environment
const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:5001/api/v1';

// Base Query configuring standard fetch behaviors, credentials, and headers
const baseQuery = fetchBaseQuery({
  baseUrl: API_BASE_URL,
  prepareHeaders: (headers, { getState }) => {
    // Cast state to RootState
    const state = getState() as RootState;
    const token = state.auth.accessToken;
    const tenantId = state.auth.tenantId;

    // Attach JWT if authenticated
    if (token) {
      headers.set('authorization', `Bearer ${token}`);
    }

    // Attach Tenant Identifier for multi-tenant backend partitioning
    if (tenantId) {
      headers.set('X-Tenant-Id', tenantId);
    }

    return headers;
  },
});

// Custom baseQuery wrapper to intercept 401 Unauthorized responses and rotate refresh tokens
const baseQueryWithReauth: BaseQueryFn<
  string | FetchArgs,
  unknown,
  FetchBaseQueryError
> = async (args, api, extraOptions) => {
  let result = await baseQuery(args, api, extraOptions);

  // If unauthorized, attempt to perform a silent refresh
  if (result.error && result.error.status === 401) {
    // Try to get a new token using the refresh endpoint
    // Note: HttpOnly Cookie rotation usually handles sending/receiving refresh cookies,
    // so we call the endpoint without needing explicit body parameters.
    const refreshResult = await baseQuery(
      {
        url: '/identity/refresh-token',
        method: 'POST',
      },
      api,
      extraOptions
    );

    if (refreshResult.data) {
      // Store the new credentials in Redux
      const data = refreshResult.data as { accessToken: string; refreshToken?: string; tenantId?: string };
      api.dispatch(
        setCredentials({
          accessToken: data.accessToken,
          tenantId: data.tenantId || (api.getState() as RootState).auth.tenantId || undefined,
        })
      );

      // Retry the original query with the new token
      result = await baseQuery(args, api, extraOptions);
    } else {
      // Refresh failed or is invalid; force logout user session
      api.dispatch(logout());
    }
  }

  return result;
};

// Global API Service definitions
export const baseApi = createApi({
  reducerPath: 'api',
  baseQuery: baseQueryWithReauth,
  tagTypes: ['User', 'Customer', 'Lead', 'Deal', 'Activity', 'Analytics'],
  endpoints: () => ({}), // Endpoints will be injected dynamically from features
});
