import { z } from 'zod';

export const customerFormSchema = z.object({
  name: z
    .string()
    .trim()
    .min(1, 'Customer name is required.')
    .max(200, 'Customer name cannot exceed 200 characters.'),

  industry: z
    .string()
    .max(100, 'Industry cannot exceed 100 characters.')
    .optional()
    .or(z.literal('')),

  website: z
    .string()
    .url('Please enter a valid website URL.')
    .optional()
    .or(z.literal('')),

  ownerUserId: z
    .string()
    .nullable()
    .optional(),
});

export type CustomerFormValues =
  z.infer<typeof customerFormSchema>;