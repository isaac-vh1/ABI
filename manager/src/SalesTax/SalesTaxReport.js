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

function formatDate(value) {
  if (!value) return 'Not recorded';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return date.toLocaleDateString();
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
  const [locationCode, setLocationCode] = useState('');

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
        const params = new URLSearchParams({ year, quarter });
        if (locationCode) {
          params.set('locationCode', locationCode);
        }
        const response = await fetch(`/api/manager/sales-tax-report?${params.toString()}`, {
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
  }, [authLoading, locationCode, quarter, user, year]);

  const summary = report?.summary || {
    invoiceCount: 0,
    taxableSales: 0,
    salesTaxCollected: 0,
    totalPaid: 0,
    expenseCount: 0,
    expenses: 0,
    netCash: 0,
  };
  const locations = report?.locations || [];
  const paidInvoices = report?.paidInvoices || [];
  const expensesByCategory = report?.expensesByCategory || [];
  const availableLocationCodes = report?.availableLocationCodes || [];

  return (
    <div className="sales-tax-page">
      <HeaderBar page="Sales Tax" toggleSidebar={toggleSidebar} collapsed={collapsed} />

      <section className="sales-tax-hero">
        <div>
          <span className="sales-tax-kicker">Cash Basis Accounting</span>
          <h2>Sales Tax Ledger</h2>
          <p>Track tax actually collected in the quarter from paid invoices, grouped by location code and backed by a payment ledger.</p>
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
          <label>
            <span>Location Code</span>
            <select value={locationCode} onChange={(e) => setLocationCode(e.target.value)}>
              <option value="">All locations</option>
              {availableLocationCodes.map((code) => (
                <option key={code} value={code}>{code}</option>
              ))}
            </select>
          </label>
        </div>
      </section>

      {loading ? <div className="sales-tax-state">Loading sales tax report...</div> : null}
      {error ? <div className="sales-tax-state sales-tax-error">{error}</div> : null}

      {!loading && !error ? (
        <>
          <section className="sales-tax-basis-note">
            <strong>Basis:</strong> {report?.basis === 'cash' ? 'Cash basis' : report?.basis || 'Unknown'}
            <span>Only invoices marked paid with a `payment_date` inside the selected quarter are included and grouped by the stored DOR location code on each service location.</span>
          </section>

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
            <article className="sales-tax-card">
              <span>Quarter Expenses</span>
              <strong>{formatCurrency(summary.expenses)}</strong>
            </article>
            <article className="sales-tax-card">
              <span>Net Cash</span>
              <strong>{formatCurrency(summary.netCash)}</strong>
            </article>
          </section>

          <section className="sales-tax-panel">
            <div className="sales-tax-panel-header">
              <div>
                <h3>{quarterLabels[report?.quarter] || `Q${quarter}`} {report?.year || year}</h3>
                <p>{report?.periodStart} through {report?.periodEnd}</p>
              </div>
              <span>{locationCode ? `Filtered to ${locationCode}` : `${locations.length} location code(s)`}</span>
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
                      <tr key={`${row.locationCode}-${row.locationId || 'na'}`}>
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

          <section className="sales-tax-panel">
            <div className="sales-tax-panel-header">
              <div>
                <h3>Expenses by Category</h3>
                <p>Quarter expenses included in the cash-basis worksheet.</p>
              </div>
              <span>{summary.expenseCount} expense(s)</span>
            </div>

            {expensesByCategory.length ? (
              <div className="sales-tax-table-wrap">
                <table className="sales-tax-table">
                  <thead>
                    <tr>
                      <th>Category</th>
                      <th>Entries</th>
                      <th>Amount</th>
                    </tr>
                  </thead>
                  <tbody>
                    {expensesByCategory.map((expense) => (
                      <tr key={expense.category}>
                        <td>{expense.category}</td>
                        <td>{expense.expenseCount}</td>
                        <td>{formatCurrency(expense.amount)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <div className="sales-tax-empty">
                No expenses were recorded in this quarter.
              </div>
            )}
          </section>

          <section className="sales-tax-panel">
            <div className="sales-tax-panel-header">
              <div>
                <h3>Paid Invoice Ledger</h3>
                <p>Audit trail for the amounts included in this cash-basis quarter.</p>
              </div>
              <span>{paidInvoices.length} paid invoice(s)</span>
            </div>

            {paidInvoices.length ? (
              <div className="sales-tax-table-wrap">
                <table className="sales-tax-table">
                  <thead>
                    <tr>
                      <th>Paid Date</th>
                      <th>Invoice</th>
                      <th>Location Code</th>
                      <th>Taxable Sales</th>
                      <th>Sales Tax</th>
                      <th>Total Paid</th>
                    </tr>
                  </thead>
                  <tbody>
                    {paidInvoices.map((invoice) => (
                      <tr key={`${invoice.invoiceNumber}-${invoice.paidDate}`}>
                        <td>{formatDate(invoice.paidDate)}</td>
                        <td>{invoice.invoiceNumber || `#${invoice.id}`}</td>
                        <td>{invoice.locationCode}</td>
                        <td>{formatCurrency(invoice.taxableSales)}</td>
                        <td>{formatCurrency(invoice.salesTaxCollected)}</td>
                        <td>{formatCurrency(invoice.totalPaid)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <div className="sales-tax-empty">
                There are no paid invoices recorded in this quarter yet.
              </div>
            )}
          </section>
        </>
      ) : null}
    </div>
  );
}
