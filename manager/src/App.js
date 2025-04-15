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
import InvoiceNew from './InvoicePage/InvoiceNew.js';

function App() {
  const [collapsed, setCollapsed] = useState(true);
  const [savedPage, setSavedPage] = useState("")
  const location = useLocation();
  const isCalendarRoute = location.pathname === "/calendar";
  const [tables, setTables] = useState(["clients", "invoices", "invoice_items", "locations", "quarterly_information", "users", "expenses", "notifications"]);
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
        {tables.map((table) => (
          <Link key={table} to={`/table/${table}`} className="nav-link" onClick={toggleSidebarSmall}>{table.charAt(0).toUpperCase() + table.slice(1)}</Link>
        ))}
        <Link to="/invoice-new" className="nav-link" onClick={toggleSidebarSmall}>New Invoice</Link>
        <Link to="/settings" className="nav-link" onClick={toggleSidebarSmall}>Settings</Link>
      </nav>
      <AuthProvider>
        <main className={`main-content ${isCalendarRoute ? "no-collapse-on-calendar" : (collapsed ? 'collapsed' : '')}`}>
          <Routes>
            <Route path="/login" element={<Login page={ savedPage }/>} />
            <Route path="/" element={<ProtectedRoute setSavedPage={setSavedPage}><Home toggleSidebar={toggleSidebar} collapsed={collapsed} /></ProtectedRoute>} />
            <Route path="/calendar" element={<ProtectedRoute setSavedPage={setSavedPage}><Calendar toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            {tables.map((table) => (
              <Route key={table} path={`/table/${table}`} element={<ProtectedRoute setSavedPage={setSavedPage}><Table page={table} toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            ))}
            <Route path="/settings" element={<ProtectedRoute setSavedPage={setSavedPage}><Settings toggleSidebar={toggleSidebar}/></ProtectedRoute>} />
            <Route path="/invoice" element={<ProtectedRoute setSavedPage={setSavedPage}><InvoicePage /></ProtectedRoute>} />
            <Route path="/invoice-new" element={<ProtectedRoute setSavedPage={setSavedPage}><InvoiceNew toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </main>
      </AuthProvider>
      </div>
  );
}

export default App;