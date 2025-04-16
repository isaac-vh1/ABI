import React, { useEffect, useState } from 'react';
import './ClientDashboard.css'; // Optional CSS for styling
import { auth } from '../firebase';
import { useNavigate } from 'react-router-dom';

const ClientDashboard = () => {
  const [invoices, setInvoices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const user = auth.currentUser;

  useEffect(() => {
    const fetchInvoices = async () => {
      try {
        user.getIdToken().then(token => {
          fetch('https://www.client.acresbyisaac.com/api/client/invoices', {
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
          }).then ((data => {
            setInvoices(data);
          }))
        });
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchInvoices();
  }, []);

  if (loading) {
    return <div>Loading invoices...</div>;
  }

  return (
    <div>
    <div className='announcement'> Thank you for signing up! Currently only invoicing is available, however new features are being added daily! If an error is encountered please email support@acresbyisaac.com so it can be addressed promptly</div>
    <div className="client-dashboard">
      <h1>My Invoices</h1>
        <table className="invoice-table">
          <thead>
            <tr>
              <th>Invoice Number</th>
              <th>Date</th>
              <th>Total</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {invoices.map((invoice) => (
              <tr key={invoice.invoice_id} onClick={() => {navigate("/invoice#" + invoice[0])}}>
                <td>{invoice[1]}</td>
                <td>{invoice[2]}</td>
                <td>{invoice[3]}</td>
                <td>{invoice[4]}</td>
              </tr>
            ))}
          </tbody>
        </table>
    </div>
    </div>
  );
};

export default ClientDashboard;