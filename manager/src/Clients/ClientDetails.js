import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

import './ClientDetails.css';

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

function formatDate(value) {
  if (!value) return 'Not scheduled';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return 'Invalid date';
  return date.toLocaleString();
}

export default function ClientDetails({ toggleSidebar, collapsed }) {
  const { clientId } = useParams();
  const navigate = useNavigate();
  const { user, loading: authLoading } = useAuth();
  const [payload, setPayload] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);
  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    address: '',
    city: '',
    zipCode: '',
    picturePreference: false,
  });

  useEffect(() => {
    if (authLoading) return;
    if (!user) {
      setLoading(false);
      return;
    }

    let cancelled = false;

    async function loadClientDetails() {
      setLoading(true);
      setError('');
      try {
        const token = await user.getIdToken();
        const year = new Date().getFullYear();
        const response = await fetch(`/api/manager/client-overview/${clientId}?year=${year}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (!response.ok) {
          const body = await response.json().catch(() => ({}));
          throw new Error(body.error || 'Failed to load client overview.');
        }

        const nextPayload = await response.json();
        if (!cancelled) {
          setPayload(nextPayload);
          setForm({
            firstName: nextPayload.client?.firstName || '',
            lastName: nextPayload.client?.lastName || '',
            email: nextPayload.client?.email || '',
            phoneNumber: nextPayload.client?.phoneNumber || '',
            address: nextPayload.client?.address || '',
            city: nextPayload.client?.city || '',
            zipCode: nextPayload.client?.zipCode || '',
            picturePreference: Boolean(nextPayload.client?.picturePreference),
          });
        }
      } catch (err) {
        if (!cancelled) {
          console.error('Client overview failed:', err);
          setError(String(err.message || err));
          setPayload(null);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    }

    loadClientDetails();

    return () => {
      cancelled = true;
    };
  }, [authLoading, clientId, user]);

  const client = payload?.client;
  const summary = payload?.summary || {
    totalSpent: 0,
    outstandingBalance: 0,
    invoiceCount: 0,
    upcomingJobs: 0,
  };
  const invoices = payload?.invoices || [];
  const upcomingJobs = payload?.upcomingJobs || [];

  const handleChange = (event) => {
    const { name, value, type, checked } = event.target;
    setForm((current) => ({
      ...current,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };

  const handleSave = async () => {
    if (!user) return;
    setSaving(true);
    setError('');
    try {
      const token = await user.getIdToken();
      const response = await fetch(`/api/manager/client/${clientId}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(form),
      });
      const result = await response.json();
      if (!response.ok) {
        throw new Error(result.error || 'Failed to save client.');
      }
      setPayload((current) => current ? ({
        ...current,
        client: {
          ...current.client,
          ...form,
          name: `${form.firstName} ${form.lastName}`.trim(),
        },
      }) : current);
    } catch (err) {
      console.error('Client save failed:', err);
      setError(String(err.message || err));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="client-details-page">
      <HeaderBar page="Client Overview" toggleSidebar={toggleSidebar} collapsed={collapsed} />

      <section className="client-details-hero">
        <div>
          <span className="client-details-kicker">Manager View</span>
          <h2>{client?.name || `Client #${clientId}`}</h2>
          <p>
            {client?.email || 'No email on file'}
            {client?.phoneNumber ? ` • ${client.phoneNumber}` : ''}
          </p>
        </div>
        <button className="client-details-back" onClick={() => navigate('/clients')}>
          Back to Clients
        </button>
      </section>

      {loading ? <div className="client-details-state">Loading client overview...</div> : null}
      {error ? <div className="client-details-state client-details-error">{error}</div> : null}

      {!loading && !error && payload ? (
        <>
          <section className="client-details-metrics">
            <article className="client-details-card">
              <span>Total Spent in {payload.year}</span>
              <strong>{formatCurrency(summary.totalSpent)}</strong>
            </article>
            <article className="client-details-card">
              <span>Outstanding Balance</span>
              <strong>{formatCurrency(summary.outstandingBalance)}</strong>
            </article>
            <article className="client-details-card">
              <span>Invoices</span>
              <strong>{summary.invoiceCount}</strong>
            </article>
            <article className="client-details-card">
              <span>Upcoming Jobs</span>
              <strong>{summary.upcomingJobs}</strong>
            </article>
          </section>

          <section className="client-details-grid">
            <article className="client-details-panel">
              <div className="client-details-panel-header">
                <h3>Client Information</h3>
                <span>Edit contact and service address</span>
              </div>
              <div className="client-details-form-grid">
                <label>
                  <span>First Name</span>
                  <input name="firstName" value={form.firstName} onChange={handleChange} />
                </label>
                <label>
                  <span>Last Name</span>
                  <input name="lastName" value={form.lastName} onChange={handleChange} />
                </label>
                <label>
                  <span>Email</span>
                  <input name="email" value={form.email} onChange={handleChange} />
                </label>
                <label>
                  <span>Phone Number</span>
                  <input name="phoneNumber" value={form.phoneNumber} onChange={handleChange} />
                </label>
                <label className="client-details-form-wide">
                  <span>Address</span>
                  <input name="address" value={form.address} onChange={handleChange} />
                </label>
                <label>
                  <span>City</span>
                  <input name="city" value={form.city} onChange={handleChange} />
                </label>
                <label>
                  <span>ZIP Code</span>
                  <input name="zipCode" value={form.zipCode} onChange={handleChange} />
                </label>
                <label className="client-details-checkbox">
                  <input
                    type="checkbox"
                    name="picturePreference"
                    checked={form.picturePreference}
                    onChange={handleChange}
                  />
                  <span>Picture updates preferred</span>
                </label>
              </div>
              <div className="client-details-actions">
                <button className="client-details-inline-button" onClick={handleSave} disabled={saving}>
                  {saving ? 'Saving...' : 'Save Client'}
                </button>
              </div>
            </article>

            <article className="client-details-panel">
              <div className="client-details-panel-header">
                <h3>Invoices</h3>
                <span>{invoices.length} total</span>
              </div>
              {invoices.length ? (
                <div className="client-details-table-wrap">
                  <table className="client-details-table">
                    <thead>
                      <tr>
                        <th>Invoice</th>
                        <th>Issue Date</th>
                        <th>Status</th>
                        <th>Balance</th>
                        <th />
                      </tr>
                    </thead>
                    <tbody>
                      {invoices.map((invoice) => (
                        <tr key={invoice.id}>
                          <td>{invoice.invoiceNumber || `#${invoice.id}`}</td>
                          <td>{formatDate(invoice.issueDate)}</td>
                          <td>{invoice.status || 'Unknown'}</td>
                          <td>{formatCurrency(invoice.balanceDue)}</td>
                          <td>
                            <button
                              className="client-details-inline-button"
                              onClick={() => navigate(`/invoice#${invoice.id}`)}
                            >
                              Open
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              ) : (
                <div className="client-details-empty">No invoices found for this client.</div>
              )}
            </article>

            <article className="client-details-panel">
              <div className="client-details-panel-header">
                <h3>Upcoming Jobs</h3>
                <span>Scheduled work ahead</span>
              </div>
              {upcomingJobs.length ? (
                <div className="client-details-job-list">
                  {upcomingJobs.map((job) => (
                    <div className="client-details-job-row" key={job.id}>
                      <div>
                        <strong>{job.title}</strong>
                        <p>{job.description || 'No description provided.'}</p>
                      </div>
                      <span>{formatDate(job.start)}</span>
                    </div>
                  ))}
                </div>
              ) : (
                <div className="client-details-empty">No upcoming jobs scheduled for this client.</div>
              )}
            </article>
          </section>
        </>
      ) : null}
    </div>
  );
}
