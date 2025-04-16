import './App.css';
import React, { useState } from 'react';
import { Routes, Route, Navigate } from "react-router-dom";
import InvoicePage from "./InvoicePage/InvoicePage.js"
import { AuthProvider } from "./AuthContext.js";
import Login from './Login/Login.js';
import ProtectedRoute from "./ProtectedRoute.js";
import CreateAccount from './CreateAccount/CreateAccount.js';
import VerifyEmail from './CreateAccount/VerifyEmail.js';
import ClientDashboard from './ClientDashboard/ClientDashboard.js';
import ClientInfo from './ClientDashboard/ClientInfo.js';

function App() {
  const [savedPage, setSavedPage] = useState("")
  return (
    <div className="App">
      <AuthProvider>
        <header className="App-header">
          
        </header>
        <main>
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/create-account" element={<CreateAccount />} />
            <Route path="/verify" element={<ProtectedRoute setSavedPage={setSavedPage}><VerifyEmail /></ProtectedRoute>} />
            <Route path="/invoice" element={<ProtectedRoute setSavedPage={setSavedPage}><InvoicePage /></ProtectedRoute>} />
            <Route path="/" element={<ProtectedRoute setSavedPage={setSavedPage}><ClientDashboard /></ProtectedRoute>} />
            <Route path="/client-info" element={<ProtectedRoute setSavedPage={setSavedPage}><ClientInfo /></ProtectedRoute>} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
          <footer><a href="https://www.acresbyisaac.com/privacy-policy">Privacy Policy</a> | <a href='https://www.acresbyisaac.com/terms-and-conditions'>Terms and Conditions</a></footer>
        </main>
      </AuthProvider>
    </div>
  );
}

export default App;
