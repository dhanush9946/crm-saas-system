import { useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-hot-toast';
import { useLogoutMutation } from '../api/authApi';

export function useLogout() {
  const [logoutMutation, { isLoading }] = useLogoutMutation();
  const navigate = useNavigate();

  const logout = useCallback(async () => {
    try {
      await logoutMutation().unwrap();
      toast.success('Signed out successfully.');
    } catch {
      toast.error('Signed out locally. Server session may still be active.');
    } finally {
      navigate('/login', { replace: true });
    }
  }, [logoutMutation, navigate]);

  return { logout, isLoggingOut: isLoading };
}
