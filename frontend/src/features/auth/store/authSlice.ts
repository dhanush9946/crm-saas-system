import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import type { AuthStatus, User } from '../types/auth.types';

export type { User } from '../types/auth.types';

interface AuthState {
  user: User | null;
  accessToken: string | null;
  tenantId: string | null;
  status: AuthStatus;
}

const initialState: AuthState = {
  user: null,
  accessToken: null,
  tenantId: null,
  status: 'initializing',
};

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setCredentials: (
      state,
      action: PayloadAction<{ user: User; accessToken: string; tenantId: string }>,
    ) => {
      state.user = action.payload.user;
      state.accessToken = action.payload.accessToken;
      state.tenantId = action.payload.tenantId;
      state.status = 'authenticated';
    },
    markUnauthenticated: (state) => {
      state.user = null;
      state.accessToken = null;
      state.tenantId = null;
      state.status = 'unauthenticated';
    },
    logout: (state) => {
      state.user = null;
      state.accessToken = null;
      state.tenantId = null;
      state.status = 'unauthenticated';
    },
  },
});

export const { setCredentials, markUnauthenticated, logout } = authSlice.actions;

export const selectAuthStatus = (state: { auth: AuthState }) => state.auth.status;
export const selectIsAuthenticated = (state: { auth: AuthState }) =>
  state.auth.status === 'authenticated';
export const selectAuthUser = (state: { auth: AuthState }) => state.auth.user;
export const selectAccessToken = (state: { auth: AuthState }) => state.auth.accessToken;
export const selectTenantId = (state: { auth: AuthState }) => state.auth.tenantId;

export default authSlice.reducer;
