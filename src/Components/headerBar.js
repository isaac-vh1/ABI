import React from 'react';
import "./Components.css"

export default function headerBar({ page }) {
    return(
        <div>
            <head>
                <title>{page}</title>
            </head>
            <header className="Bar">
                <div className="menu-toggle" onClick={toggleSidebar}><HamburgerMenu collapsed={collapsed} /></div>
                <h1>{text}</h1>
            </header>
      </div>
    )
}