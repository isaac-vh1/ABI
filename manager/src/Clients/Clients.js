import React, { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';

import './Clients.css';

import HeaderBar from '../Components/HeaderBar';
import { useAuth } from '../AuthContext';

const currencyFormatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
  maximumFractionDigits: 2,
});

function formatCurrency(value) {
  return currencyFormatter.format(Number(value || 0));
}

export default function Clients({ toggleSidebar, collapsed }) {
  const { user, loading: authLoading } = useAuth();
  const navigate = useNavigate();
  const [clients, setClients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [search, setSearch] = useState('');

  useEffect(() => {
    if (authLoading) return;
    if (!user) {
      setLoading(false);
      return;
    }

    let cancelled = false;

    async function loadClients() {
      setLoading(true);
      setError('');
      try {
        const token = await user.getIdToken();
        const response = await fetch('/api/manager/clients-list', {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (!response.ok) {
          const body = await response.json().catch(() => ({}));
          throw new Error(body.error || 'Failed to load clients.');
        }
        const payload = await response.json();
        if (!cancelled) {
          setClients(payload);
        }
      } catch (err) {
        if (!cancelled) {
          console.error('Client list failed:', err);
          setError(String(err.message || err));
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    }

    loadClients();

    return () => {
      cancelled = true;
    };
  }, [authLoading, user]);

  const filteredClients = useMemo(() => {
    const term = search.trim().toLowerCase();
    if (!term) return clients;
    return clients.filter((client) =>
      [
        client.name,
        client.email,
        client.phoneNumber,
        client.address,
        client.city,
        client.zipCode,
      ].some((value) => String(value || '').toLowerCase().includes(term))
    );
  }, [clients, search]);

  const handleDelete = async (clientId) => {
    if (!user) return;
    if (!window.confirm('Delete this client record?')) {
      return;
    }
    try {
      const token = await user.getIdToken();
      const response = await fetch('/api/manager/delete', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(['clients', clientId]),
      });
      if (!response.ok) {
        const body = await response.json().catch(() => ({}));
        throw new Error(body.error || 'Failed to delete client.');
      }
      setClients((current) => current.filter((client) => client.id !== clientId));
    } catch (err) {
      console.error('Client delete failed:', err);
      setError(String(err.message || err));
    }
  };

  return (
    <div className="clients-page">
      <HeaderBar page="Clients" toggleSidebar={toggleSidebar} collapsed={collapsed} />

      <section className="clients-hero">
        <div>
          <span className="clients-kicker">Client Management</span>
          <h2>Clients Directory</h2>
          <p>Browse clients with balances, upcoming work, and direct actions without digging through raw tables.</p>
        </div>
        <div className="clients-hero-stat">
          <span>Total Clients</span>
          <strong>{clients.length}</strong>
        </div>
      </section>

      <section className="clients-toolbar">
        <input
          className="clients-search"
          type="text"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Search by name, email, phone, or address"
        />
      </section>

      {loading ? <div className="clients-state">Loading clients...</div> : null}
      {error ? <div className="clients-state clients-error">{error}</div> : null}

      {!loading && !error ? (
        <section className="clients-panel">
          <div className="clients-panel-header">
            <h3>Active Client Records</h3>
            <span>{filteredClients.length} result(s)</span>
          </div>

          {filteredClients.length ? (
            <div className="clients-table-wrap">
              <table className="clients-table">
                <thead>
                  <tr>
                    <th>Client</th>
                    <th>Contact</th>
                    <th>Address</th>
                    <th>Open Balance</th>
                    <th>Jobs</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredClients.map((client) => (
                    <tr key={client.id}>
                      <td>
                        <strong>{client.name}</strong>
                        <span>{client.invoiceCount} invoice(s)</span>
                      </td>
                      <td>
                        <strong>{client.email || 'No email'}</strong>
                        <span>{client.phoneNumber || 'No phone'}</span>
                      </td>
                      <td>
                        <strong>{client.address || 'No address'}</strong>
                        <span>{[client.city, client.zipCode].filter(Boolean).join(', ')}</span>
                      </td>
                      <td>{formatCurrency(client.outstandingBalance)}</td>
                      <td>{client.upcomingJobs}</td>
                      <td>
                        <div className="clients-actions">
                          <button className="clients-action clients-view" onClick={() => navigate(`/clients/${client.id}`)}>
                            Open
                          </button>
                          <button className="clients-action clients-delete" onClick={() => handleDelete(client.id)}>
                            Delete
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <div className="clients-empty">No clients matched this search.</div>
          )}
        </section>
      ) : null}
    </div>
  );
}
