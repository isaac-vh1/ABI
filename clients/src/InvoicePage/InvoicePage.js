import React, { useState, useEffect } from 'react';
import './InvoicePage.css';
import { useLocation } from 'react-router-dom';
import { auth } from '../firebase';
const user = auth.currentUser;

const InvoicePage = () => {
  const [invoiceData, setInvoiceData] = useState(null);
  const location = useLocation();
  const invoiceNum = location.hash.slice(1);
  const user = auth.currentUser;
  console.log(invoiceNum)
  
  useEffect(() => {
    setInvoiceData(null)
    user.getIdToken().then(token => {
      fetch('https://www.client.acresbyisaac.com/api/invoice/' + invoiceNum, {
        method: 'GET',
        headers: {
          'Authorization': 'Bearer ' + token
        }
      })
      .then(response => {
        if (!response.ok) {
          return response.json().then(err => {
            console.error('Error response:', err);
            throw new Error(err.error);
          });
        }
        return response.json();
      })
      .then(dataAPI => {
        setInvoiceData(dataAPI)
      })
      .catch(error => console.error('Error fetching Data:', error))
    });
  }, [invoiceNum, user]);

  if(!invoiceData || invoiceData === "Invoice not found" || invoiceData === "All Data not found") {
    return (<h1>Error Retrieving Invoice, Please try again</h1>)
  }

  return (
    <div className="container">
      <h1 className="title">Invoice #{invoiceNum}</h1>
      
      <section className="section">
        <h2 className="companyName">Acres By Isaac</h2>
        <p>156 NE 193rd St.</p>
        <p>Shoreline, WA 98155</p>
        <p>(206) 595-5831</p>
      </section>
      
      <section className="section">
        <h3 className="heading">Invoice Details</h3>
        <p><strong>Date of Completion:</strong> {invoiceData[0]}</p>
        <p><strong>Due Date:</strong> {invoiceData[1]}</p>
      </section>
      
      <section className="section">
        <h3 className="heading">Billed To</h3>
        <p>{invoiceData[9]} {invoiceData[10]}</p>
        <p>{invoiceData[11]}</p>
        <p>{invoiceData[12]}, WA {invoiceData[13]}</p>
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
          {invoiceData.slice(14).map((item, index, slicedArray) => {
            if (index % 2 === 0) {
              const description = slicedArray[index];
              const price = slicedArray[index + 1];
              return (
                <tr key={index}>
                  <td className="leftAlign">{description}</td>
                  <td className="rightAlign">{price}</td>
                </tr>
              );
            }
            return null;
          })}
          </tbody>
        </table>
      </section>
      
      <section className="section">
        <div className="totals">
          <p><strong>Subtotal:</strong> ${invoiceData[2]}</p>
          <p><strong>Sales Tax (10.5%):</strong> ${invoiceData[3]}</p>
          <p><strong>Total:</strong> ${invoiceData[2]+invoiceData[3]}</p>
          <p><strong>Balance Due:</strong> ${invoiceData[4]}</p>
          <p><strong>Tips:</strong> ${invoiceData[5]}</p>
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