import React, { useState, useEffect } from 'react';
import './CreateAccount.css';
import { useAuth } from '../AuthContext';
import { useNavigate, useSearchParams} from 'react-router-dom';

const VerifyEmail = () => {
  const [code, setCode] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const navigate = useNavigate();
  const { user, loading } = useAuth();
  const [searchParams] = useSearchParams();
  setCode(searchParams.get('code'));

  if(code !== ''){
    handleSubmit();
  }

  useEffect(() => {
    const sendVerificationEmail = async () => {
      if (user) {
        try {
          const token = await user.getIdToken();
          const response = await fetch('https://www.client.acresbyisaac.com/api/verify-email', {
            method: 'GET',
            headers: {
              'Authorization': 'Bearer ' + token,
              'Content-Type': 'application/json'
            }
          });
          // Optionally parse the response if needed
          // const data = await response.json();
          setSuccess("Verification Email Sent! (Please do not reload this page)");
        } catch (err) {
          console.error('Error sending email:', err);
          setError("Error sending email, please try again");
        }
      }
    };

    sendVerificationEmail();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    try {
      const token = await user.getIdToken();
      const response = await fetch('https://www.client.acresbyisaac.com/api/verify', {
        method: 'POST',
        headers: {
          'Authorization': 'Bearer ' + token,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ code })
      });
      const data = await response.json();
      console.log(data);
      // Explicitly check if data equals the string "true" (or a boolean true, if that’s what your API returns)
      if (data === "true" || data === true) {
        setSuccess('Email verified successfully! Going to Home!');
        navigate('/invoice#1'); // This should perform client-side navigation if the route exists
      } else {
        setError("Wrong Verification Code! Try again!");
      }
    } catch (err) {
      console.error(err);
      setError(err.message);
    }
  };

  if (loading) {
    return <div>Loading...</div>;
  }

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