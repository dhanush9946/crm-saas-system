import { createBrowserRouter } from 'react-router-dom';

import DashboardLayout from '@app/layouts/DashboardLayout';
import AuthLayout from '@app/layouts/AuthLayout';

import ProtectedRoute from '@features/auth/components/ProtectedRoute';
import GuestRoute from '@features/auth/components/GuestRoute';

import LoginPage from '@features/auth/pages/LoginPage';
import RegisterPage from '@features/auth/pages/RegisterPage';

import CustomersPage from '@features/customers/pages/CustomersPage';
import CreateCustomerPage from '@features/customers/pages/CreateCustomerPage';
import EditCustomerPage from '@features/customers/pages/EditCustomerPage';
import CustomerDetailsPage from '@features/customers/pages/CustomerDetailsPage';

import {
  Box,
  Typography,
  Button,
} from '@mui/material';

// -------------------------------------------------------------
// PLACEHOLDER COMPONENT
// -------------------------------------------------------------

const PlaceholderPage = ({
  title,
  description,
}: {
  title: string;
  description: string;
}) => (
  <Box
    sx={{
      p: 4,
      display: 'flex',
      flexDirection: 'column',
      gap: 2,
    }}
  >
    <Typography
      variant="h4"
      sx={{ fontWeight: 700 }}
    >
      {title}
    </Typography>

    <Typography
      variant="body1"
      color="text.secondary"
    >
      {description}
    </Typography>

    <Box
      sx={{
        display: 'flex',
        gap: 2,
        mt: 2,
      }}
    >
      <Button variant="contained">
        Primary Action
      </Button>

      <Button variant="outlined">
        Secondary Action
      </Button>
    </Box>
  </Box>
);

// -------------------------------------------------------------
// ROUTER
// -------------------------------------------------------------

export const router = createBrowserRouter([
  // -----------------------------
  // AUTH ROUTES
  // -----------------------------
  {
    path: '/',
    element: (
      <GuestRoute>
        <AuthLayout />
      </GuestRoute>
    ),
    children: [
      {
        path: 'login',
        element: <LoginPage />,
      },
      {
        path: 'register',
        element: <RegisterPage />,
      },
    ],
  },

  // -----------------------------
  // PROTECTED ROUTES
  // -----------------------------
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <DashboardLayout />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: (
          <PlaceholderPage
            title="CRM Dashboard"
            description="Overview of company performance metrics, active leads, and AI summaries."
          />
        ),
      },

      // ===================================
      // CUSTOMERS
      // ===================================

      {
        path: 'customers',
        element: <CustomersPage />,
      },

      {
        path: 'customers/new',
        element: <CreateCustomerPage />,
      },

    {
    path: 'customers/:customerId',
   element: <CustomerDetailsPage />,
   },

    {
     path: 'customers/:customerId/edit',
     element: <EditCustomerPage />,
    },

      // ===================================
      // LEADS
      // ===================================

      {
        path: 'leads',
        element: (
          <PlaceholderPage
            title="Leads"
            description="Pipeline for tracking and converting hot prospective clients."
          />
        ),
      },

      // ===================================
      // DEALS
      // ===================================

      {
        path: 'deals',
        element: (
          <PlaceholderPage
            title="Deals"
            description="Revenue opportunities mapped across target pipeline stages."
          />
        ),
      },

      // ===================================
      // ACTIVITIES
      // ===================================

      {
        path: 'activities',
        element: (
          <PlaceholderPage
            title="Activities"
            description="Logs of tasks, emails, calendar events, and phone call timelines."
          />
        ),
      },

      // ===================================
      // ANALYTICS
      // ===================================

      {
        path: 'analytics',
        element: (
          <PlaceholderPage
            title="Analytics"
            description="Reports of sales quotas, conversions, and historical data projections."
          />
        ),
      },

      // ===================================
      // AI
      // ===================================

      {
        path: 'ai',
        element: (
          <PlaceholderPage
            title="AI Insights"
            description="DeepMind-powered predictive lead scoring and deal closure recommendations."
          />
        ),
      },
    ],
  },

  // -----------------------------
  // ERROR ROUTES
  // -----------------------------
  {
    path: '/unauthorized',
    element: (
      <PlaceholderPage
        title="Unauthorized (403)"
        description="Your assigned role has insufficient permissions to view this resource."
      />
    ),
  },

  {
    path: '*',
    element: (
      <PlaceholderPage
        title="Page Not Found (404)"
        description="The resource you are looking for does not exist or has been relocated."
      />
    ),
  },
]);

export default router;