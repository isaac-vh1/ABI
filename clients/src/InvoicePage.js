import React from 'react';
import './InvoicePage.css';

const InvoicePage = () => {
  return (
    <div className="container">
      <h1 className="title">Invoice #312048</h1>
      
      <section className="section">
        <h2 className="companyName">Acres By Isaac</h2>
        <p className="subHeading">Landscaping</p>
        <p>156 NE 193rd St.</p>
        <p>Shoreline, WA 98155</p>
        <p>(206) 595-5831</p>
      </section>
      
      <section className="section">
        <h3 className="heading">Invoice Details</h3>
        <p><strong>Date of Completion:</strong> Jul 01, 2024</p>
        <p><strong>Due Date:</strong> 8/9/2024</p>
      </section>
      
      <section className="section">
        <h3 className="heading">Billed To</h3>
        <p>Dummy test</p>
        <p>1234 56th st.</p>
        <p>Test, WA 98026</p>
      </section>
      
      <section className="section">
        <h3 className="heading">Items</h3>
        <table className="table">
          <thead>
            <tr>
              <th className="tableHeader">Description</th>
              <th className="tableHeader">Price</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Bush Trimming</td>
              <td className="rightAlign">$300.00</td>
            </tr>
            <tr>
              <td>Initial Mow and Edging</td>
              <td className="rightAlign">$300.00</td>
            </tr>
          </tbody>
        </table>
      </section>
      
      <section className="section">
        <div className="totals">
          <p><strong>Subtotal:</strong> $600.00</p>
          <p><strong>Sales Tax (10.5%):</strong> $63.00</p>
          <p><strong>Total:</strong> $663.00</p>
        </div>
      </section>
      
      <section className="section">
        <h3 className="heading">Payment Methods</h3>
        <ul>
          <li>Apple Pay (206) 595-5831</li>
          <li>Venmo (isaac_vh)</li>
          <li>Cash or Check</li>
        </ul>
      </section>
      
      <section className="section">
        <p className="note">
          Note: A late fee of 20% of the subtotal may apply if payment is not received by the due date.
        </p>
      </section>
    </div>
  );
};

export default InvoicePage;