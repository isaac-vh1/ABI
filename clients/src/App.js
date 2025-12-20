import './App.css';
import React, { useState } from 'react';
import { Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider } from "./AuthContext.js";
import InvoicePage from "./InvoicePage/InvoicePage.js";
import Login from './Login/Login.js';
import ProtectedRoute from "./ProtectedRoute.js";
import CreateAccount from './CreateAccount/CreateAccount.js';
import VerifyEmail from './CreateAccount/VerifyEmail.js';
import ClientDashboard from './ClientDashboard/ClientDashboard.js';
import ClientInfo from './ClientDashboard/ClientInfo.js';
import EndOfYearSurvey from './Forms/endOfYearSurvey.js';

function PublicLayout({ children }) {
  return (
    <>
      <header className="App-header" />
      <main>{children}</main>
      <footer>
        <a href="https://www.acresbyisaac.com/privacy-policy">Privacy Policy</a> |{" "}
        <a href="https://www.acresbyisaac.com/terms-and-conditions">Terms and Conditions</a>
      </footer>
    </>
  );
}

function AuthLayout({ children }) {
  return (
    <AuthProvider>
      <header className="App-header" />
      <main>{children}</main>
      <footer>
        <a href="https://www.acresbyisaac.com/privacy-policy">Privacy Policy</a> |{" "}
        <a href="https://www.acresbyisaac.com/terms-and-conditions">Terms and Conditions</a>
      </footer>
    </AuthProvider>
  );
}

export default function App() {
  const [savedPage, setSavedPage] = useState("");

  return (
    <div className="App">
      <Routes>
        {/* Public branch */}
        <Route
          path="/end-of-year-survey"
          element={
            <PublicLayout>
              <EndOfYearSurvey />
            </PublicLayout>
          }
        />

        {/* Auth branch */}
        <Route
          path="/login"
          element={
            <AuthLayout>
              <Login />
            </AuthLayout>
          }
        />
        <Route
          path="/create-account"
          element={
            <AuthLayout>
              <CreateAccount />
            </AuthLayout>
          }
        />
        <Route
          path="/verify"
          element={
            <AuthLayout>
              <ProtectedRoute setSavedPage={setSavedPage}>
                <VerifyEmail />
              </ProtectedRoute>
            </AuthLayout>
          }
        />
        <Route
          path="/invoice"
          element={
            <AuthLayout>
              <ProtectedRoute setSavedPage={setSavedPage}>
                <InvoicePage />
              </ProtectedRoute>
            </AuthLayout>
          }
        />
        <Route
          path="/"
          element={
            <AuthLayout>
              <ProtectedRoute setSavedPage={setSavedPage}>
                <ClientDashboard />
              </ProtectedRoute>
            </AuthLayout>
          }
        />
        <Route
          path="/client-info"
          element={
            <AuthLayout>
              <ProtectedRoute setSavedPage={setSavedPage}>
                <ClientInfo />
              </ProtectedRoute>
            </AuthLayout>
          }
        />

        {/* Global fallback */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </div>
  );
}
