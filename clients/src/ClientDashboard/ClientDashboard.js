import React, { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';

import './ClientDashboard.css';

import { useAuth } from '../AuthContext';

const currencyFormatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
  maximumFractionDigits: 2,
});
const statusLabels = {
  draft: 'Draft invoice',
  pending: 'Awaiting payment',
  paid: 'Paid in full',
};
const weekdayLabels = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
const monthLabels = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

function formatCurrency(value) {
  return currencyFormatter.format(Number(value || 0));
}

function formatDate(value) {
  if (!value) return 'Not scheduled';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return date.toLocaleDateString();
}

function formatDateTime(value) {
  if (!value) return 'Not scheduled';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return date.toLocaleString();
}

function dayKey(value) {
  const date = value instanceof Date ? value : new Date(value);
  if (Number.isNaN(date.getTime())) return '';
  const year = date.getFullYear();
  const month = `${date.getMonth() + 1}`.padStart(2, '0');
  const day = `${date.getDate()}`.padStart(2, '0');
  return `${year}-${month}-${day}`;
}

function MetricCard({ label, value, detail, tone = 'default' }) {
  return (
    <article className={`client-metric client-metric-${tone}`}>
      <span>{label}</span>
      <strong>{value}</strong>
      {detail ? <p>{detail}</p> : null}
    </article>
  );
}

export default function ClientDashboard() {
  const { user, loading: authLoading } = useAuth();
  const navigate = useNavigate();
  const [dashboard, setDashboard] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [selectedInvoiceYear, setSelectedInvoiceYear] = useState('');
  const [calendarMonth, setCalendarMonth] = useState(() => {
    const today = new Date();
    return new Date(today.getFullYear(), today.getMonth(), 1);
  });
  const [selectedCalendarDate, setSelectedCalendarDate] = useState('');
  const [jobForm, setJobForm] = useState({
    title: '',
    details: '',
    priority: 'normal',
    preferredWindow: '',
    serviceAddress: '',
  });

  useEffect(() => {
    if (authLoading) return;
    if (!user) {
      setLoading(false);
      return;
    }

    let cancelled = false;

    async function loadDashboard() {
      setLoading(true);
      setError('');
      try {
        const token = await user.getIdToken();
        const response = await fetch('/api/client/dashboard', {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        const payload = await response.json().catch(() => ({}));
        if (!response.ok) {
          throw new Error(payload.error || 'Failed to load dashboard.');
        }
        if (!cancelled) {
          setDashboard(payload);
          const availableYears = (payload.invoices || [])
            .map((invoice) => {
              const date = new Date(invoice.issueDate || invoice.dueDate || invoice.paymentDate || '');
              return Number.isNaN(date.getTime()) ? null : String(date.getFullYear());
            })
            .filter(Boolean)
            .sort((a, b) => Number(b) - Number(a));
          setSelectedInvoiceYear((current) => current || availableYears[0] || '');
          setJobForm((current) => ({
            ...current,
            serviceAddress:
              current.serviceAddress ||
              [payload.client?.address, payload.client?.city, payload.client?.zipCode]
                .filter(Boolean)
                .join(', '),
          }));
        }
      } catch (err) {
        if (!cancelled) {
          console.error('Client dashboard failed:', err);
          setError(String(err.message || err));
          setDashboard(null);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    }

    loadDashboard();

    return () => {
      cancelled = true;
    };
  }, [authLoading, user]);

  const summary = dashboard?.summary || {
    invoiceCount: 0,
    outstandingBalance: 0,
    paidInvoiceValue: 0,
    pendingInvoiceCount: 0,
    jobRequestCount: 0,
  };
  const invoices = dashboard?.invoices || [];
  const jobRequests = dashboard?.jobRequests || [];
  const scheduledJobs = dashboard?.scheduledJobs || [];
  const scheduledJobsByDay = useMemo(
    () =>
      scheduledJobs.reduce((acc, job) => {
        const key = dayKey(job.start);
        if (!key) return acc;
        acc[key] = [...(acc[key] || []), job];
        return acc;
      }, {}),
    [scheduledJobs]
  );
  const selectedDayJobs = selectedCalendarDate ? (scheduledJobsByDay[selectedCalendarDate] || []) : [];
  const calendarDays = useMemo(() => {
    const startOfMonth = new Date(calendarMonth.getFullYear(), calendarMonth.getMonth(), 1);
    const totalDays = new Date(calendarMonth.getFullYear(), calendarMonth.getMonth() + 1, 0).getDate();
    const cells = Array.from({ length: startOfMonth.getDay() }, () => null);
    for (let day = 1; day <= totalDays; day += 1) {
      cells.push(new Date(calendarMonth.getFullYear(), calendarMonth.getMonth(), day));
    }
    return cells;
  }, [calendarMonth]);
  const invoiceYears = useMemo(
    () =>
      Array.from(
        new Set(
          invoices
            .map((invoice) => {
              const date = new Date(invoice.issueDate || invoice.dueDate || invoice.paymentDate || '');
              return Number.isNaN(date.getTime()) ? null : String(date.getFullYear());
            })
            .filter(Boolean)
        )
      ).sort((a, b) => Number(b) - Number(a)),
    [invoices]
  );
  const filteredInvoices = useMemo(() => {
    const filtered = selectedInvoiceYear
      ? invoices.filter((invoice) => {
          const date = new Date(invoice.issueDate || invoice.dueDate || invoice.paymentDate || '');
          return !Number.isNaN(date.getTime()) && String(date.getFullYear()) === selectedInvoiceYear;
        })
      : invoices;
    return [...filtered].sort((a, b) => {
      const left = new Date(a.issueDate || a.dueDate || a.paymentDate || 0).getTime();
      const right = new Date(b.issueDate || b.dueDate || b.paymentDate || 0).getTime();
      return right - left;
    });
  }, [invoices, selectedInvoiceYear]);
  const latestInvoices = useMemo(() => filteredInvoices.slice(0, 5), [filteredInvoices]);

  useEffect(() => {
    const firstScheduledDay = Object.keys(scheduledJobsByDay).sort()[0] || '';
    if (!selectedCalendarDate && firstScheduledDay) {
      setSelectedCalendarDate(firstScheduledDay);
      const firstDate = new Date(firstScheduledDay);
      setCalendarMonth(new Date(firstDate.getFullYear(), firstDate.getMonth(), 1));
    }
  }, [scheduledJobsByDay, selectedCalendarDate]);

  const handleFormChange = (event) => {
    const { name, value } = event.target;
    setJobForm((current) => ({
      ...current,
      [name]: value,
    }));
  };

  const handleJobRequestSubmit = async (event) => {
    event.preventDefault();
    if (!user || submitting) return;
    if (!jobForm.title.trim()) {
      setError('Job request title is required.');
      return;
    }

    setSubmitting(true);
    setError('');
    try {
      const token = await user.getIdToken();
      const response = await fetch('/api/client/job-request', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(jobForm),
      });
      const payload = await response.json().catch(() => ({}));
      if (!response.ok) {
        throw new Error(payload.error || 'Failed to send job request.');
      }
      setDashboard((current) => current ? ({
        ...current,
        summary: {
          ...current.summary,
          jobRequestCount: (current.summary?.jobRequestCount || 0) + 1,
        },
        jobRequests: [payload, ...(current.jobRequests || [])],
      }) : current);
      setJobForm({
        title: '',
        details: '',
        priority: 'normal',
        preferredWindow: '',
        serviceAddress:
          [dashboard?.client?.address, dashboard?.client?.city, dashboard?.client?.zipCode]
            .filter(Boolean)
            .join(', '),
      });
    } catch (err) {
      console.error('Job request submit failed:', err);
      setError(String(err.message || err));
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return <div className="client-dashboard-state">Loading your dashboard...</div>;
  }

  return (
    <div className="client-portal">
      <section className="client-hero">
        <div className="client-hero-copy">
          <span className="client-kicker">Client Portal</span>
          <h1>{dashboard?.client?.name || 'My Dashboard'}</h1>
          <p>
            {[dashboard?.client?.address, dashboard?.client?.city, dashboard?.client?.zipCode]
              .filter(Boolean)
              .join(', ') || 'No service address on file.'}
          </p>
        </div>
        <div className="client-hero-summary">
          <span>Outstanding Balance</span>
          <strong>{formatCurrency(summary.outstandingBalance)}</strong>
          <p>{summary.pendingInvoiceCount} invoice(s) still open</p>
        </div>
      </section>

      {error ? <div className="client-dashboard-state client-dashboard-error">{error}</div> : null}

      <section className="client-metrics-grid">
        <MetricCard
          label="Paid Invoice Value"
          value={formatCurrency(summary.paidInvoiceValue)}
          detail={`${summary.invoiceCount} total invoice(s) on file`}
          tone="calm"
        />
        <MetricCard
          label="Open Balance"
          value={formatCurrency(summary.outstandingBalance)}
          detail={`${summary.pendingInvoiceCount} invoice(s) awaiting payment`}
          tone="warning"
        />
        <MetricCard
          label="Job Requests"
          value={summary.jobRequestCount}
          detail="Separate from your scheduled service events"
          tone="neutral"
        />
      </section>

      <section className="client-dashboard-grid">
        <article className="client-panel client-panel-form">
          <div className="client-panel-header">
            <div>
              <h2>Request New Work</h2>
              <p>Send the team a scoped request without creating a calendar event.</p>
            </div>
          </div>
          <form className="client-job-form" onSubmit={handleJobRequestSubmit}>
            <label>
              <span>Request Title</span>
              <input
                name="title"
                value={jobForm.title}
                onChange={handleFormChange}
                placeholder="Spring cleanup, patio refresh, pruning..."
              />
            </label>
            <label>
              <span>Priority</span>
              <select name="priority" value={jobForm.priority} onChange={handleFormChange}>
                <option value="low">Low</option>
                <option value="normal">Normal</option>
                <option value="high">High</option>
              </select>
            </label>
            <label className="client-job-form-wide">
              <span>Preferred Timing</span>
              <input
                name="preferredWindow"
                value={jobForm.preferredWindow}
                onChange={handleFormChange}
                placeholder="Next week, before April 15, weekday mornings..."
              />
            </label>
            <label className="client-job-form-wide">
              <span>Service Address</span>
              <input
                name="serviceAddress"
                value={jobForm.serviceAddress}
                onChange={handleFormChange}
              />
            </label>
            <label className="client-job-form-wide">
              <span>Project Notes</span>
              <textarea
                name="details"
                rows="5"
                value={jobForm.details}
                onChange={handleFormChange}
                placeholder="Describe the work, goals, materials, access notes, or photos you plan to send separately."
              />
            </label>
            <div className="client-job-actions client-job-form-wide">
              <button className="client-primary-button" type="submit" disabled={submitting}>
                {submitting ? 'Sending...' : 'Submit Job Request'}
              </button>
            </div>
          </form>
        </article>

        <article className="client-panel">
          <div className="client-panel-header">
            <div>
              <h2>Scheduled Calendar</h2>
              <p>Click a highlighted day to view the jobs already booked for that date.</p>
            </div>
          </div>
          <div className="client-calendar-header">
            <button
              type="button"
              className="client-calendar-nav"
              onClick={() => setCalendarMonth((current) => new Date(current.getFullYear(), current.getMonth() - 1, 1))}
            >
              Prev
            </button>
            <strong>{monthLabels[calendarMonth.getMonth()]} {calendarMonth.getFullYear()}</strong>
            <button
              type="button"
              className="client-calendar-nav"
              onClick={() => setCalendarMonth((current) => new Date(current.getFullYear(), current.getMonth() + 1, 1))}
            >
              Next
            </button>
          </div>
          <div className="client-calendar-grid">
            {weekdayLabels.map((label) => (
              <div key={label} className="client-calendar-weekday">{label}</div>
            ))}
            {calendarDays.map((date, index) => {
              if (!date) {
                return <div key={`empty-${index}`} className="client-calendar-cell client-calendar-empty" />;
              }
              const key = dayKey(date);
              const jobs = scheduledJobsByDay[key] || [];
              const isSelected = key === selectedCalendarDate;
              return (
                <button
                  type="button"
                  key={key}
                  className={`client-calendar-cell ${jobs.length ? 'has-jobs' : ''} ${isSelected ? 'selected' : ''}`}
                  onClick={() => setSelectedCalendarDate(key)}
                >
                  <span>{date.getDate()}</span>
                  {jobs.length ? <strong>{jobs.length} job{jobs.length > 1 ? 's' : ''}</strong> : null}
                </button>
              );
            })}
          </div>
          {selectedDayJobs.length ? (
            <div className="client-scheduled-list">
              {selectedDayJobs.map((job) => (
                <article className="client-scheduled-card" key={job.id}>
                  <strong>{job.title}</strong>
                  <p>{job.description || 'No additional details provided.'}</p>
                  <div className="client-scheduled-meta">
                    <span>{formatDateTime(job.start)}</span>
                    {job.end ? <span>Ends {formatDateTime(job.end)}</span> : null}
                  </div>
                </article>
              ))}
            </div>
          ) : (
            <div className="client-empty-state">
              {selectedCalendarDate ? 'No jobs are scheduled on that day.' : 'No scheduled jobs yet.'}
            </div>
          )}
        </article>

        <article className="client-panel client-panel-wide">
          <div className="client-panel-header">
            <div>
              <h2>Invoices</h2>
              <p>A cleaner view of balances, due dates, and invoice history.</p>
            </div>
            {invoiceYears.length ? (
              <label className="client-invoice-year-filter">
                <span>Year</span>
                <select value={selectedInvoiceYear} onChange={(event) => setSelectedInvoiceYear(event.target.value)}>
                  {invoiceYears.map((year) => (
                    <option key={year} value={year}>{year}</option>
                  ))}
                </select>
              </label>
            ) : null}
          </div>

          {latestInvoices.length ? (
            <div className="client-invoice-highlights">
              {latestInvoices.slice(0, 3).map((invoice) => (
                <button
                  className="client-invoice-highlight"
                  key={invoice.id}
                  onClick={() => navigate(`/invoice#${invoice.id}`)}
                >
                  <span>{invoice.invoiceNumber || `#${invoice.id}`}</span>
                  <strong>{formatCurrency(invoice.balanceDue)}</strong>
                  <p>{statusLabels[invoice.status] || invoice.status}</p>
                </button>
              ))}
            </div>
          ) : null}

          {filteredInvoices.length ? (
            <div className="client-invoice-table-wrap">
              <table className="client-invoice-table">
                <thead>
                  <tr>
                    <th>Invoice</th>
                    <th>Issued</th>
                    <th>Due</th>
                    <th>Status</th>
                    <th>Total</th>
                    <th>Balance</th>
                    <th>Open</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredInvoices.map((invoice) => (
                    <tr key={invoice.id} onClick={() => navigate(`/invoice#${invoice.id}`)}>
                      <td>
                        <strong>{invoice.invoiceNumber || `#${invoice.id}`}</strong>
                      </td>
                      <td>{formatDate(invoice.issueDate)}</td>
                      <td>{formatDate(invoice.dueDate)}</td>
                      <td>
                        <span className={`client-status-pill client-status-${invoice.status}`}>
                          {invoice.status}
                        </span>
                      </td>
                      <td>{formatCurrency(invoice.subtotal + invoice.salesTax + invoice.tips)}</td>
                      <td>{formatCurrency(invoice.balanceDue)}</td>
                      <td>
                        <button
                          type="button"
                          className="client-open-button"
                          onClick={(event) => {
                            event.stopPropagation();
                            navigate(`/invoice#${invoice.id}`);
                          }}
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
            <div className="client-empty-state">
              No invoices are available for {selectedInvoiceYear || 'the selected year'}.
            </div>
          )}
        </article>
      </section>
    </div>
  );
}
