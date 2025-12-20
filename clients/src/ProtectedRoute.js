import React, { useState, useEffect } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from './AuthContext';

function ProtectedRoute({ setSavedPage, children }) {
  const { user, loading } = useAuth();
  const location = useLocation();
  const [verified, setVerified] = useState(null);
  const [client, setClient] = useState(null);

  useEffect(() => {
    setVerified(null)
    if (user) {
      user.getIdToken().then(token => {
        fetch('https://www.client.acresbyisaac.com/api/get-verified', {
          method: 'GET',
          headers: {
            'Authorization': 'Bearer ' + token
          }
        })
        .then(response => {
          return response.json();
        })
        .then(data => {
          if (data === "true") {
            setVerified(true);
            setClient(true);
          } else if (data === "Not Verified") {
            setVerified(false);
            setClient(false);
          } else if (data === "No Client") {
            setVerified(true);
            setClient(false);
          } else {
            setVerified(false);
            setClient(false);
          }
        })
        .catch(error => {
          console.error('Error fetching Data:', error);
          setVerified(false);
        });
      });
    }
  }, [user, location]);

  if (loading) {
    return <div>Loading...</div>;
  }
  if (!user) {
    setSavedPage(location.pathname);
    return <Navigate to="/login" />;
  }
  if (verified === null || client === null) {
    return <div>Loading</div>;
  }
  if (!verified && location.pathname !== "/verify") {
    return <Navigate to="/verify" />;
  }
  if(verified && location.pathname === "/verify") {
    return <Navigate to="/" />;
  }
  if (!client && location.pathname !== "/client-info" && location.pathname !== "/verify") {
    return <Navigate to="client-info" />;
  }
  return children;
}

export default ProtectedRoute;