import React, { useState } from 'react';
import { auth } from '../firebase';
import { createUserWithEmailAndPassword, updateProfile } from 'firebase/auth';
import './CreateAccount.css';

const CreateAccount = () => {
  const [formData, setFormData] = useState({
    displayName: '',
    email: '',
    password: '',
    confirmPassword: ''
  });
  const [error, setError] = useState('');
  const [passwordError, setPasswordError] = useState('');
  const [success, setSuccess] = useState('');

  // Function to validate the password
  const validatePassword = (password) => {
    // This regex checks for at least one lowercase letter, one uppercase letter, and one digit.
    const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/;
    return regex.test(password);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    
    // Update form data
    setFormData(prevData => ({
      ...prevData,
      [name]: value,
    }));

    // Validate password in real-time
    if (name === 'password') {
      if (!validatePassword(value)) {
        setPasswordError('Password must contain at least one uppercase letter, one lowercase letter, and one number.');
      } else {
        setPasswordError('');
      }
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    // Check if passwords match
    if (formData.password !== formData.confirmPassword) {
      setError('Passwords do not match.');
      return;
    }

    // Ensure password meets criteria
    if (!validatePassword(formData.password)) {
      setError('Password does not meet the required criteria.');
      return;
    }

    setError('');
    try {
      // Create the user in Firebase
      const userCredential = await createUserWithEmailAndPassword(
        auth,
        formData.email,
        formData.password
      );
      // Update the user's display name
      await updateProfile(userCredential.user, {
        displayName: formData.displayName
      });
      setSuccess('Account created successfully!');
    } catch (err) {
      setError(err.message);
    }
  };

  if (user) {
    return <Navigate to={"/"} />
  }
  
  return (
    <div className="create-account-container">
      <h2>Create Account</h2>
      {error && <p className="error">{error}</p>}
      {passwordError && <p className="error">{passwordError}</p>}
      {success && <p className="success">{success}</p>}
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="displayName">Name:</label>
          <input
            type="text"
            id="displayName"
            name="displayName"
            value={formData.displayName}
            onChange={handleChange}
            required
          />
        </div>
        <div className="form-group">
          <label htmlFor="email">Email:</label>
          <input 
            type="email" 
            id="email" 
            name="email"
            value={formData.email} 
            onChange={handleChange} 
            required 
          />
        </div>
        <div className="form-group">
          <label htmlFor="password">Password:</label>
          <input 
            type="password" 
            id="password" 
            name="password"
            value={formData.password} 
            onChange={handleChange} 
            required 
          />
          <small>
            Password must contain at least one uppercase letter, one lowercase letter, and one number.
          </small>
        </div>
        <div className="form-group">
          <label htmlFor="confirmPassword">Confirm Password:</label>
          <input 
            type="password" 
            id="confirmPassword" 
            name="confirmPassword"
            value={formData.confirmPassword} 
            onChange={handleChange} 
            required 
          />
        </div>
        <button type="submit">Create Account</button>
      </form>
    </div>
  );
};

export default CreateAccount;