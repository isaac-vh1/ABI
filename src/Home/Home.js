import React from 'react';

function Home({toggleSidebar}) {  // Receive toggleSidebar as a prop
  return (
    <div>
      <head>
        <title>Home</title>
      </head>
      <section className="Bar">
        <button className="menu-toggle" onClick={toggleSidebar}>☰</button>
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
        </article><article className="dashboard-column">
          <div className="widget">
            <h3>Widget 2</h3>
            <p>Content goes here...</p>
          </div>
        </article>
      </section>
    </div>
  );
}

export default Home;