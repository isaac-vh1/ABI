import React, { useState } from 'react';
import { signInWithEmailAndPassword } from 'firebase/auth';
import { Navigate } from 'react-router-dom';
import { auth } from '../firebase';
import '../CreateAccount/CreateAccount.css'; 

function Login({ page }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [userLogged, setUserLogged] = useState(false);
  const [error, setError] = useState('');

  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const userCredential = await signInWithEmailAndPassword(auth, email, password);
      console.log('Logged in:', userCredential.user);
      setUserLogged(true);
    } catch (err) {
      console.error('Login error:', err);
      setError(err.message);
    }
  };

  if (userLogged) {
    return <Navigate to={`/${page}`} />;
  }

  return (
    <div className="create-account-container">
      <h2>Login</h2>
      {error && <p className="error">{error}</p>}
      <form onSubmit={handleLogin}>
        <div className="form-group">
          <label htmlFor="email">Email:</label>
          <input 
            type="email" 
            id="email" 
            name="email"
            value={email} 
            onChange={(e) => setEmail(e.target.value)}
            required 
          />
        </div>
        <div className="form-group">
          <label htmlFor="password">Password:</label>
          <input 
            type="password" 
            id="password" 
            name="password"
            value={password} 
            onChange={(e) => setPassword(e.target.value)}
            required 
          />
        </div>
        <button type="submit">Log In</button>
      </form>
    </div>
  );
}

export default Login;