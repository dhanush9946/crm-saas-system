import { createBrowserRouter } from 'react-router-dom';
import DashboardLayout from '@app/layouts/DashboardLayout';
import AuthLayout from '@app/layouts/AuthLayout';
import ProtectedRoute from '@features/auth/components/ProtectedRoute';
import { Box, Typography, Button } from '@mui/material';

// -------------------------------------------------------------
// PLACEHOLDER PAGES FOR ARCHITECTURAL SETUP
// -------------------------------------------------------------

const PlaceholderPage = ({ title, description }: { title: string; description: string }) => (
  <Box sx={{ p: 4, display: 'flex', flexDirection: 'column', gap: 2 }}>
    <Typography variant="h4" sx={{ fontWeight: 700 }}>{title}</Typography>
    <Typography variant="body1" color="text.secondary">{description}</Typography>
    <Box sx={{ display: 'flex', gap: 2, mt: 2 }}>
      <Button variant="contained">Primary Action</Button>
      <Button variant="outlined">Secondary Action</Button>
    </Box>
  </Box>
);

const LoginPagePlaceholder = () => {
  // Simple fake login dispatcher to demonstrate route protection in action
  const handleFakeLogin = () => {
    // In real app, this triggers Redux action
    alert('Click "Fake Login" to log in. In a real integration, this will trigger the Auth API.');
  };

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, textAlign: 'center' }}>
      <Typography variant="h5" sx={{ fontWeight: 700 }}>Sign In</Typography>
      <Typography variant="body2" color="text.secondary">Enter credentials to access your CRM tenant</Typography>
      <Box sx={{ mt: 2, display: 'flex', flexDirection: 'column', gap: 2 }}>
        <Button variant="contained" fullWidth onClick={handleFakeLogin}>
          Authenticate Session
        </Button>
      </Box>
    </Box>
  );
};

// -------------------------------------------------------------
// ROUTER ROUTING GRAPH
// -------------------------------------------------------------
export const router = createBrowserRouter([
  // Unauthenticated Auth Shell Routes
  {
    path: '/',
    element: <AuthLayout />,
    children: [
      { path: 'login', element: <LoginPagePlaceholder /> },
      { path: 'register', element: <PlaceholderPage title="Register" description="Register a new multi-tenant organization subscription." /> },
    ],
  },
  // Authenticated Core CRM Shell Routes (Protected)
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <DashboardLayout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <PlaceholderPage title="CRM Dashboard" description="Overview of company performance metrics, active leads, and AI summaries." /> },
      { path: 'customers', element: <PlaceholderPage title="Customers" description="Manage database of accounts, contacts, and customer profile relations." /> },
      { path: 'leads', element: <PlaceholderPage title="Leads" description="Pipeline for tracking and converting hot prospective clients." /> },
      { path: 'deals', element: <PlaceholderPage title="Deals" description="Revenue opportunities mapped across target pipeline stages." /> },
      { path: 'activities', element: <PlaceholderPage title="Activities" description="Logs of tasks, emails, calendar events, and phone call timelines." /> },
      { path: 'analytics', element: <PlaceholderPage title="Analytics" description="Reports of sales quotas, conversions, and historical data projections." /> },
      { path: 'ai', element: <PlaceholderPage title="AI Insights" description="DeepMind-powered predictive lead scoring and deal closure recommendations." /> },
    ],
  },
  // Fallback and Error Routes
  {
    path: '/unauthorized',
    element: <PlaceholderPage title="Unauthorized (403)" description="Your assigned role has insufficient permissions to view this resource." />,
  },
  {
    path: '*',
    element: <PlaceholderPage title="Page Not Found (404)" description="The resource you are looking for does not exist or has been relocated." />,
  },
]);
export default router;
