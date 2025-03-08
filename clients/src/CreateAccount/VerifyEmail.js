import React, { useState, useEffect } from 'react';
import './CreateAccount.css';
import { useAuth } from '../AuthContext';
import { Navigate } from 'react-router-dom';

const VerifyEmail = () => {
  const [code, setCode] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const { user, loading } = useAuth();
    useEffect(() => {
        user.getIdToken().then(token => {
            fetch('https://www.client.acresbyisaac.com/api/verify-email', {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + token
                },
            }).catch(error => console.error('Error sending email:', error))
        })
    }, [])

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    try {
        user.getIdToken().then(token => {
            const response = fetch('https://www.client.acresbyisaac.com/api/verify', {
                method: 'POST',
                headers: {
                    'Authorization': 'Bearer ' + token,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ code })
            });
            if (!response.ok) {
                const data = response.json();
                throw new Error(data.error || 'Verification failed. Please try again.');
            }
            if(response == "true") {
                setSuccess('Email verified successfully! Going to Home!');
            } else {
                setError("Wrong Verification Code! Try again!")
            }
        })
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="create-account-container">
      <h2>Verify Your Email</h2>
      {error && <p className="error">{error}</p>}
      {success && <p className="success">{success}</p>}
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="code">Verification Code:</label>
          <input 
            type="text" 
            id="code" 
            name="code"
            value={code}
            onChange={(e) => setCode(e.target.value)}
            required 
          />
        </div>
        <button type="submit">Verify Email</button>
      </form>
    </div>
  );
};

export default VerifyEmail;