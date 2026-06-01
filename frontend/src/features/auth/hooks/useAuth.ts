import { useSelector } from 'react-redux';
import {
  selectAccessToken,
  selectAuthStatus,
  selectAuthUser,
  selectIsAuthenticated,
  selectTenantId,
} from '../store/authSlice';

export function useAuth() {
  const status = useSelector(selectAuthStatus);
  const isAuthenticated = useSelector(selectIsAuthenticated);
  const user = useSelector(selectAuthUser);
  const accessToken = useSelector(selectAccessToken);
  const tenantId = useSelector(selectTenantId);

  return {
    status,
    isAuthenticated,
    isInitializing: status === 'initializing',
    user,
    accessToken,
    tenantId,
  };
}
