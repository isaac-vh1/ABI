import React from 'react';
import "./Components.css";
import HamburgerMenu from "./HamburgerMenu"; // Adjust the path to your HamburgerMenu component file
import { Helmet } from 'react-helmet';

export default function HeaderBar({ page, toggleSidebar, collapsed }) {
  return (
    <div>
      <Helmet>
        <title>{page}</title>
      </Helmet>
      <header className="Bar">
        <div className="menu-toggle" onClick={toggleSidebar}>
          <HamburgerMenu collapsed={collapsed} />
        </div>
        <h1>{page}</h1>
      </header>
    </div>
  );
}
