import {
  Alert,
  CircularProgress,
  Paper,
  Typography,
  Box,
} from '@mui/material';

import {
  useGetCustomerHistoryQuery,
} from '../api/customersApi';

interface Props {
  customerId: string;
}

export default function CustomerHistoryPanel({
  customerId,
}: Props) {
  const {
    data,
    isLoading,
    isError,
  } = useGetCustomerHistoryQuery({
    customerId,
  });

  if (isLoading) {
    return <CircularProgress />;
  }

  if (isError) {
    return (
      <Alert severity="error">
        Failed to load history.
      </Alert>
    );
  }

  return (
    <Box sx={{ mt: 3 }}>
      <Typography
        variant="h6"
        sx={{ mb: 2 }}
      >
        History
      </Typography>

      {data?.items.map((history, index) => (
        <Paper
          key={index}
          sx={{ p: 2, mb: 2 }}
        >
          <Typography>
            Action: {history.action}
          </Typography>

          <Typography>
            User: {history.userId}
          </Typography>

          <Typography>
            Date:{' '}
            {new Date(
              history.createdAtUtc,
            ).toLocaleString()}
          </Typography>

          <Typography>
            Success:{' '}
            {history.succeeded
              ? 'Yes'
              : 'No'}
          </Typography>
        </Paper>
      ))}
    </Box>
  );
}