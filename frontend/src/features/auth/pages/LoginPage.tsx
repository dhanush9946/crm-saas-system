import React, { useState } from 'react';
import { useNavigate, useLocation, Link as RouterLink } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { toast } from 'react-hot-toast';
import {
  Box,
  Button,
  TextField,
  Typography,
  InputAdornment,
  IconButton,
  Link,
  CircularProgress,
} from '@mui/material';
import {
  EmailOutlined,
  LockOutlined,
  Visibility,
  VisibilityOff,
  BusinessOutlined,
} from '@mui/icons-material';
import { useLoginMutation } from '../api/authApi';
import { loginSchema, type LoginFormValues } from '../validations/authSchemas';

export const LoginPage: React.FC = () => {
  const [showPassword, setShowPassword] = useState(false);
  const [login, { isLoading }] = useLoginMutation();
  const navigate = useNavigate();
  const location = useLocation();
  
  // Extract redirect URL (if any) passed from Route Guards
  const from = (location.state as any)?.from?.pathname || '/';

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      tenantSlug: '',
      email: '',
      password: '',
    },
  });

  const onSubmit = async (data: LoginFormValues) => {
    try {
      const response = await login(data).unwrap();
      if (response.success) {
        toast.success(`Welcome back! Session successfully authorized.`);
        navigate(from, { replace: true });
      } else {
        toast.error('Authentication failed. Please verify credentials.');
      }
    } catch (error: any) {
      console.error('Login error:', error);
      const errorMsg = error?.data?.error || error?.data?.message || 'Invalid workspace slug, email or password.';
      toast.error(errorMsg);
    }
  };

  const handleTogglePassword = () => {
    setShowPassword(!showPassword);
  };

  return (
    <Box sx={{ width: '100%' }}>
      <Box sx={{ mb: 3, textAlign: 'center' }}>
        <Typography variant="h5" sx={{ fontWeight: 700, color: 'text.primary' }}>
          Sign In
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
          Access your multi-tenant workspace below
        </Typography>
      </Box>

      <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate sx={{ mt: 1 }}>
        <TextField
          margin="normal"
          required
          fullWidth
          id="tenantSlug"
          label="Workspace Slug"
          placeholder="e.g. Acme-corp"
          error={!!errors.tenantSlug}
          helperText={errors.tenantSlug?.message}
          {...register('tenantSlug')}
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <BusinessOutlined color={errors.tenantSlug ? 'error' : 'action'} />
                </InputAdornment>
              ),
            },
          }}
          sx={{
            '& .MuiOutlinedInput-root': {
              borderRadius: 2,
            },
          }}
        />

        <TextField
          margin="normal"
          required
          fullWidth
          id="email"
          label="Email Address"
          placeholder="dhanush@company.com"
          autoComplete="email"
          error={!!errors.email}
          helperText={errors.email?.message}
          {...register('email')}
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <EmailOutlined color={errors.email ? 'error' : 'action'} />
                </InputAdornment>
              ),
            },
          }}
          sx={{
            '& .MuiOutlinedInput-root': {
              borderRadius: 2,
            },
          }}
        />

        <TextField
          margin="normal"
          required
          fullWidth
          label="Password"
          type={showPassword ? 'text' : 'password'}
          id="password"
          autoComplete="current-password"
          error={!!errors.password}
          helperText={errors.password?.message}
          {...register('password')}
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <LockOutlined color={errors.password ? 'error' : 'action'} />
                </InputAdornment>
              ),
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    aria-label="toggle password visibility"
                    onClick={handleTogglePassword}
                    edge="end"
                  >
                    {showPassword ? <VisibilityOff /> : <Visibility />}
                  </IconButton>
                </InputAdornment>
              ),
            },
          }}
          sx={{
            '& .MuiOutlinedInput-root': {
              borderRadius: 2,
            },
          }}
        />

        <Box sx={{ mt: 3, mb: 2, position: 'relative' }}>
          <Button
            type="submit"
            fullWidth
            variant="contained"
            disabled={isLoading}
            sx={{
              py: 1.5,
              borderRadius: 2.5,
              fontWeight: 600,
              fontSize: '0.95rem',
              textTransform: 'none',
              boxShadow: '0 4px 12px rgba(124, 58, 237, 0.15)',
              background: 'linear-gradient(135deg, #7C3AED 0%, #6D28D9 100%)',
              '&:hover': {
                background: 'linear-gradient(135deg, #6D28D9 0%, #5B21B6 100%)',
                transform: 'translateY(-1px)',
                boxShadow: '0 6px 16px rgba(124, 58, 237, 0.25)',
              },
              transition: 'all 0.2s ease',
            }}
          >
            {isLoading ? (
              <CircularProgress size={24} sx={{ color: '#fff' }} />
            ) : (
              'Authenticate Session'
            )}
          </Button>
        </Box>

        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
          <Typography variant="body2" color="text.secondary">
            Don't have an organization subscription?{' '}
            <Link
              component={RouterLink}
              to="/register"
              sx={{
                fontWeight: 600,
                color: 'primary.main',
                textDecoration: 'none',
                '&:hover': { textDecoration: 'underline' },
              }}
            >
              Sign up
            </Link>
          </Typography>
        </Box>
      </Box>
    </Box>
  );
};
export default LoginPage;
