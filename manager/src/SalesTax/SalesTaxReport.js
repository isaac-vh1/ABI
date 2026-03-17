import React, { useEffect, useMemo, useState } from 'react';

import './SalesTaxReport.css';

import HeaderBar from '../Components/HeaderBar';
import { useAuth } from '../AuthContext';

const currencyFormatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: 'USD',
  maximumFractionDigits: 2,
});

const quarterLabels = {
  1: 'Q1',
  2: 'Q2',
  3: 'Q3',
  4: 'Q4',
};

function formatCurrency(value) {
  return currencyFormatter.format(Number(value || 0));
}

function currentQuarter() {
  return Math.floor(new Date().getMonth() / 3) + 1;
}

export default function SalesTaxReport({ toggleSidebar, collapsed }) {
  const { user, loading: authLoading } = useAuth();
  const [report, setReport] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [year, setYear] = useState(String(new Date().getFullYear()));
  const [quarter, setQuarter] = useState(String(currentQuarter()));

  const yearOptions = useMemo(() => {
    const now = new Date().getFullYear();
    return [now - 1, now, now + 1];
  }, []);

  useEffect(() => {
    if (authLoading) return;
    if (!user) {
      setLoading(false);
      return;
    }

    let cancelled = false;

    async function loadReport() {
      setLoading(true);
      setError('');
      try {
        const token = await user.getIdToken();
        const response = await fetch(`/api/manager/sales-tax-report?year=${year}&quarter=${quarter}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (!response.ok) {
          const payload = await response.json().catch(() => ({}));
          throw new Error(payload.error || 'Failed to load sales tax report.');
        }

        const payload = await response.json();
        if (!cancelled) {
          setReport(payload);
        }
      } catch (err) {
        if (!cancelled) {
          console.error('Sales tax report failed:', err);
          setError(String(err.message || err));
          setReport(null);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    }

    loadReport();

    return () => {
      cancelled = true;
    };
  }, [authLoading, quarter, user, year]);

  const summary = report?.summary || {
    invoiceCount: 0,
    taxableSales: 0,
    salesTaxCollected: 0,
    totalPaid: 0,
  };
  const locations = report?.locations || [];

  return (
    <div className="sales-tax-page">
      <HeaderBar page="Sales Tax" toggleSidebar={toggleSidebar} collapsed={collapsed} />

      <section className="sales-tax-hero">
        <div>
          <span className="sales-tax-kicker">Quarterly Filing</span>
          <h2>Sales Tax Tracking</h2>
          <p>Review paid invoices by location code, see tax collected, and confirm the quarter total before filing.</p>
        </div>
        <div className="sales-tax-filters">
          <label>
            <span>Year</span>
            <select value={year} onChange={(e) => setYear(e.target.value)}>
              {yearOptions.map((option) => (
                <option key={option} value={option}>{option}</option>
              ))}
            </select>
          </label>
          <label>
            <span>Quarter</span>
            <select value={quarter} onChange={(e) => setQuarter(e.target.value)}>
              {Object.entries(quarterLabels).map(([value, label]) => (
                <option key={value} value={value}>{label}</option>
              ))}
            </select>
          </label>
        </div>
      </section>

      {loading ? <div className="sales-tax-state">Loading sales tax report...</div> : null}
      {error ? <div className="sales-tax-state sales-tax-error">{error}</div> : null}

      {!loading && !error ? (
        <>
          <section className="sales-tax-metrics">
            <article className="sales-tax-card">
              <span>Taxable Sales</span>
              <strong>{formatCurrency(summary.taxableSales)}</strong>
            </article>
            <article className="sales-tax-card">
              <span>Sales Tax Collected</span>
              <strong>{formatCurrency(summary.salesTaxCollected)}</strong>
            </article>
            <article className="sales-tax-card">
              <span>Total Paid</span>
              <strong>{formatCurrency(summary.totalPaid)}</strong>
            </article>
            <article className="sales-tax-card">
              <span>Paid Invoices</span>
              <strong>{summary.invoiceCount}</strong>
            </article>
          </section>

          <section className="sales-tax-panel">
            <div className="sales-tax-panel-header">
              <div>
                <h3>{quarterLabels[report?.quarter] || `Q${quarter}`} {report?.year || year}</h3>
                <p>{report?.periodStart} through {report?.periodEnd}</p>
              </div>
              <span>{locations.length} location code(s)</span>
            </div>

            {locations.length ? (
              <div className="sales-tax-table-wrap">
                <table className="sales-tax-table">
                  <thead>
                    <tr>
                      <th>Location Code</th>
                      <th>Invoices</th>
                      <th>Taxable Sales</th>
                      <th>Sales Tax Collected</th>
                      <th>Total Paid</th>
                    </tr>
                  </thead>
                  <tbody>
                    {locations.map((row) => (
                      <tr key={row.locationCode}>
                        <td>{row.locationCode}</td>
                        <td>{row.invoiceCount}</td>
                        <td>{formatCurrency(row.taxableSales)}</td>
                        <td>{formatCurrency(row.salesTaxCollected)}</td>
                        <td>{formatCurrency(row.totalPaid)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <div className="sales-tax-empty">
                No paid invoices found for this quarter. This report uses paid invoices with a `payment_date`.
              </div>
            )}
          </section>
        </>
      ) : null}
    </div>
  );
}
