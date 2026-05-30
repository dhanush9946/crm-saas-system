import React, { useEffect } from 'react';
import { useLocation, useNavigate, Link as RouterLink } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { toast } from 'react-hot-toast';
import {
  Box,
  Button,
  CircularProgress,
  InputAdornment,
  Link,
  TextField,
  Typography,
} from '@mui/material';
import { BusinessOutlined, LinkOutlined, PersonOutlineOutlined } from '@mui/icons-material';
import { useCompleteGoogleOnboardingMutation } from '../api/authApi';
import {
  googleOnboardingSchema,
  type GoogleOnboardingFormValues,
} from '../validations/authSchemas';

interface GoogleOnboardingLocationState {
  from?: string;
  google?: {
    idToken?: string;
    email?: string;
    fullName?: string;
  };
}

function buildSlug(value: string): string {
  return value
    .toLowerCase()
    .replace(/[^a-z0-9\s-]/g, '')
    .trim()
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-');
}

export const GoogleOnboardingPage: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const state = location.state as GoogleOnboardingLocationState | null;
  const googleProfile = state?.google;
  const from = state?.from || '/';
  const [completeGoogleOnboarding, { isLoading }] = useCompleteGoogleOnboardingMutation();

  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<GoogleOnboardingFormValues>({
    resolver: zodResolver(googleOnboardingSchema),
    defaultValues: {
      tenantName: '',
      tenantSlug: '',
    },
  });

  const tenantName = watch('tenantName');

  useEffect(() => {
    if (!googleProfile?.idToken) {
      toast.error('Start with Google sign-in before creating a workspace.');
      navigate('/login', { replace: true });
    }
  }, [googleProfile?.idToken, navigate]);

  useEffect(() => {
    if (tenantName) {
      setValue('tenantSlug', buildSlug(tenantName), { shouldValidate: true });
    }
  }, [setValue, tenantName]);

  const onSubmit = async (data: GoogleOnboardingFormValues) => {
    if (!googleProfile?.idToken) {
      toast.error('Google session expired. Please sign in again.');
      navigate('/login', { replace: true });
      return;
    }

    try {
      const response = await completeGoogleOnboarding({
        idToken: googleProfile.idToken,
        tenantName: data.tenantName,
        tenantSlug: data.tenantSlug,
      }).unwrap();

      if (response.success) {
        toast.success('Workspace created and Google account linked.');
        navigate(from, { replace: true });
        return;
      }

      toast.error('Workspace setup failed. Please check the details.');
    } catch (error: any) {
      console.error('Google onboarding error:', error);
      const errorMsg =
        error?.data?.error ||
        error?.data?.message ||
        'Workspace setup failed. Please try another workspace slug.';
      toast.error(errorMsg);
    }
  };

  return (
    <Box sx={{ width: '100%' }}>
      <Box sx={{ mb: 3, textAlign: 'center' }}>
        <Typography variant="h5" sx={{ fontWeight: 700, color: 'text.primary' }}>
          Create Workspace
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 0.5 }}>
          Finish setup for {googleProfile?.email || 'your Google account'}
        </Typography>
      </Box>

      <Box
        sx={{
          mb: 2,
          px: 2,
          py: 1.5,
          borderRadius: 2,
          bgcolor: 'grey.50',
          border: '1px solid',
          borderColor: 'divider',
          display: 'flex',
          alignItems: 'center',
          gap: 1.5,
        }}
      >
        <PersonOutlineOutlined color="action" />
        <Box sx={{ minWidth: 0 }}>
          <Typography variant="body2" sx={{ fontWeight: 600 }} noWrap>
            {googleProfile?.fullName || 'Google user'}
          </Typography>
          <Typography variant="caption" color="text.secondary" noWrap>
            {googleProfile?.email || 'Verified by Google'}
          </Typography>
        </Box>
      </Box>

      <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate sx={{ mt: 1 }}>
        <TextField
          margin="normal"
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
          margin="normal"
          required
          fullWidth
          id="tenantSlug"
          label="Workspace Slug"
          placeholder="e.g. acme-corp"
          error={!!errors.tenantSlug}
          helperText={errors.tenantSlug?.message || 'Used to identify your workspace'}
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

        <Box sx={{ mt: 3, mb: 2 }}>
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
              'Create Workspace'
            )}
          </Button>
        </Box>

        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 2 }}>
          <Typography variant="body2" color="text.secondary">
            Wrong Google account?{' '}
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
              Sign in again
            </Link>
          </Typography>
        </Box>
      </Box>
    </Box>
  );
};

export default GoogleOnboardingPage;
