import './App.css';
import { Routes, Route, Link } from "react-router-dom";
import React, { useState } from 'react';
import { useLocation } from "react-router-dom";
import Home from "./Home/Home.js";
import Calendar from "./Calendar/Calendar.js";
import Table from "./Table/Table.js";
import Settings from "./Settings/Settings.js";
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
        <Link to="/invoices" className="nav-link" onClick={toggleSidebarSmall}>Invoices</Link>
        <Link to="/settings" className="nav-link" onClick={toggleSidebarSmall}>Settings</Link>
      </nav>
      <AuthProvider>
        <main className={`main-content ${isCalendarRoute ? "no-collapse-on-calendar" : (collapsed ? 'collapsed' : '')}`}>
          <Routes>
            <Route path="/login" element={<Login setSavedPage={ setSavedPage }/>} />
            <Route path="/" element={<ProtectedRoute page={savedPage}><Home toggleSidebar={toggleSidebar} collapsed={collapsed} /></ProtectedRoute>} />
            <Route path="/calendar" element={<ProtectedRoute><Calendar toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="/clients" element={<ProtectedRoute><Table page="clients"toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="/invoices" element={<ProtectedRoute><Table page="invoices"toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="/settings" element={<ProtectedRoute><Settings toggleSidebar={toggleSidebar}/></ProtectedRoute>} />
          </Routes>
        </main>
      </AuthProvider>
      </div>
  );
}

export default App;