import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';

export interface User {
  id: string;
  email: string;
  fullName: string;
  role: string;
}

interface AuthState {
  user: User | null;
  accessToken: string | null;
  tenantId: string | null;
  isAuthenticated: boolean;
}

const initialState: AuthState = {
  user: null,
  accessToken: null,
  tenantId: null,
  isAuthenticated: false,
};

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setCredentials: (
      state,
      action: PayloadAction<{ user?: User; accessToken: string; tenantId?: string }>
    ) => {
      const { user, accessToken, tenantId } = action.payload;
      state.accessToken = accessToken;
      state.isAuthenticated = true;
      if (user) {
        state.user = user;
      }
      if (tenantId) {
        state.tenantId = tenantId;
      }
    },
    logout: (state) => {
      state.user = null;
      state.accessToken = null;
      state.tenantId = null;
      state.isAuthenticated = false;
    },
  },
});

export const { setCredentials, logout } = authSlice.actions;
export default authSlice.reducer;
