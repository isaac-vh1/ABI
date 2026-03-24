import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

import './ReceiptDetails.css';

import HeaderBar from '../Components/HeaderBar';
import { useAuth } from '../AuthContext';

function formatDate(value) {
  if (!value) return 'Not provided';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return String(value);
  return date.toLocaleDateString();
}

function formatCurrency(value) {
  const amount = Number(value || 0);
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    maximumFractionDigits: 2,
  }).format(amount);
}

export default function ReceiptDetails({ toggleSidebar, collapsed }) {
  const { receiptId } = useParams();
  const navigate = useNavigate();
  const { user, loading: authLoading } = useAuth();
  const [receipt, setReceipt] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    if (authLoading) return;
    if (!user) {
      setLoading(false);
      return;
    }

    let cancelled = false;

    async function loadReceipt() {
      setLoading(true);
      setError('');
      try {
        const token = await user.getIdToken();
        const response = await fetch(`/api/manager/receipt/${receiptId}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        const payload = await response.json().catch(() => ({}));
        if (!response.ok) {
          throw new Error(payload.error || 'Failed to load receipt.');
        }
        if (!cancelled) {
          setReceipt(payload);
        }
      } catch (err) {
        if (!cancelled) {
          console.error('Receipt load failed:', err);
          setError(String(err.message || err));
          setReceipt(null);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    }

    loadReceipt();

    return () => {
      cancelled = true;
    };
  }, [authLoading, receiptId, user]);

  return (
    <div className="receipt-details-page">
      <HeaderBar page="Receipt Details" toggleSidebar={toggleSidebar} collapsed={collapsed} />

      <section className="receipt-details-hero">
        <div>
          <span className="receipt-details-kicker">Manager View</span>
          <h2>Receipt #{receiptId}</h2>
          <p>{receipt?.description || 'View the stored receipt image and expense fields.'}</p>
        </div>
        <button className="receipt-details-back" onClick={() => navigate('/receipts')}>
          Back to Receipts
        </button>
      </section>

      {loading ? <div className="receipt-details-state">Loading receipt...</div> : null}
      {error ? <div className="receipt-details-state receipt-details-error">{error}</div> : null}

      {!loading && !error && receipt ? (
        <section className="receipt-details-grid">
          <article className="receipt-details-panel">
            <div className="receipt-details-panel-header">
              <h3>Receipt Image</h3>
              <span>{receipt.image_name || receipt.image_mime_type || 'Stored image'}</span>
            </div>
            {receipt.image_src ? (
              <div className="receipt-details-image-frame">
                <img src={receipt.image_src} alt={`Receipt ${receipt.id}`} className="receipt-details-image" />
              </div>
            ) : (
              <div className="receipt-details-empty">No receipt image is stored for this record.</div>
            )}
          </article>

          <article className="receipt-details-panel">
            <div className="receipt-details-panel-header">
              <h3>Expense Details</h3>
              <span>Saved receipt fields</span>
            </div>
            <dl className="receipt-details-list">
              <div>
                <dt>Date</dt>
                <dd>{formatDate(receipt.expense_date)}</dd>
              </div>
              <div>
                <dt>Category</dt>
                <dd>{receipt.category || 'Uncategorized'}</dd>
              </div>
              <div>
                <dt>Amount</dt>
                <dd>{formatCurrency(receipt.amount)}</dd>
              </div>
              <div>
                <dt>Vendor</dt>
                <dd>{receipt.vendor_name || 'Not provided'}</dd>
              </div>
              <div>
                <dt>Subtotal</dt>
                <dd>{receipt.subtotal ? formatCurrency(receipt.subtotal) : 'Not provided'}</dd>
              </div>
              <div>
                <dt>Tax</dt>
                <dd>{receipt.tax_amount ? formatCurrency(receipt.tax_amount) : 'Not provided'}</dd>
              </div>
              <div className="receipt-details-list-wide">
                <dt>Description</dt>
                <dd>{receipt.description || 'No description saved.'}</dd>
              </div>
            </dl>
          </article>
        </section>
      ) : null}
    </div>
  );
}
