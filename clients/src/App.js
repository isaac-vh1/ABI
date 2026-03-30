import './App.css';
import React, { useState } from 'react';
import { Routes, Route, Navigate, NavLink } from "react-router-dom";
import { AuthProvider } from "./AuthContext.js";
import InvoicePage from "./InvoicePage/InvoicePage.js";
import Login from './Login/Login.js';
import ProtectedRoute from "./ProtectedRoute.js";
import CreateAccount from './CreateAccount/CreateAccount.js';
import VerifyEmail from './CreateAccount/VerifyEmail.js';
import ClientDashboard from './ClientDashboard/ClientDashboard.js';
import ClientInfo from './ClientDashboard/ClientInfo.js';
import EndOfYearSurvey from './Forms/endOfYearSurvey.js';
import { navItems } from './ClientDashboard/clientDashboardShared';

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

function AuthLayout({ children, showClientNav = false }) {
  return (
    <AuthProvider>
      <header className="App-header" />
      {showClientNav ? (
        <nav className="app-client-nav" aria-label="Client portal sections">
          <div className="app-client-nav-inner">
            {navItems.map((item) => (
              <NavLink
                key={item.key}
                to={item.to}
                end={item.to === '/'}
                className={({ isActive }) => `app-client-nav-link ${isActive ? 'active' : ''}`}
              >
                {item.label}
              </NavLink>
            ))}
          </div>
        </nav>
      ) : null}
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
            <AuthLayout showClientNav>
              <ProtectedRoute setSavedPage={setSavedPage}>
                <ClientDashboard section="overview" />
              </ProtectedRoute>
            </AuthLayout>
          }
        />
        <Route
          path="/client-requests"
          element={
            <AuthLayout showClientNav>
              <ProtectedRoute setSavedPage={setSavedPage}>
                <ClientDashboard section="requests" />
              </ProtectedRoute>
            </AuthLayout>
          }
        />
        <Route
          path="/client-schedule"
          element={
            <AuthLayout showClientNav>
              <ProtectedRoute setSavedPage={setSavedPage}>
                <ClientDashboard section="schedule" />
              </ProtectedRoute>
            </AuthLayout>
          }
        />
        <Route
          path="/client-invoices"
          element={
            <AuthLayout showClientNav>
              <ProtectedRoute setSavedPage={setSavedPage}>
                <ClientDashboard section="invoices" />
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
