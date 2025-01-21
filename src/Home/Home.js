import React from 'react';
import './home.css';
import HamburgerMenu from '../Components/Components';

function Home({toggleSidebar, collapsed}) {  // Receive toggleSidebar as a prop
  return (
    <div>
      <head>
        <title>Home</title>
      </head>
      <section className="Bar">
      <div className="menu-toggle" onClick={toggleSidebar}><HamburgerMenu collapsed={collapsed} /></div>
        <h1>Welcome to Your Dashboard</h1>
      </section>

      <section className="dashboard-grid">
        <article className="dashboard-column">
          <div className="widget">
            <h3>Widget 1</h3>
            <p>Content goes here...</p>
          </div>
        </article>
        <article className="dashboard-column">
          <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
        </article>
        <article className="dashboard-column">
          <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
        </article><article className="dashboard-column">
          <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
        </article>
        <article className="dashboard-column">
          <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
        </article>
      </section>
      <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
          <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
          <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
          <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
          <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
          <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
          <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
          <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
          <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
    </div>
  );
}

export default Home;