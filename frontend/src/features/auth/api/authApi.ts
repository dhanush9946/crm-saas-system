import { baseApi } from '@shared/services/baseApi';
import { setCredentials, logout } from '../store/authSlice';
import { mapAuthResponseToCredentials } from '../services/authSession';
import type {
  ApiResponse,
  AuthPublicResponseData,
  LoginRequest,
  RegisterRequest,
} from '../types/auth.types';

export const authApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    login: builder.mutation<ApiResponse<AuthPublicResponseData>, LoginRequest>({
      query: (credentials) => ({
        url: '/auth/login',
        method: 'POST',
        body: credentials,
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data: response } = await queryFulfilled;
          if (!response.success || !response.data) {
            return;
          }

          const credentialsPayload = mapAuthResponseToCredentials(response.data, {
            email: arg.email,
          });

          if (credentialsPayload) {
            dispatch(setCredentials(credentialsPayload));
          }
        } catch (error) {
          console.error('Login failed:', error);
        }
      },
    }),

    register: builder.mutation<ApiResponse<AuthPublicResponseData>, RegisterRequest>({
      query: (details) => ({
        url: '/auth/register',
        method: 'POST',
        body: {
          tenantName: details.tenantName,
          tenantSlug: details.tenantSlug,
          email: details.email,
          password: details.password,
          displayName: details.displayName,
        },
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data: response } = await queryFulfilled;
          if (!response.success || !response.data) {
            return;
          }

          const credentialsPayload = mapAuthResponseToCredentials(response.data, {
            email: arg.email,
            displayName: arg.displayName,
          });

          if (credentialsPayload) {
            dispatch(setCredentials(credentialsPayload));
          }
        } catch (error) {
          console.error('Registration failed:', error);
        }
      },
    }),

    /**
     * Silent session restore — refresh token travels via HttpOnly cookie only.
     */
    restoreSession: builder.mutation<ApiResponse<AuthPublicResponseData>, void>({
      query: () => ({
        url: '/auth/refresh',
        method: 'POST',
        body: {},
      }),
    }),

    logout: builder.mutation<void, void>({
      query: () => ({
        url: '/auth/logout',
        method: 'POST',
        body: {},
      }),
      async onQueryStarted(_arg, { dispatch, queryFulfilled }) {
        dispatch(logout());
        try {
          await queryFulfilled;
        } catch (error) {
          console.error('Logout request error:', error);
        }
      },
    }),
  }),
});

export const {
  useLoginMutation,
  useRegisterMutation,
  useRestoreSessionMutation,
  useLogoutMutation,
} = authApi;
