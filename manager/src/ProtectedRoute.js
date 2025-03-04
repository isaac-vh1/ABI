import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';
import { useLocation } from 'react-router-dom';

function ProtectedRoute({ setSavedPage, children }) {
  const { user, loading } = useAuth();
  const location = useLocation();
  
  if (loading) {
    return <div>Loading...</div>;
  }
  if (!user) {
    // Not logged in → redirect
    setSavedPage(location);
    return <Navigate to="/login" />;
  }
  return children;
}

export default ProtectedRoute;