import { useEffect } from 'react';
import {
  Box,
  Button,
  TextField,
} from '@mui/material';

import {
  useForm,
} from 'react-hook-form';

import {
  zodResolver,
} from '@hookform/resolvers/zod';

import {
  customerFormSchema,
  type CustomerFormValues,
} from '../validations/customerSchemas';

interface CustomerFormProps {
  initialValues?: Partial<CustomerFormValues>;
  isSubmitting?: boolean;
  submitText?: string;

  onSubmit: (
    values: CustomerFormValues,
  ) => Promise<void>;
}

export default function CustomerForm({
  initialValues,
  isSubmitting = false,
  submitText = 'Save',
  onSubmit,
}: CustomerFormProps) {
  const {
    register,
    handleSubmit,
    reset,
    formState: {
      errors,
    },
  } = useForm<CustomerFormValues>({
    resolver: zodResolver(
      customerFormSchema,
    ),
    defaultValues: {
      name: '',
      industry: '',
      website: '',
      ownerUserId: null,
    },
  });

  useEffect(() => {
    if (initialValues) {
      reset({
        name:
          initialValues.name ?? '',
        industry:
          initialValues.industry ?? '',
        website:
          initialValues.website ?? '',
        ownerUserId:
          initialValues.ownerUserId ??
          null,
      });
    }
  }, [initialValues, reset]);

  return (
    <Box
      component="form"
      onSubmit={handleSubmit(onSubmit)}
      sx={{
        display: 'flex',
        flexDirection: 'column',
        gap: 2,
        maxWidth: 600,
      }}
    >
      <TextField
        label="Customer Name"
        {...register('name')}
        error={!!errors.name}
        helperText={
          errors.name?.message
        }
        required
      />

      <TextField
        label="Industry"
        {...register('industry')}
        error={!!errors.industry}
        helperText={
          errors.industry?.message
        }
      />

      <TextField
        label="Website"
        {...register('website')}
        error={!!errors.website}
        helperText={
          errors.website?.message
        }
      />

      <Button
        type="submit"
        variant="contained"
        disabled={isSubmitting}
      >
        {submitText}
      </Button>
    </Box>
  );
}