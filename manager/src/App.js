import './App.css';
import { Routes, Route, Link, Navigate } from "react-router-dom";
import React, { useState } from 'react';
import Home from "./Home/Home.js";
import Calendar from "./Calendar/Calendar.js";
import Table from "./Table/Table.js";
import Settings from "./Settings/Settings.js";
import InvoicePage from './InvoicePage/InvoicePage.js';
import { useMediaQuery } from 'react-responsive';
import ProtectedRoute from "./ProtectedRoute.js";
import { AuthProvider } from "./AuthContext.js";
import Login from './Login/Login.js';
import InvoiceNew from './new-invoice/InvoiceNew.js';
import Invoices from './Invoices/Invoices.js'
import ReceiptScanner from './reciepts/reciepts.js';
import ReceiptDetails from './reciepts/ReceiptDetails.js';
import { Dropdown } from 'react-bootstrap'
import ClientDetails from './Clients/ClientDetails.js';
import SalesTaxReport from './SalesTax/SalesTaxReport.js';
import Clients from './Clients/Clients.js';
import JobRequestsBoard from './JobRequests/JobRequestsBoard.js';

function NavIcon({ path }) {
  return (
    <svg className="nav-icon" viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d={path} fill="currentColor" />
    </svg>
  );
}

const navItems = [
  {
    to: "/",
    label: "Home",
    icon: "M12 3l8 7v10a1 1 0 0 1-1 1h-5v-6H10v6H5a1 1 0 0 1-1-1V10l8-7z",
  },
  {
    to: "/calendar",
    label: "Calendar",
    icon: "M7 2h2v2h6V2h2v2h3a1 1 0 0 1 1 1v15a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a1 1 0 0 1 1-1h3V2zm12 8H5v10h14V10z",
  },
  {
    to: "/clients",
    label: "Clients",
    icon: "M16 11a4 4 0 1 0-3.999-4A4 4 0 0 0 16 11zm-8 1a3 3 0 1 0-3-3 3 3 0 0 0 3 3zm8 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4zM8 14c-2.67 0-8 1.34-8 4v2h6v-2c0-1.2.62-2.25 1.67-3.08A8.9 8.9 0 0 1 8 14z",
  },
  {
    to: "/job-requests",
    label: "Job Board",
    icon: "M4 4h7v7H4V4zm9 0h7v4h-7V4zM4 13h7v7H4v-7zm9-1h7v8h-7v-8z",
  },
  {
    to: "/invoice-dashboard",
    label: "Invoices",
    icon: "M6 2h9l5 5v14a1 1 0 0 1-1 1H6a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2zm8 1.5V8h4.5L14 3.5zM8 12h8v2H8v-2zm0 4h8v2H8v-2z",
  },
  {
    to: "/sales-tax",
    label: "Sales Tax",
    icon: "M12 2l4 4-6 6-3-3-5 5 1.5 1.5L7 12l3 3 7.5-7.5L22 12V2h-10zM4 18h16v2H4z",
  },
  {
    to: "/receipts",
    label: "Receipts",
    icon: "M7 2h10a1 1 0 0 1 1 1v18l-3-2-2 2-2-2-2 2-2-2-3 2V3a1 1 0 0 1 1-1zm2 5v2h6V7H9zm0 4v2h6v-2H9z",
  },
  {
    to: "/new-invoice",
    label: "New Invoice",
    icon: "M11 3h2v8h8v2h-8v8h-2v-8H3v-2h8V3z",
  },
];

function App() {
  const [collapsed, setCollapsed] = useState(true);
  const [savedPage, setSavedPage] = useState("")
  const tables = ["invoices", "invoice_items", "locations", "quarterly_information", "users", "receipts", "notifications",  "contractors",
    "contractor_payments",
    "owner_draws",
    "calendar"];
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
        {navItems.map((item) => (
          <Link
            key={item.to}
            to={item.to}
            className="nav-link"
            onClick={item.to === "/calendar" ? toggleSidebar : toggleSidebarSmall}
          >
            <span className="nav-link-content">
              <NavIcon path={item.icon} />
              <span>{item.label}</span>
            </span>
          </Link>
        ))}
        <Dropdown>
            <Dropdown.Toggle variant="secondary" id="tables-dropdown">
              <span className="nav-link-content nav-link-content-inline">
                <NavIcon path="M4 5h16v3H4V5zm0 5h16v4H4v-4zm0 6h16v3H4v-3z" />
                <span>Tables</span>
              </span>
            </Dropdown.Toggle>
          <Dropdown.Menu>
            {tables.map((table) => (
              <Dropdown.Item
                as={Link}
                to={`/table/${table}`}
                key={table}
                onClick={toggleSidebarSmall}
              >
                {table.charAt(0).toUpperCase() + table.slice(1)}
              </Dropdown.Item>
            ))}
          </Dropdown.Menu>
        </Dropdown>
      </nav>
      <AuthProvider>
        <main className={`main-content ${collapsed ? 'collapsed' : ''}`}>
          <Routes>
            <Route path="/login" element={<Login page={ savedPage }/>} />
            <Route path="/" element={<ProtectedRoute setSavedPage={setSavedPage}><Home toggleSidebar={toggleSidebar} collapsed={collapsed} /></ProtectedRoute>} />
            <Route path="/calendar" element={<ProtectedRoute setSavedPage={setSavedPage}><Calendar toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="/clients" element={<ProtectedRoute setSavedPage={setSavedPage}><Clients toggleSidebar={toggleSidebar} collapsed={collapsed} /></ProtectedRoute>} />
            <Route path="/job-requests" element={<ProtectedRoute setSavedPage={setSavedPage}><JobRequestsBoard toggleSidebar={toggleSidebar} collapsed={collapsed} /></ProtectedRoute>} />
            <Route path="/table/clients" element={<Navigate to="/clients" replace />} />
            {tables.map((table) => (
              <Route key={table} path={`/table/${table}`} element={<ProtectedRoute setSavedPage={setSavedPage}><Table page={table} toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            ))}
            <Route path="/settings" element={<ProtectedRoute setSavedPage={setSavedPage}><Settings toggleSidebar={toggleSidebar}/></ProtectedRoute>} />
            <Route path="/invoice" element={<ProtectedRoute setSavedPage={setSavedPage}><InvoicePage /></ProtectedRoute>} />
            <Route path="/invoice-dashboard" element={<ProtectedRoute setSavedPage={setSavedPage}><Invoices toggleSidebar={toggleSidebar} collapsed={collapsed} /></ProtectedRoute>} />
            <Route path="/sales-tax" element={<ProtectedRoute setSavedPage={setSavedPage}><SalesTaxReport toggleSidebar={toggleSidebar} collapsed={collapsed} /></ProtectedRoute>} />
            <Route path="/new-invoice" element={<ProtectedRoute setSavedPage={setSavedPage}><InvoiceNew toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="/clients/:clientId" element={<ProtectedRoute setSavedPage={setSavedPage}><ClientDetails toggleSidebar={toggleSidebar} collapsed={collapsed} /></ProtectedRoute>} />
            <Route path="/receipts" element={<ProtectedRoute setSavedPage={setSavedPage}><ReceiptScanner toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="/receipts/:receiptId" element={<ProtectedRoute setSavedPage={setSavedPage}><ReceiptDetails toggleSidebar={toggleSidebar} collapsed={collapsed}/></ProtectedRoute>} />
            <Route path="/reciepts" element={<Navigate to="/receipts" replace />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </main>
      </AuthProvider>
      </div>
  );
}

export default App;
