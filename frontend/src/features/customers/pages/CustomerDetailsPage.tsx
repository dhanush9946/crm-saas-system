import {
  Alert,
  Box,
  CircularProgress,
  Paper,
  Typography,
  Divider,
  Button,
} from '@mui/material';

import { useNavigate, useParams } from 'react-router-dom';

import {
  useGetCustomerByIdQuery,
} from '../api/customersApi';

import CustomerHistoryPanel from '../components/CustomerHistoryPanel';

export default function CustomerDetailsPage() {
  const { customerId } = useParams();

  const navigate = useNavigate();

  const {
    data,
    isLoading,
    isError,
  } = useGetCustomerByIdQuery(customerId!);

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

  return (
    <Box>
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          mb: 3,
        }}
      >
        <Typography variant="h4">
          Customer Details
        </Typography>

        <Button
          variant="contained"
          onClick={() =>
            navigate(
              `/customers/${customer.id}/edit`,
            )
          }
        >
          Edit Customer
        </Button>
      </Box>

      <Paper sx={{ p: 3 }}>
        <Typography variant="h6">
          {customer.name}
        </Typography>

        <Divider sx={{ my: 2 }} />

        <Typography>
          <strong>Industry:</strong>{' '}
          {customer.industry ?? '-'}
        </Typography>

        <Typography>
          <strong>Website:</strong>{' '}
          {customer.website ?? '-'}
        </Typography>

        <Typography>
          <strong>Status:</strong>{' '}
          {customer.status}
        </Typography>

        <Typography>
          <strong>Owner User Id:</strong>{' '}
          {customer.ownerUserId ?? '-'}
        </Typography>

        <Typography>
          <strong>Created:</strong>{' '}
          {new Date(
            customer.createdAtUtc,
          ).toLocaleString()}
        </Typography>

        <Typography>
          <strong>Updated:</strong>{' '}
          {customer.updatedAtUtc
            ? new Date(
                customer.updatedAtUtc,
              ).toLocaleString()
            : '-'}
        </Typography>

        //This is the customer history,you can remove if it is not need
        <CustomerHistoryPanel
         customerId={customer.id}
        />
      </Paper>
    </Box>
  );
}