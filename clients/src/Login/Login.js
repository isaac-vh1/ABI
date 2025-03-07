import React, { useState } from 'react';
import { signInWithEmailAndPassword } from 'firebase/auth';
import { Navigate } from 'react-router-dom';
import { auth } from '../firebase';


function Login({ page }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleLogin = (e) => {
    e.preventDefault();
    signInWithEmailAndPassword(auth, email, password)
      .then(userCredential => {
        console.log('Logged in:', userCredential.user);
        <Navigate to={"/" + page} />
      })
      .catch(error => {
        console.error('Login error:', error);
      });
  };


  return (
    <div>
      <h2>Login Page</h2>
      <form onSubmit={handleLogin}>
        <div>
          <label>Email:</label>
          <input 
            type="email" 
            value={email} 
            onChange={(e)=> setEmail(e.target.value)} 
          />
        </div>
        <div>
          <label>Password:</label>
          <input 
            type="password" 
            value={password} 
            onChange={(e)=> setPassword(e.target.value)} 
          />
        </div>
        <button type="submit">Log In</button>
      </form>
    </div>
  );
}

export default Login;