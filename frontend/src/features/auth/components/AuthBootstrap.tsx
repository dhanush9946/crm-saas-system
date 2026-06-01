import React, { useEffect, useRef } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Box, CircularProgress } from '@mui/material';
import type { AppDispatch, RootState } from '@app/store';
import { useRestoreSessionMutation } from '../api/authApi';
import { setCredentials, markUnauthenticated } from '../store/authSlice';
import { mapAuthResponseToCredentials } from '../services/authSession';
import { selectAuthStatus } from '../store/authSlice';

interface AuthBootstrapProps {
  children: React.ReactNode;
}

/**
 * On app load, attempts silent refresh using the HttpOnly cookie before routing decisions.
 */
export const AuthBootstrap: React.FC<AuthBootstrapProps> = ({ children }) => {
  const dispatch = useDispatch<AppDispatch>();
  const status = useSelector((state: RootState) => selectAuthStatus(state));
  const [restoreSession] = useRestoreSessionMutation();
  const hasBootstrapped = useRef(false);

  useEffect(() => {
    if (hasBootstrapped.current || status !== 'initializing') {
      return;
    }

    hasBootstrapped.current = true;

    const bootstrap = async () => {
      // Remove legacy insecure token persistence from earlier implementation
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('tenantId');

      try {
        const response = await restoreSession().unwrap();
        if (response.success && response.data) {
          const credentials = mapAuthResponseToCredentials(response.data);
          if (credentials) {
            dispatch(setCredentials(credentials));
            return;
          }
        }
        dispatch(markUnauthenticated());
      } catch {
        dispatch(markUnauthenticated());
      }
    };

    void bootstrap();
  }, [dispatch, restoreSession, status]);

  if (status === 'initializing') {
    return (
      <Box
        sx={{
          minHeight: '100vh',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
        }}
      >
        <CircularProgress size={32} />
      </Box>
    );
  }

  return <>{children}</>;
};

export default AuthBootstrap;
