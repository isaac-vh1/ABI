import React, { useEffect } from 'react';
import "./Components.css";
import HamburgerMenu from "./HamburgerMenu"; // Adjust the path to your HamburgerMenu component file

export default function HeaderBar({ page, toggleSidebar, collapsed }) {
  useEffect(() => {
    document.title = page;
  }, [page]);

  return (
    <header className="Bar">
      <div className="menu-toggle" onClick={toggleSidebar}>
        <HamburgerMenu collapsed={collapsed} />
      </div>
      <h1>{page}</h1>
    </header>
  );
}
