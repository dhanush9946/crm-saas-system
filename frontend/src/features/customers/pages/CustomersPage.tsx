import { useState } from 'react';
import { Alert, Box, Button, Typography } from '@mui/material';
import { useNavigate } from 'react-router-dom';

import CustomersTable from '../components/CustomersTable';
import CustomerFilters from '../components/CustomerFilters';

import {
  useDeleteCustomerMutation,
  useGetCustomersQuery,
} from '../api/customersApi';

export default function CustomersPage() {
  const navigate = useNavigate();

  const [search, setSearch] = useState('');
  const [pageSize, setPageSize] = useState(10);
  const [page] = useState(1);

  const {
    data,
    isLoading,
    isError,
    error,
  } = useGetCustomersQuery({
    search,
    page,
    pageSize,
  });

  const [deleteCustomer] = useDeleteCustomerMutation();

  const customers = data?.data?.items ?? [];

  const handleDelete = async (customerId: string) => {
    const confirmed = window.confirm(
      'Are you sure you want to delete this customer?',
    );

    if (!confirmed) {
      return;
    }

    try {
      await deleteCustomer(customerId).unwrap();
    } catch (err) {
      console.error(err);
      alert('Failed to delete customer.');
    }
  };

  return (
    <Box>
      {/* Header */}
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          mb: 3,
        }}
      >
        <Typography variant="h4">
          Customers
        </Typography>

        <Button
          variant="contained"
          onClick={() => navigate('/customers/new')}
        >
          Create Customer
        </Button>
      </Box>

      {/* Filters */}
      <CustomerFilters
        search={search}
        pageSize={pageSize}
        onSearchChange={setSearch}
        onPageSizeChange={setPageSize}
        onClear={() => {
          setSearch('');
          setPageSize(10);
        }}
      />

      {/* Error */}
      {isError && (
        <Alert severity="error" sx={{ mb: 2 }}>
          Failed to load customers.
          {error && (
            <Typography variant="body2">
              {JSON.stringify(error)}
            </Typography>
          )}
        </Alert>
      )}

      {/* Table */}
      <CustomersTable
        customers={customers}
        isLoading={isLoading}
        onView={(customerId) =>
          navigate(`/customers/${customerId}`)
        }
        onEdit={(customerId) =>
          navigate(`/customers/${customerId}/edit`)
        }
        onDelete={handleDelete}
      />
    </Box>
  );
}