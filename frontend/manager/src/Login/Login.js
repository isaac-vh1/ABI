import React, { useState } from 'react';
import { signInWithEmailAndPassword, signOut } from 'firebase/auth';
import { Navigate, useNavigate } from 'react-router-dom';
import { useAuth } from '../AuthContext';
import { auth } from '../firebase';
import "./Login.css"


function Login({ page }) {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const { user, loading } = useAuth();
  const navigate = useNavigate()

  const handleLogin = (e) => {
    e.preventDefault();
    const userCredential = signInWithEmailAndPassword(auth, email, password)
      .then(userCredential => {
        console.log('Logged in:', userCredential.user);
        return <Navigate to={"/" + page} />
      })
      .catch(error => {
        console.error('Login error:', error);
      });
      userCredential.user.getIdToken().then(token => {
        fetch('https://www.client.acresbyisaac.com/api/manager/login', {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token,
            },
        }).then(response => {
          return response.json();
        }).then(data => {
          if(data !=="true") {
            signOut(auth)
            alert("You are not authorized to access this page. Please contact the administrator.")
          } else {
            return navigate("/" + page);
          }
        })
        .catch(error => signOut(auth))
      })
  };
  if (user) {
    return <Navigate to={"/" + page} />
  }


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