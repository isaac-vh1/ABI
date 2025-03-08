import React, { useState, useEffect } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from './AuthContext';

function ProtectedRoute({ setSavedPage, children }) {
  const { user, loading } = useAuth();
  const location = useLocation();
  const [verified, setVerified] = useState(null); // null: not checked, true: verified, false: not verified

  useEffect(() => {
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
          } else {
            setVerified(false);
          }
        })
        .catch(error => {
          console.error('Error fetching Data:', error);
          setVerified(false);
        });
      });
    }
  }, [user]);

  if (loading) {
    return <div>Loading...</div>;
  }

  if (!user) {
    setSavedPage(location.pathname);
    return <Navigate to="/login" />;
  }
  if (verified === null) {
    return <div>Checking verification...</div>;
  }

  if (!verified && location.pathname != "/verify") {
    return <Navigate to="/verify" />;
  }
  if(verified && location.pathname == "/verify") {
    return <Navigate to="/" />;
  }

  return children;
}

export default ProtectedRoute;