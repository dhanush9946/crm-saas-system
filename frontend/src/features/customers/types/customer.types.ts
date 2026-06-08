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

export interface UpdateCustomerRequest {
  name: string;
  industry?: string | null;
  website?: string | null;
  ownerUserId?: string | null;
  rowVersion: string;
}

export interface CustomerHistoryChange {
  propertyName: string;
  oldValue?: string | null;
  newValue?: string | null;
}

export interface CustomerHistory {
  action: string;
  userId?: string | null;
  createdAtUtc: string;
  succeeded: boolean;
  failureReason?: string | null;
  changes: CustomerHistoryChange[];
}

export interface CustomerListQuery {
  search?: string;
  sortBy?: string;
  sortDirection?: string;
  page?: number;
  pageSize?: number;
}