import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';
import { useLocation } from 'react-router-dom';
import Spinner from 'react-bootstrap/Spinner';

function ProtectedRoute({ setSavedPage, children }) {
  const { user, loading } = useAuth();
  const location = useLocation();
  
  if (loading) return <Spinner className="m-5" />;
  if (!user) {
    setSavedPage(location.pathname);
    return <Navigate to="/login" />;
  }
  return children;
}

export default ProtectedRoute;