import React, { useEffect } from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';
import { useLocation } from 'react-router-dom';
import Spinner from 'react-bootstrap/Spinner';

function ProtectedRoute({ setSavedPage, children }) {
  const { user, loading } = useAuth();
  const location = useLocation();

  useEffect(() => {
    if (!loading && !user) {
      setSavedPage?.(location.pathname);
    }
  }, [loading, location.pathname, setSavedPage, user]);

  if (loading) return <Spinner className="m-5" />;
  if (!user) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }
  return children;
}

export default ProtectedRoute;
