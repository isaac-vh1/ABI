import React from 'react';
import './home.css';
import HeaderBar from '../Components/HeaderBar';

function Home({toggleSidebar, collapsed}) {
  return (
    <div>
      <HeaderBar page="Home" toggleSidebar={toggleSidebar} collapsed={collapsed} />
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