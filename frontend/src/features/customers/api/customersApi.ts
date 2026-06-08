import { baseApi } from '@shared/services/baseApi';

import type {
  ApiResponse,
  Customer,
  CustomerDetails,
  CustomerHistory,
  CustomerListQuery,
  CreateCustomerRequest,
  UpdateCustomerRequest,
  PagedResult,
} from '../types/customer.types';

export const customersApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    getCustomers: builder.query<
      ApiResponse<PagedResult<Customer>>,
      CustomerListQuery
    >({
      query: (params) => ({
        url: '/customers',
        method: 'GET',
        params,
      }),
      providesTags: ['Customer'],
    }),

    getCustomerById: builder.query<
      ApiResponse<CustomerDetails>,
      string
    >({
      query: (customerId) => ({
        url: `/customers/${customerId}`,
        method: 'GET',
      }),
      providesTags: (_result, _error, customerId) => [
        { type: 'Customer', id: customerId },
      ],
    }),

    createCustomer: builder.mutation<
      ApiResponse<{ customerId: string }>,
      CreateCustomerRequest
    >({
      query: (body) => ({
        url: '/customers',
        method: 'POST',
        body,
      }),
      invalidatesTags: ['Customer'],
    }),

    updateCustomer: builder.mutation<
      ApiResponse<string>,
      {
        customerId: string;
        body: UpdateCustomerRequest;
      }
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

    restoreCustomer: builder.mutation<void, string>({
      query: (customerId) => ({
        url: `/customers/${customerId}/restore`,
        method: 'POST',
      }),
      invalidatesTags: ['Customer'],
    }),

    getCustomerHistory: builder.query<
      PagedResult<CustomerHistory>,
      {
        customerId: string;
        page?: number;
        pageSize?: number;
      }
    >({
      query: ({
        customerId,
        page = 1,
        pageSize = 20,
      }) => ({
        url: `/customers/${customerId}/history`,
        method: 'GET',
        params: {
          page,
          pageSize,
        },
      }),
    }),
  }),
});

export const {
  useGetCustomersQuery,
  useGetCustomerByIdQuery,

  useCreateCustomerMutation,
  useUpdateCustomerMutation,

  useDeleteCustomerMutation,
  useRestoreCustomerMutation,

  useGetCustomerHistoryQuery,
} = customersApi;