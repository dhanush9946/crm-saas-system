import { z } from 'zod';

// Slug regex: lowercase letters, numbers, and hyphens only
const slugRegex = /^[a-z0-9-]+$/;

export const loginSchema = z.object({
  tenantSlug: z
    .string()
    .min(1, 'Workspace slug is required')
    .regex(
      slugRegex,
      'Slug can only contain lowercase letters, numbers, and hyphens'
    ),
  email: z.string().min(1, 'Email is required').email('Invalid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
});

export const registerSchema = z
  .object({
    tenantName: z
      .string()
      .min(3, 'Organization name must be at least 3 characters')
      .max(50, 'Organization name must not exceed 50 characters'),
    tenantSlug: z
      .string()
      .min(1, 'Workspace slug is required')
      .regex(
        slugRegex,
        'Slug can only contain lowercase letters, numbers, and hyphens (no spaces)'
      ),
    displayName: z
      .string()
      .min(2, 'Name must be at least 2 characters')
      .max(50, 'Name must not exceed 50 characters'),
    email: z.string().min(1, 'Email is required').email('Invalid email address'),
    password: z
      .string()
      .min(8, 'Password must be at least 8 characters')
      .regex(/[A-Z]/, 'Password must contain at least one uppercase letter')
      .regex(/[a-z]/, 'Password must contain at least one lowercase letter')
      .regex(/[0-9]/, 'Password must contain at least one number')
      .regex(/[^A-Za-z0-9]/, 'Password must contain at least one special character'),
    confirmPassword: z.string().min(1, 'Please confirm your password'),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: 'Passwords do not match',
    path: ['confirmPassword'],
  });

export type LoginFormValues = z.infer<typeof loginSchema>;
export type RegisterFormValues = z.infer<typeof registerSchema>;
