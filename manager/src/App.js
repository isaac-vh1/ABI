import './App.css';
import { Routes, Route, Link, Navigate } from "react-router-dom";
import React, { useState } from 'react';
import { useLocation } from "react-router-dom";
import Home from "./Home/Home.js";
import Calendar from "./Calendar/Calendar.js";
import Table from "./Table/Table.js";
import Settings from "./Settings/Settings.js";
import InvoicePage from './InvoicePage/InvoicePage.js';
import { useMediaQuery } from 'react-responsive';
import ProtectedRoute from "./ProtectedRoute.js";
import { AuthProvider } from "./AuthContext.js";
import Login from './Login/Login.js';

function App() {
  const [collapsed, setCollapsed] = useState(true);
  const [savedPage, setSavedPage] = useState("")

  const location = useLocation();
  const isCalendarRoute = location.pathname === "/calendar";
  const toggleSidebar = () => {
    setCollapsed(!collapsed);
  };

  const isSmallScreen = useMediaQuery({ query: '(max-width: 768px)' });

  const toggleSidebarSmall = () => {
    if (isSmallScreen) {
      setCollapsed(!collapsed);
    }
  };

  return (
    <div className="app-container">
      <nav className={`sidebar ${collapsed ? 'collapsed' : ''}`}>
        <div className='sidebar-header'>
          <h2>Dashboard</h2>
        </div>
        <Link to="/" className="nav-link" onClick={toggleSidebarSmall}>Home</Link>
        <Link to="/calendar" className="nav-link" onClick={toggleSidebar}>Calendar</Link>
        <Link to="/clients" className="nav-link" onClick={toggleSidebarSmall}>Clients</Link>
        <Link to="/locations" className="nav-link" onClick={toggleSidebarSmall}>Locations</Link>
        <Link to="/invoices" className="nav-link" onClick={toggleSidebarSmall}>Invoices</Link>
        <Link to="/invoice_items" className="nav-link" onClick={toggleSidebarSmall}>Invoice Items</Link>
        <Link to="/settings" className="nav-link" onClick={toggleSidebarSmall}>Settings</Link>
      </nav>
      <AuthProvider>
        <main className={`main-content ${isCalendarRoute ? "no-collapse-on-calendar" : (collapsed ? 'collapsed' : '')}`}>
          <Routes>
            <Route path="/login" element={<Login page={ savedPage }/>} />
            <Route path="/" element={<ProtectedRoute setSavedPage={setSavedPage}><Home toggleSidebar={toggleSidebar} collapsed={collapsed} /></ProtectedRoute>} />
            <Route path="/calendar" element={<ProtectedRoute setSavedPage={setSavedPage}><Calendar toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="/clients" element={<ProtectedRoute setSavedPage={setSavedPage}><Table page="clients" toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="/invoices" element={<ProtectedRoute setSavedPage={setSavedPage}><Table page="invoices" toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="/invoice_items" element={<ProtectedRoute setSavedPage={setSavedPage}><Table page="invoice_items" toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="/locations" element={<ProtectedRoute setSavedPage={setSavedPage}><Table page="locations"toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="/quarterly-information" element={<ProtectedRoute setSavedPage={setSavedPage}><Table page="quarterly_information" toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="/settings" element={<ProtectedRoute setSavedPage={setSavedPage}><Settings toggleSidebar={toggleSidebar}/></ProtectedRoute>} />
            <Route path="/invoice" element={<ProtectedRoute setSavedPage={setSavedPage}><InvoicePage /></ProtectedRoute>} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </main>
      </AuthProvider>
      </div>
  );
}

export default App;