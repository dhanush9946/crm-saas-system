import { Box, TextField, MenuItem, Button } from '@mui/material';

interface CustomerFiltersProps {
  search: string;
  pageSize: number;

  onSearchChange: (value: string) => void;
  onPageSizeChange: (value: number) => void;
  onClear: () => void;
}

export default function CustomerFilters({
  search,
  pageSize,
  onSearchChange,
  onPageSizeChange,
  onClear,
}: CustomerFiltersProps) {
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: {
          xs: 'column',
          md: 'row',
        },
        gap: 2,
        mb: 3,
      }}
    >
      <TextField
        label="Search Customers"
        value={search}
        onChange={(e) => onSearchChange(e.target.value)}
        fullWidth
      />

      <TextField
        select
        label="Page Size"
        value={pageSize}
        onChange={(e) =>
          onPageSizeChange(Number(e.target.value))
        }
        sx={{
          minWidth: 150,
        }}
      >
        <MenuItem value={10}>10</MenuItem>
        <MenuItem value={20}>20</MenuItem>
        <MenuItem value={50}>50</MenuItem>
        <MenuItem value={100}>100</MenuItem>
      </TextField>

      <Button
        variant="outlined"
        onClick={onClear}
      >
        Clear
      </Button>
    </Box>
  );
}