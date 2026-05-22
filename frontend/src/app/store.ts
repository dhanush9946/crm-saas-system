import { configureStore } from '@reduxjs/toolkit';
import { baseApi } from '@shared/services/baseApi';
import authReducer from '@features/auth/store/authSlice';

export const store = configureStore({
  reducer: {
    // Inject global RTK Query reducer
    [baseApi.reducerPath]: baseApi.reducer,
    // Inject feature-specific auth slice
    auth: authReducer,
  },
  // Add baseApi middleware for caching, invalidation, polling, and reauth interceptors
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(baseApi.middleware),
});

// Root State type
export type RootState = ReturnType<typeof store.getState>;

// App Dispatch type
export type AppDispatch = typeof store.dispatch;
