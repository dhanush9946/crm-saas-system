import { createTheme } from '@mui/material/styles';

// Premium HSL-based harmonious SaaS color palette (Deep slate, royal blue, emerald green)
export const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#1A56DB', // Royal Blue
      light: '#EBF5FF',
      dark: '#1E429F',
      contrastText: '#FFFFFF',
    },
    secondary: {
      main: '#7C3AED', // Violet / Purple Accent
      light: '#F5F3FF',
      dark: '#6D28D9',
      contrastText: '#FFFFFF',
    },
    background: {
      default: '#F9FAFB', // Cool grey background
      paper: '#FFFFFF',
    },
    text: {
      primary: '#111827', // Dark Slate for readability
      secondary: '#4B5563', // Muted Slate
      disabled: '#9CA3AF',
    },
    success: {
      main: '#10B981', // Emerald
      light: '#D1FAE5',
      dark: '#047857',
    },
    warning: {
      main: '#F59E0B', // Amber
      light: '#FEF3C7',
      dark: '#B45309',
    },
    error: {
      main: '#EF4444', // Rose Red
      light: '#FEE2E2',
      dark: '#B91C1C',
    },
    divider: '#E5E7EB',
  },
  typography: {
    fontFamily: '"Inter", "Roboto", "Helvetica", "Arial", sans-serif',
    h1: {
      fontWeight: 700,
      fontSize: '2.25rem',
      letterSpacing: '-0.025em',
      color: '#111827',
    },
    h2: {
      fontWeight: 700,
      fontSize: '1.875rem',
      letterSpacing: '-0.025em',
      color: '#111827',
    },
    h3: {
      fontWeight: 600,
      fontSize: '1.5rem',
      letterSpacing: '-0.025em',
      color: '#111827',
    },
    h4: {
      fontWeight: 600,
      fontSize: '1.25rem',
      letterSpacing: '-0.015em',
    },
    h5: {
      fontWeight: 600,
      fontSize: '1rem',
    },
    h6: {
      fontWeight: 600,
      fontSize: '0.875rem',
    },
    body1: {
      fontSize: '1rem',
      lineHeight: 1.5,
      color: '#374151',
    },
    body2: {
      fontSize: '0.875rem',
      lineHeight: 1.57,
      color: '#4B5563',
    },
    button: {
      fontWeight: 600,
      textTransform: 'none', // Remove uppercase standard for modern UI
    },
  },
  shape: {
    borderRadius: 8, // Sleek rounded corners for modern card-based layout
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          boxShadow: 'none',
          padding: '8px 16px',
          transition: 'all 0.2s ease-in-out',
          '&:hover': {
            boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)',
            transform: 'translateY(-1px)',
          },
          '&:active': {
            transform: 'translateY(0)',
          },
          '&.MuiButton-containedPrimary': {
            backgroundColor: '#1A56DB',
            '&:hover': {
              backgroundColor: '#1E429F',
            },
          },
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          backgroundImage: 'none',
          boxShadow: '0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1)',
          border: '1px solid #E5E7EB',
          borderRadius: 12,
        },
      },
    },
    MuiTextField: {
      defaultProps: {
        size: 'small',
      },
    },
    MuiOutlinedInput: {
      styleOverrides: {
        root: {
          backgroundColor: '#FFFFFF',
          transition: 'border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out',
          '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
            borderWidth: '1.5px',
          },
        },
      },
    },
  },
});
