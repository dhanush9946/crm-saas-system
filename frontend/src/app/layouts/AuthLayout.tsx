import React from 'react';
import { Outlet } from 'react-router-dom';
import { Box, Card, Typography, Container } from '@mui/material';

export const AuthLayout: React.FC = () => {
  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        background: 'linear-gradient(135deg, #EEF2F6 0%, #E3E9F3 100%)',
        py: 4,
      }}
    >
      <Container maxWidth="sm">
        <Box sx={{ mb: 4, textAlign: 'center' }}>
          <Typography variant="h4" sx={{ fontWeight: 800, color: 'primary.main', tracking: '-0.05em' }}>
            CRM<span style={{ color: '#7C3AED' }}>SaaS</span>
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
            Enterprise AI-Powered CRM Suite
          </Typography>
        </Box>
        <Card
          sx={{
            p: 4,
            boxShadow: '0 10px 25px -5px rgba(0, 0, 0, 0.05), 0 8px 10px -6px rgba(0, 0, 0, 0.05)',
            borderRadius: 3,
            bgcolor: 'background.paper',
          }}
        >
          <Outlet />
        </Card>
      </Container>
    </Box>
  );
};
export default AuthLayout;
