import './App.css';
import { Routes, Route, Link } from "react-router-dom";
import React, { useState } from 'react';
import Home from "./Home/Home.js";
import Calendar from "./Calendar/Calendar.js";
import Clients from "./Clients/Clients.js";
import Invoices from "./Invoices/Invoices.js";
import Settings from "./Settings/Settings.js";
import { useMediaQuery } from 'react-responsive';

function App() {
  const [collapsed, setCollapsed] = useState(false);

  // Function to toggle sidebar collapse
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
      {/* Sidebar with dynamic collapse class */}
      <nav className={`sidebar ${collapsed ? 'collapsed' : ''}`}>
        <div className='sidebar-header'>
        <h2>Dashboard</h2>
        <button className="sidebar-close" onClick={toggleSidebar}>X</button>
        </div>
        <Link to="/" className="nav-link" onClick={toggleSidebarSmall}>Home</Link>
        <Link to="/calendar" className="nav-link" onClick={toggleSidebar}>Calendar</Link>
        <Link to="/clients" className="nav-link" onClick={toggleSidebarSmall}>Clients</Link>
        <Link to="/invoices" className="nav-link" onClick={toggleSidebarSmall}>Invoices</Link>
        <Link to="/settings" className="nav-link" onClick={toggleSidebarSmall}>Settings</Link>
      </nav>
      
      {/* Main content */}
      <main className={`main-content ${collapsed ? 'collapsed' : ''}`}>
        <Routes>
          <Route path="/" element={<Home toggleSidebar={toggleSidebar} />} />
          <Route path="/calendar" element={<Calendar toggleSidebar={toggleSidebar} collapsed={collapsed}/>} />
          <Route path="/clients" element={<Clients toggleSidebar={toggleSidebar}/>} />
          <Route path="/invoices" element={<Invoices toggleSidebar={toggleSidebar}/>} />
          <Route path="/settings" element={<Settings toggleSidebar={toggleSidebar}/>} />
        </Routes>
      </main>
    </div>
  );
}

export default App;