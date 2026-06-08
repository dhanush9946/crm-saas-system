import {
  Box,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Chip,
  Typography,
  CircularProgress,
} from '@mui/material';

import VisibilityIcon from '@mui/icons-material/Visibility';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';

import type { Customer } from '../types/customer.types';

interface CustomersTableProps {
  customers: Customer[];
  isLoading: boolean;

  onView: (customerId: string) => void;
  onEdit: (customerId: string) => void;
  onDelete: (customerId: string) => void;
}

export default function CustomersTable({
  customers,
  isLoading,
  onView,
  onEdit,
  onDelete,
}: CustomersTableProps) {
  if (isLoading) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          py: 4,
        }}
      >
        <CircularProgress />
      </Box>
    );
  }

  if (customers.length === 0) {
    return (
      <Paper
        sx={{
          p: 3,
          textAlign: 'center',
        }}
      >
        <Typography variant="body1">
          No customers found.
        </Typography>
      </Paper>
    );
  }

  return (
    <TableContainer component={Paper}>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Name</TableCell>
            <TableCell>Industry</TableCell>
            <TableCell>Website</TableCell>
            <TableCell>Status</TableCell>
            <TableCell>Created</TableCell>
            <TableCell align="right">
              Actions
            </TableCell>
          </TableRow>
        </TableHead>

        <TableBody>
          {customers.map((customer) => (
            <TableRow
              key={customer.id}
              hover
            >
              <TableCell>
                {customer.name}
              </TableCell>

              <TableCell>
                {customer.industry || '-'}
              </TableCell>

              <TableCell>
                {customer.website ? (
                  <a
                    href={customer.website}
                    target="_blank"
                    rel="noreferrer"
                  >
                    {customer.website}
                  </a>
                ) : (
                  '-'
                )}
              </TableCell>

              <TableCell>
                <Chip
                  label={customer.status}
                  size="small"
                  color={
                    customer.status === 'Active'
                      ? 'success'
                      : 'default'
                  }
                />
              </TableCell>

              <TableCell>
                {new Date(
                  customer.createdAtUtc,
                ).toLocaleDateString()}
              </TableCell>

              <TableCell align="right">
                <IconButton
                  title="View"
                  onClick={() =>
                    onView(customer.id)
                  }
                >
                  <VisibilityIcon />
                </IconButton>

                <IconButton
                  title="Edit"
                  onClick={() =>
                    onEdit(customer.id)
                  }
                >
                  <EditIcon />
                </IconButton>

                <IconButton
                  title="Delete"
                  color="error"
                  onClick={() =>
                    onDelete(customer.id)
                  }
                >
                  <DeleteIcon />
                </IconButton>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}