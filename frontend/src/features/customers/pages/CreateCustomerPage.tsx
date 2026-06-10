import { Box, Typography, Paper } from '@mui/material';
import { useNavigate } from 'react-router-dom';

import CustomerForm from '../components/CustomerForm';

import {
  useCreateCustomerMutation,
} from '../api/customersApi';

import type {
  CustomerFormValues,
} from '../validations/customerSchemas';

export default function CreateCustomerPage() {
  const navigate = useNavigate();

  const [createCustomer, { isLoading }] =
    useCreateCustomerMutation();

  const handleSubmit = async (
    values: CustomerFormValues,
  ) => {
    try {
      await createCustomer({
        name: values.name,
        industry: values.industry || null,
        website: values.website || null,
        ownerUserId:
          values.ownerUserId || null,
      }).unwrap();

      navigate('/customers');
    } catch (error) {
      console.error(
        'Failed to create customer',
        error,
      );
    }
  };

  return (
    <Box>
      <Typography
        variant="h4"
        sx={{ mb: 3 }}
      >
        Create Customer
      </Typography>

      <Paper sx={{ p: 3 }}>
        <CustomerForm
          submitText="Create Customer"
          isSubmitting={isLoading}
          onSubmit={handleSubmit}
        />
      </Paper>
    </Box>
  );
}