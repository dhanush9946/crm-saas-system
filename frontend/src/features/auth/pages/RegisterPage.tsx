import React, { useState, useEffect } from 'react';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
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
  PersonOutlineOutlined,
  LinkOutlined,
} from '@mui/icons-material';
import { useRegisterMutation } from '../api/authApi';
import { registerSchema, type RegisterFormValues } from '../validations/authSchemas';

export const RegisterPage: React.FC = () => {
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [registerUser, { isLoading }] = useRegisterMutation();
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      tenantName: '',
      tenantSlug: '',
      displayName: '',
      email: '',
      password: '',
      confirmPassword: '',
    },
  });

  // Watch organization name to automatically suggest matching URL slug
  const tenantName = watch('tenantName');

  useEffect(() => {
    if (tenantName) {
      const generatedSlug = tenantName
        .toLowerCase()
        .replace(/[^a-z0-9\s-]/g, '') // remove symbols
        .trim()
        .replace(/\s+/g, '-') // spaces to hyphens
        .replace(/-+/g, '-'); // collapse multiple hyphens
      setValue('tenantSlug', generatedSlug, { shouldValidate: true });
    }
  }, [tenantName, setValue]);

  const onSubmit = async (data: RegisterFormValues) => {
    try {
      const response = await registerUser(data).unwrap();
      if (response.success) {
        toast.success(`Organization successfully subscribed and logged in!`);
        navigate('/', { replace: true });
      } else {
        toast.error('Registration failed. Please check fields.');
      }
    } catch (error: any) {
      console.error('Registration error:', error);
      const errorMsg = error?.data?.error || error?.data?.message || 'Email already exists or registration details are invalid.';
      toast.error(errorMsg);
    }
  };

  return (
    <Box sx={{ width: '100%' }}>
      <Box sx={{ mb: 2, textAlign: 'center' }}>
        <Typography variant="h5" sx={{ fontWeight: 700, color: 'text.primary' }}>
          Get Started
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
          Create a new isolated multi-tenant organization
        </Typography>
      </Box>

      <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate sx={{ mt: 1 }}>
        <TextField
          margin="dense"
          required
          fullWidth
          id="tenantName"
          label="Organization / Company Name"
          placeholder="e.g. Acme Corp"
          error={!!errors.tenantName}
          helperText={errors.tenantName?.message}
          {...register('tenantName')}
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <BusinessOutlined color={errors.tenantName ? 'error' : 'action'} />
                </InputAdornment>
              ),
            },
          }}
          sx={{
            '& .MuiOutlinedInput-root': { borderRadius: 2 },
          }}
        />

        <TextField
          margin="dense"
          required
          fullWidth
          id="tenantSlug"
          label="Workspace Slug (URL)"
          placeholder="e.g. acme-corp"
          error={!!errors.tenantSlug}
          helperText={errors.tenantSlug?.message || 'This will build your custom URL path'}
          {...register('tenantSlug')}
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <LinkOutlined color={errors.tenantSlug ? 'error' : 'action'} />
                </InputAdornment>
              ),
            },
          }}
          sx={{
            '& .MuiOutlinedInput-root': { borderRadius: 2 },
          }}
        />

        <TextField
          margin="dense"
          required
          fullWidth
          id="displayName"
          label="Your Full Name"
          placeholder="Dhanush"
          error={!!errors.displayName}
          helperText={errors.displayName?.message}
          {...register('displayName')}
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <PersonOutlineOutlined color={errors.displayName ? 'error' : 'action'} />
                </InputAdornment>
              ),
            },
          }}
          sx={{
            '& .MuiOutlinedInput-root': { borderRadius: 2 },
          }}
        />

        <TextField
          margin="dense"
          required
          fullWidth
          id="email"
          label="Admin Email Address"
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
            '& .MuiOutlinedInput-root': { borderRadius: 2 },
          }}
        />

        <TextField
          margin="dense"
          required
          fullWidth
          label="Password"
          type={showPassword ? 'text' : 'password'}
          id="password"
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
                  <IconButton onClick={() => setShowPassword(!showPassword)} edge="end">
                    {showPassword ? <VisibilityOff /> : <Visibility />}
                  </IconButton>
                </InputAdornment>
              ),
            },
          }}
          sx={{
            '& .MuiOutlinedInput-root': { borderRadius: 2 },
          }}
        />

        <TextField
          margin="dense"
          required
          fullWidth
          label="Confirm Password"
          type={showConfirmPassword ? 'text' : 'password'}
          id="confirmPassword"
          error={!!errors.confirmPassword}
          helperText={errors.confirmPassword?.message}
          {...register('confirmPassword')}
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <LockOutlined color={errors.confirmPassword ? 'error' : 'action'} />
                </InputAdornment>
              ),
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton onClick={() => setShowConfirmPassword(!showConfirmPassword)} edge="end">
                    {showConfirmPassword ? <VisibilityOff /> : <Visibility />}
                  </IconButton>
                </InputAdornment>
              ),
            },
          }}
          sx={{
            '& .MuiOutlinedInput-root': { borderRadius: 2 },
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
              'Create Multi-Tenant Organization'
            )}
          </Button>
        </Box>

        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
          <Typography variant="body2" color="text.secondary">
            Already have an active subscription?{' '}
            <Link
              component={RouterLink}
              to="/login"
              sx={{
                fontWeight: 600,
                color: 'primary.main',
                textDecoration: 'none',
                '&:hover': { textDecoration: 'underline' },
              }}
            >
              Sign in
            </Link>
          </Typography>
        </Box>
      </Box>
    </Box>
  );
};
export default RegisterPage;
