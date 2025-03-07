import './App.css';
import React, { useState } from 'react';
import { Routes, Route, Link } from "react-router-dom";
import InvoicePage from "./InvoicePage/InvoicePage.js"
import { AuthProvider } from "./AuthContext.js";
import Login from './Login/Login.js';
import ProtectedRoute from "./ProtectedRoute.js";
import CreateAccount from './CreateAccount/CreateAccount.js';

function App() {
  const [savedPage, setSavedPage] = useState("")
  return (
    <div className="App">
      <AuthProvider>
        <header className="App-header">
          
        </header>
        <main>
          <Routes>
            <Route path="/login" element={<Login page={ savedPage }/>} />
            <Route path="/create-account" element={<CreateAccount />} />
            <Route path="/invoice" element={<ProtectedRoute setSavedPage={setSavedPage}><InvoicePage /></ProtectedRoute>} />
          </Routes>
        </main>
      </AuthProvider>
    </div>
  );
}

export default App;
