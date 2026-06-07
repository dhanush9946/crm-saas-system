# Frontend Module Development Guide

> Purpose: This document explains the exact frontend flow to create any CRM module page. Use Customers as the first example, then repeat the same pattern for Leads, Deals, Activities, Analytics, or any future module.

---

## 1. Current Frontend Flow

The app starts in this order:

```text
index.html
  -> src/main.tsx
    -> src/App.tsx
      -> AppProviders
        -> Redux store
        -> MUI theme
        -> AuthBootstrap
        -> Toast notifications
      -> React Router
        -> AuthLayout for /login and /register
        -> ProtectedRoute + DashboardLayout for private CRM pages
```

Important files:

| File | Purpose |
|------|---------|
| `frontend/src/main.tsx` | Starts the React app and renders `App`. |
| `frontend/src/App.tsx` | Wraps the app with providers and router. |
| `frontend/src/app/providers/AppProviders.tsx` | Provides Redux, MUI theme, auth bootstrap, and toast notifications. |
| `frontend/src/app/store.ts` | Creates the Redux store and registers RTK Query plus auth state. |
| `frontend/src/app/router.tsx` | Maps URLs like `/customers` to page components. |
| `frontend/src/app/layouts/DashboardLayout.tsx` | Sidebar, top bar, logout, user avatar, and `<Outlet />` for protected pages. |
| `frontend/src/shared/services/baseApi.ts` | Shared RTK Query API layer. Adds token, tenant id, device id, and refreshes expired tokens. |

For normal CRM modules, you mostly work inside:

```text
frontend/src/features/<module-name>/
```

Example:

```text
frontend/src/features/customers/
```

---

## 2. Feature Folder Rule

Each module should be a self-contained feature.

Recommended structure:

```text
frontend/src/features/customers/
  api/
    customersApi.ts
  components/
    CustomersTable.tsx
    CustomerForm.tsx
    CustomerDetailsPanel.tsx
    CustomerHistoryPanel.tsx
    CustomerFilters.tsx
  hooks/
    useCustomerFilters.ts
  pages/
    CustomersPage.tsx
    CustomerDetailsPage.tsx
    CreateCustomerPage.tsx
    EditCustomerPage.tsx
  types/
    customer.types.ts
  validations/
    customerSchemas.ts
  readme.md
```

You do not need every file on day one. Start with only what the page needs.

Minimum files for a list page:

```text
types/customer.types.ts
api/customersApi.ts
pages/CustomersPage.tsx
components/CustomersTable.tsx
```

Minimum extra files for create/edit forms:

```text
validations/customerSchemas.ts
components/CustomerForm.tsx
pages/CreateCustomerPage.tsx
pages/EditCustomerPage.tsx
```

---

## 3. Development Order For Any Module

Follow this order every time.

### Step 1: Read The Backend Contract

Before creating frontend files, inspect the backend controller, request DTOs, response DTOs, query object, and enums.

For Customers, reference:

| Backend File | What To Learn |
|--------------|---------------|
| `backend/CRM.API/Controllers/Customers/CustomersController.cs` | Available API endpoints and HTTP methods. |
| `backend/CRM.API/Requests/Customers/CreateCustomerRequestDto.cs` | Fields needed to create a customer. |
| `backend/CRM.API/Requests/Customers/UpdateCustomerRequest.cs` | Fields needed to update a customer. |
| `backend/CRM.API/Responses/Customers/CreateCustomerResponseDto.cs` | Create response shape. |
| `backend/CRM.Application/CRM/Customers/DTOs/CustomerDto.cs` | List item shape. |
| `backend/CRM.Application/CRM/Customers/DTOs/CustomerDetailsDto.cs` | Details page shape. |
| `backend/CRM.Application/Common/Models/PagedResult.cs` | Pagination response shape. |

For Leads, use the same idea:

```text
backend/CRM.API/Controllers/Leads/LeadsController.cs
backend/CRM.API/Requests/Leads/
backend/CRM.Application/CRM/Leads/DTOs/
backend/CRM.Domain/CRM/Enums/LeadSource.cs
backend/CRM.Domain/CRM/Enums/LeadStatus.cs
```

For Deals:

```text
backend/CRM.API/Controllers/Deals/DealsController.cs
backend/CRM.API/Requests/Deals/
backend/CRM.Application/CRM/Deals/DTOs/
backend/CRM.Domain/CRM/Enums/DealStage.cs
```

Goal of this step: know the exact fields, endpoints, methods, and response wrappers before writing React code.

---

### Step 2: Create Frontend Types

Create:

```text
frontend/src/features/customers/types/customer.types.ts
```

Purpose: TypeScript interfaces for API request and response data.

For Customers, the backend shape is:

```ts
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  traceId: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface Customer {
  id: string;
  name: string;
  industry?: string | null;
  website?: string | null;
  status: string;
  ownerUserId?: string | null;
  createdAtUtc: string;
}

export interface CustomerDetails extends Customer {
  updatedAtUtc?: string | null;
  rowVersion: string;
}

export interface CreateCustomerRequest {
  name: string;
  industry?: string | null;
  website?: string | null;
  ownerUserId?: string | null;
}

export interface UpdateCustomerRequest extends CreateCustomerRequest {
  rowVersion: string;
}

export interface CreateCustomerResponse {
  customerId: string;
}

export interface CustomerListQuery {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: string;
}
```

Notes:

| Backend Type | Frontend Type |
|--------------|---------------|
| `Guid` | `string` |
| `DateTime` | `string` |
| `DateTime?` | `string | null` or optional string |
| `string?` | `string | null` or optional string |
| `int` | `number` |
| `decimal` | `number` |

Do not guess enum values. Read backend enum files first.

---

### Step 3: Create RTK Query API File

Create:

```text
frontend/src/features/customers/api/customersApi.ts
```

Purpose: Define how the frontend calls customer backend endpoints.

Reference:

```text
frontend/src/shared/services/baseApi.ts
frontend/src/features/auth/api/authApi.ts
```

Why reference `authApi.ts`? It already shows the correct pattern:

```ts
export const authApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    login: builder.mutation<...>({
      query: (...) => ({ url, method, body }),
    }),
  }),
});
```

Customers API should follow the same `baseApi.injectEndpoints` pattern.

Expected Customer endpoints:

| Frontend Endpoint | Backend Endpoint | Method | Purpose |
|-------------------|------------------|--------|---------|
| `getCustomers` | `/customers` | `GET` | Fetch paginated customer list. |
| `getCustomerById` | `/customers/{customerId}` | `GET` | Fetch details for one customer. |
| `createCustomer` | `/customers` | `POST` | Create a customer. |
| `updateCustomer` | `/customers/{customerId}` | `PUT` | Update a customer. |
| `deleteCustomer` | `/customers/{customerId}` | `DELETE` | Soft-delete or delete customer. |
| `restoreCustomer` | `/customers/{customerId}/restore` | `POST` | Restore deleted customer. |
| `getCustomerHistory` | `/customers/{customerId}/history` | `GET` | Fetch audit history. |

Example pattern:

```ts
import { baseApi } from '@shared/services/baseApi';
import type {
  ApiResponse,
  CreateCustomerRequest,
  CreateCustomerResponse,
  Customer,
  CustomerDetails,
  CustomerListQuery,
  PagedResult,
  UpdateCustomerRequest,
} from '../types/customer.types';

export const customersApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    getCustomers: builder.query<ApiResponse<PagedResult<Customer>>, CustomerListQuery | void>({
      query: (params) => ({
        url: '/customers',
        method: 'GET',
        params,
      }),
      providesTags: ['Customer'],
    }),

    getCustomerById: builder.query<ApiResponse<CustomerDetails>, string>({
      query: (customerId) => `/customers/${customerId}`,
      providesTags: (_result, _error, customerId) => [{ type: 'Customer', id: customerId }],
    }),

    createCustomer: builder.mutation<ApiResponse<CreateCustomerResponse>, CreateCustomerRequest>({
      query: (body) => ({
        url: '/customers',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Customer'],
    }),

    updateCustomer: builder.mutation<
      ApiResponse<string>,
      { customerId: string; body: UpdateCustomerRequest }
    >({
      query: ({ customerId, body }) => ({
        url: `/customers/${customerId}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: (_result, _error, { customerId }) => [
        'Customer',
        { type: 'Customer', id: customerId },
      ],
    }),

    deleteCustomer: builder.mutation<void, string>({
      query: (customerId) => ({
        url: `/customers/${customerId}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['Customer'],
    }),
  }),
});

export const {
  useGetCustomersQuery,
  useGetCustomerByIdQuery,
  useCreateCustomerMutation,
  useUpdateCustomerMutation,
  useDeleteCustomerMutation,
} = customersApi;
```

Important: `baseApi.ts` already handles auth token, tenant id, device id, refresh token, and API base URL. Do not manually add authorization headers in feature API files.

---

### Step 4: Create Validation Schemas

Create:

```text
frontend/src/features/customers/validations/customerSchemas.ts
```

Purpose: Validate form inputs before sending API requests.

Reference:

```text
frontend/src/features/auth/validations/authSchemas.ts
```

Use this when you create or edit data.

Example:

```ts
import { z } from 'zod';

export const customerFormSchema = z.object({
  name: z.string().min(1, 'Customer name is required.'),
  industry: z.string().optional(),
  website: z.string().url('Website must be a valid URL.').optional().or(z.literal('')),
  ownerUserId: z.string().optional().nullable(),
});

export type CustomerFormValues = z.infer<typeof customerFormSchema>;
```

Validation should match backend validators as closely as possible. Frontend validation is for user experience. Backend validation is still the final authority.

---

### Step 5: Create Small Reusable Components

Create only the components needed by the page.

For Customers list page:

```text
frontend/src/features/customers/components/CustomersTable.tsx
frontend/src/features/customers/components/CustomerFilters.tsx
```

For create/edit:

```text
frontend/src/features/customers/components/CustomerForm.tsx
```

For details/history:

```text
frontend/src/features/customers/components/CustomerDetailsPanel.tsx
frontend/src/features/customers/components/CustomerHistoryPanel.tsx
```

Purpose of each component:

| Component | Purpose |
|-----------|---------|
| `CustomersTable.tsx` | Displays customer list, status, owner, created date, and actions. |
| `CustomerFilters.tsx` | Search, status filter, industry filter, page size selector. |
| `CustomerForm.tsx` | Shared form for create and edit pages. |
| `CustomerDetailsPanel.tsx` | Shows one customer's full information. |
| `CustomerHistoryPanel.tsx` | Shows audit/history records. |

Rule: pages should decide data fetching and navigation. Components should mostly receive props and render UI.

Good:

```ts
<CustomersTable
  customers={customers}
  isLoading={isLoading}
  onEdit={handleEdit}
  onDelete={handleDelete}
/>
```

Avoid putting all page logic, API calls, dialogs, and table rendering into one huge component.

---

### Step 6: Create Page Components

Pages live here:

```text
frontend/src/features/customers/pages/
```

Recommended pages:

| Page | Route | Purpose |
|------|-------|---------|
| `CustomersPage.tsx` | `/customers` | List, search, filters, pagination, create button. |
| `CustomerDetailsPage.tsx` | `/customers/:customerId` | View one customer and history. |
| `CreateCustomerPage.tsx` | `/customers/new` | Create form. |
| `EditCustomerPage.tsx` | `/customers/:customerId/edit` | Edit form. |

`CustomersPage.tsx` should usually:

1. Store filter and pagination state.
2. Call `useGetCustomersQuery(...)`.
3. Show loading state.
4. Show error state.
5. Render filters and table.
6. Navigate to details, edit, and create pages.
7. Trigger delete/restore mutations if needed.

Example flow:

```text
CustomersPage
  -> useGetCustomersQuery({ page, pageSize, search, status })
  -> CustomerFilters
  -> CustomersTable
  -> Create/Edit/Delete actions
```

`CreateCustomerPage.tsx` should usually:

1. Render `CustomerForm`.
2. Call `useCreateCustomerMutation`.
3. On success, show toast.
4. Navigate to `/customers` or `/customers/:customerId`.

`EditCustomerPage.tsx` should usually:

1. Read `customerId` from route params.
2. Call `useGetCustomerByIdQuery(customerId)`.
3. Fill `CustomerForm` with current values.
4. Submit with `useUpdateCustomerMutation`.
5. Include `rowVersion` in the update request.

---

### Step 7: Wire Routes

Update:

```text
frontend/src/app/router.tsx
```

Purpose: replace placeholder route with real pages.

Current route:

```tsx
{ path: 'customers', element: <PlaceholderPage title="Customers" description="..." /> },
```

After creating pages, change it to:

```tsx
import CustomersPage from '@features/customers/pages/CustomersPage';
import CustomerDetailsPage from '@features/customers/pages/CustomerDetailsPage';
import CreateCustomerPage from '@features/customers/pages/CreateCustomerPage';
import EditCustomerPage from '@features/customers/pages/EditCustomerPage';
```

Then routes:

```tsx
{ path: 'customers', element: <CustomersPage /> },
{ path: 'customers/new', element: <CreateCustomerPage /> },
{ path: 'customers/:customerId', element: <CustomerDetailsPage /> },
{ path: 'customers/:customerId/edit', element: <EditCustomerPage /> },
```

All these are already protected because they are children of:

```tsx
<ProtectedRoute>
  <DashboardLayout />
</ProtectedRoute>
```

So do not wrap each customer page again with `ProtectedRoute`.

---

### Step 8: Wire Sidebar Only If Needed

Update:

```text
frontend/src/app/layouts/DashboardLayout.tsx
```

Customers, Leads, Deals, Activities, Analytics, and AI are already in the sidebar.

Only edit `DashboardLayout.tsx` when:

1. You add a brand-new module not already shown in the sidebar.
2. You need better active state for nested routes.

Current active logic:

```ts
const isActive = location.pathname === item.path;
```

For nested customer routes like `/customers/new` or `/customers/:id/edit`, this exact match will not highlight Customers. Improve it like this:

```ts
const isActive =
  item.path === '/'
    ? location.pathname === '/'
    : location.pathname.startsWith(item.path);
```

This is useful after adding nested pages.

---

### Step 9: Use The Existing UI System

Reference:

```text
frontend/src/styles/theme.ts
frontend/src/app/layouts/DashboardLayout.tsx
frontend/src/features/auth/pages/LoginPage.tsx
frontend/src/features/auth/pages/RegisterPage.tsx
```

Use MUI components:

| UI Need | MUI Component |
|---------|---------------|
| Page layout | `Box`, `Stack`, `Grid`, `Container` |
| Text | `Typography` |
| Buttons | `Button`, `IconButton` |
| Forms | `TextField`, `Select`, `Autocomplete`, `Checkbox` |
| Tables | `Table`, `DataGrid` if installed later |
| Dialogs | `Dialog`, `DialogTitle`, `DialogContent`, `DialogActions` |
| Status | `Chip` |
| Loading | `CircularProgress`, `Skeleton` |
| Errors | `Alert` |

Use `react-hot-toast` for success/error notifications:

```ts
toast.success('Customer created successfully.');
toast.error('Unable to create customer.');
```

Keep module pages quiet and operational. CRM screens should be easy to scan, not like marketing landing pages.

---

### Step 10: Verify

After implementation, run:

```text
cd frontend
npm run build
npm run lint
```

Manual browser checks:

1. Login works.
2. `/customers` loads inside dashboard layout.
3. Sidebar highlights Customers.
4. List loading state appears.
5. Empty state appears when no data exists.
6. Create form validates required fields.
7. Create mutation sends correct body.
8. Edit form includes `rowVersion`.
9. Delete refreshes the list.
10. 401 token refresh still works through `baseApi.ts`.

---

## 4. Customer Module Worked Example

If you start Customers now, create files in this order:

```text
1. frontend/src/features/customers/types/customer.types.ts
2. frontend/src/features/customers/api/customersApi.ts
3. frontend/src/features/customers/validations/customerSchemas.ts
4. frontend/src/features/customers/components/CustomersTable.tsx
5. frontend/src/features/customers/components/CustomerFilters.tsx
6. frontend/src/features/customers/components/CustomerForm.tsx
7. frontend/src/features/customers/pages/CustomersPage.tsx
8. frontend/src/features/customers/pages/CreateCustomerPage.tsx
9. frontend/src/features/customers/pages/EditCustomerPage.tsx
10. frontend/src/features/customers/pages/CustomerDetailsPage.tsx
11. frontend/src/app/router.tsx
12. frontend/src/app/layouts/DashboardLayout.tsx only if nested active sidebar state is needed
```

Why this order?

| Order | File | Why First/Next |
|-------|------|----------------|
| 1 | `customer.types.ts` | Every API and component needs correct data shapes. |
| 2 | `customersApi.ts` | Pages need generated hooks like `useGetCustomersQuery`. |
| 3 | `customerSchemas.ts` | Forms need validation types. |
| 4-6 | Components | Build reusable UI blocks before page composition. |
| 7-10 | Pages | Pages combine API hooks, state, navigation, and components. |
| 11 | `router.tsx` | Route pages only after they exist. |
| 12 | `DashboardLayout.tsx` | Sidebar is global; touch it only when necessary. |

---

## 5. Backend To Frontend Mapping For Customers

### Create Customer

Backend:

```text
POST /api/v1/customers
```

Request:

```ts
{
  name: string;
  industry?: string | null;
  website?: string | null;
  ownerUserId?: string | null;
}
```

Response:

```ts
{
  success: boolean;
  data: {
    customerId: string;
  };
  traceId: string;
}
```

Frontend files:

```text
types/customer.types.ts
api/customersApi.ts
validations/customerSchemas.ts
components/CustomerForm.tsx
pages/CreateCustomerPage.tsx
```

### List Customers

Backend:

```text
GET /api/v1/customers
```

Response:

```ts
{
  success: boolean;
  data: {
    items: Customer[];
    page: number;
    pageSize: number;
    totalCount: number;
  };
  traceId: string;
}
```

Frontend files:

```text
types/customer.types.ts
api/customersApi.ts
components/CustomersTable.tsx
components/CustomerFilters.tsx
pages/CustomersPage.tsx
```

### Customer Details

Backend:

```text
GET /api/v1/customers/{customerId}
```

Frontend files:

```text
api/customersApi.ts
pages/CustomerDetailsPage.tsx
components/CustomerDetailsPanel.tsx
```

### Update Customer

Backend:

```text
PUT /api/v1/customers/{customerId}
```

Important: update requires `rowVersion`.

Frontend files:

```text
api/customersApi.ts
components/CustomerForm.tsx
pages/EditCustomerPage.tsx
```

### Delete Customer

Backend:

```text
DELETE /api/v1/customers/{customerId}
```

Frontend files:

```text
api/customersApi.ts
components/CustomersTable.tsx
pages/CustomersPage.tsx
```

### Customer History

Backend:

```text
GET /api/v1/customers/{customerId}/history?page=1&pageSize=20
```

Frontend files:

```text
api/customersApi.ts
components/CustomerHistoryPanel.tsx
pages/CustomerDetailsPage.tsx
```

---

## 6. Reuse This For Any CRM Module

Use this generic checklist for every new module.

### Generic Feature Folder

```text
frontend/src/features/<module>/
  api/
    <module>Api.ts
  components/
    <Module>Table.tsx
    <Module>Form.tsx
    <Module>Filters.tsx
  pages/
    <Module>Page.tsx
    <Module>DetailsPage.tsx
    Create<Module>Page.tsx
    Edit<Module>Page.tsx
  types/
    <module>.types.ts
  validations/
    <module>Schemas.ts
```

### Generic Build Order

```text
1. Backend controller
2. Backend request DTOs
3. Backend response DTOs
4. Backend enums
5. Frontend types
6. Frontend API endpoints
7. Validation schemas
8. Reusable components
9. Page components
10. Router entries
11. Sidebar entry if new module
12. Build, lint, browser test
```

### Generic Route Pattern

```tsx
{ path: '<module>', element: <<Module>Page /> },
{ path: '<module>/new', element: <Create<Module>Page /> },
{ path: '<module>/:<module>Id', element: <<Module>DetailsPage /> },
{ path: '<module>/:<module>Id/edit', element: <Edit<Module>Page /> },
```

Examples:

```tsx
{ path: 'leads', element: <LeadsPage /> },
{ path: 'leads/new', element: <CreateLeadPage /> },
{ path: 'leads/:leadId', element: <LeadDetailsPage /> },
{ path: 'leads/:leadId/edit', element: <EditLeadPage /> },
```

```tsx
{ path: 'deals', element: <DealsPage /> },
{ path: 'deals/new', element: <CreateDealPage /> },
{ path: 'deals/:dealId', element: <DealDetailsPage /> },
{ path: 'deals/:dealId/edit', element: <EditDealPage /> },
```

---

## 7. What To Put Where

| Code Type | Correct Folder | Example |
|-----------|----------------|---------|
| Page routed by URL | `features/<module>/pages` | `CustomersPage.tsx` |
| Table used only by one module | `features/<module>/components` | `CustomersTable.tsx` |
| Form used only by one module | `features/<module>/components` | `CustomerForm.tsx` |
| API endpoints for one module | `features/<module>/api` | `customersApi.ts` |
| TypeScript types for one module | `features/<module>/types` | `customer.types.ts` |
| Zod schemas for one module | `features/<module>/validations` | `customerSchemas.ts` |
| Reusable button/modal used by many modules | `shared/components` | `ConfirmDialog.tsx` |
| Reusable date formatter | `shared/utils` | `formatDate.ts` |
| Global route map | `app/router.tsx` | `/customers` route |
| Global sidebar layout | `app/layouts/DashboardLayout.tsx` | Customers nav item |
| Global API auth/refresh logic | `shared/services/baseApi.ts` | token refresh |

Rule: if only Customers uses it, keep it inside `features/customers`. If Customers and Leads both need it, move it to `shared`.

---

## 8. Import Rules

Use aliases instead of long relative imports.

Good:

```ts
import { baseApi } from '@shared/services/baseApi';
import CustomersPage from '@features/customers/pages/CustomersPage';
import type { RootState } from '@app/store';
```

Avoid:

```ts
import { baseApi } from '../../../shared/services/baseApi';
```

Aliases are configured in:

```text
frontend/tsconfig.app.json
frontend/vite.config.ts
```

Available aliases:

```text
@app/*
@shared/*
@features/*
@styles/*
```

---

## 9. Common Mistakes To Avoid

1. Do not put feature API endpoints in `baseApi.ts`. Use `baseApi.injectEndpoints` inside the feature.
2. Do not manually attach auth headers. `baseApi.ts` already does that.
3. Do not create one giant `CustomersPage.tsx` with table, filters, form, dialogs, and API logic all mixed together.
4. Do not import Customers code into Leads or Deals. Move shared logic to `shared`.
5. Do not forget `rowVersion` when updating Customers, Leads, or Deals if backend requires it.
6. Do not add route entries before page files exist.
7. Do not edit `DashboardLayout.tsx` for every page. Only edit it for sidebar navigation or layout behavior.
8. Do not trust frontend validation alone. Backend remains the final authority.
9. Do not hardcode `/api/v1` inside feature APIs. Use paths like `/customers` because `baseApi.ts` already has the base URL.
10. Do not ignore loading, error, empty, and success states.

---

## 10. Final Checklist Before Saying A Module Is Done

Use this checklist for every CRM module:

```text
[ ] Types match backend DTOs.
[ ] API file uses baseApi.injectEndpoints.
[ ] Query endpoints use providesTags.
[ ] Mutation endpoints use invalidatesTags.
[ ] Forms use react-hook-form and zod.
[ ] Page has loading state.
[ ] Page has error state.
[ ] Page has empty state.
[ ] Create action shows success/error toast.
[ ] Edit action sends rowVersion when required.
[ ] Delete action refreshes list.
[ ] Routes are added in app/router.tsx.
[ ] Sidebar item exists or was added.
[ ] Nested route active state works.
[ ] npm run build passes.
[ ] npm run lint passes.
[ ] Manual browser test passes.
```

---

## 11. Best Reference Files In This Project

Use these as examples while building:

| File | Why It Helps |
|------|--------------|
| `frontend/src/features/auth/api/authApi.ts` | Best example of `baseApi.injectEndpoints`. |
| `frontend/src/features/auth/pages/LoginPage.tsx` | Best example of form + RTK Query mutation + toast + navigation. |
| `frontend/src/features/auth/pages/RegisterPage.tsx` | Best example of a larger form with generated field value. |
| `frontend/src/features/auth/validations/authSchemas.ts` | Best example of Zod schemas. |
| `frontend/src/app/router.tsx` | Where to register new pages. |
| `frontend/src/app/layouts/DashboardLayout.tsx` | Sidebar and protected page shell. |
| `frontend/src/shared/services/baseApi.ts` | Authenticated API foundation. |
| `frontend/src/styles/theme.ts` | UI colors, typography, and MUI component defaults. |
| `backend/CRM.API/Controllers/Customers/CustomersController.cs` | Customer endpoint source of truth. |
| `backend/CRM.Application/CRM/Customers/DTOs/CustomerDto.cs` | Customer list response source of truth. |
| `backend/CRM.Application/CRM/Customers/DTOs/CustomerDetailsDto.cs` | Customer details response source of truth. |

---

## 12. Short Mental Model

When you build a module, think like this:

```text
Backend DTOs tell me the data shape.
Frontend types copy that shape safely.
API file connects React to backend endpoints.
Validation file protects form input.
Components render small pieces of UI.
Pages combine API, state, components, and navigation.
Router makes pages reachable by URL.
DashboardLayout makes pages visible inside the CRM shell.
```

If you follow that order, you can build Customers first, then repeat the same process for Leads, Deals, Activities, Analytics, and any future CRM module.
