import {
  Alert,
  Box,
  CircularProgress,
  Paper,
  Typography,
} from '@mui/material';

import { useNavigate, useParams } from 'react-router-dom';

import CustomerForm from '../components/CustomerForm';

import {
  useGetCustomerByIdQuery,
  useUpdateCustomerMutation,
} from '../api/customersApi';

import type {
  CustomerFormValues,
} from '../validations/customerSchemas';

export default function EditCustomerPage() {
  const { customerId } = useParams();

  const navigate = useNavigate();

  const {
    data,
    isLoading,
    isError,
  } = useGetCustomerByIdQuery(
    customerId!,
  );

  const [updateCustomer, { isLoading: isUpdating }] =
    useUpdateCustomerMutation();

  if (isLoading) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          py: 4,
        }}
      >
        <CircularProgress />
      </Box>
    );
  }

  if (isError || !data?.data) {
    return (
      <Alert severity="error">
        Failed to load customer.
      </Alert>
    );
  }

  const customer = data.data;

  const handleSubmit = async (
    values: CustomerFormValues,
  ) => {
    try {
      await updateCustomer({
        customerId: customer.id,
        body: {
          name: values.name,
          industry: values.industry || null,
          website: values.website || null,
          ownerUserId:
            values.ownerUserId || null,
          rowVersion:
            customer.rowVersion,
        },
      }).unwrap();

      navigate('/customers');
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <Box>
      <Typography
        variant="h4"
        sx={{ mb: 3 }}
      >
        Edit Customer
      </Typography>

      <Paper sx={{ p: 3 }}>
        <CustomerForm
          initialValues={{
            name: customer.name,
            industry:
              customer.industry ?? '',
            website:
              customer.website ?? '',
            ownerUserId:
              customer.ownerUserId ??
              null,
          }}
          submitText="Update Customer"
          isSubmitting={isUpdating}
          onSubmit={handleSubmit}
        />
      </Paper>
    </Box>
  );
}