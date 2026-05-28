import React from 'react';
import { Navigate } from 'react-router-dom';
import { Box, CircularProgress } from '@mui/material';
import { useAuth } from '../hooks/useAuth';

interface GuestRouteProps {
  children: React.ReactNode;
}

/**
 * Prevents authenticated users from visiting login/register while session restore runs.
 */
export const GuestRoute: React.FC<GuestRouteProps> = ({ children }) => {
  const { isInitializing, isAuthenticated } = useAuth();

  if (isInitializing) {
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

  if (isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
};

export default GuestRoute;
