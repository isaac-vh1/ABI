import React, { useEffect, useMemo, useState } from 'react';

import './JobRequestsBoard.css';

import HeaderBar from '../Components/HeaderBar';
import { useAuth } from '../AuthContext';

const statusOptions = [
  { value: 'requested', label: 'Requested' },
  { value: 'review', label: 'In Review' },
  { value: 'scheduled', label: 'Scheduled' },
  { value: 'in_progress', label: 'In Progress' },
  { value: 'completed', label: 'Completed' },
];

function formatDate(value) {
  if (!value) return 'Not recorded';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return date.toLocaleString();
}

export default function JobRequestsBoard({ toggleSidebar, collapsed }) {
  const { user, loading: authLoading } = useAuth();
  const [payload, setPayload] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedRequest, setSelectedRequest] = useState(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (authLoading) return;
    if (!user) {
      setLoading(false);
      return;
    }

    let cancelled = false;

    async function loadBoard() {
      setLoading(true);
      setError('');
      try {
        const token = await user.getIdToken();
        const response = await fetch('/api/manager/job-requests', {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        const nextPayload = await response.json().catch(() => ({}));
        if (!response.ok) {
          throw new Error(nextPayload.error || 'Failed to load job board.');
        }
        if (!cancelled) {
          setPayload(nextPayload);
        }
      } catch (err) {
        if (!cancelled) {
          console.error('Job board failed:', err);
          setError(String(err.message || err));
          setPayload(null);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    }

    loadBoard();

    return () => {
      cancelled = true;
    };
  }, [authLoading, user]);

  const summary = payload?.summary || { total: 0, highPriority: 0, open: 0 };
  const columns = payload?.columns || [];
  const selectedStatusLabel = useMemo(
    () => statusOptions.find((option) => option.value === selectedRequest?.status)?.label || selectedRequest?.status,
    [selectedRequest]
  );

  const handleRequestChange = (event) => {
    const { name, value } = event.target;
    setSelectedRequest((current) => current ? ({ ...current, [name]: value }) : current);
  };

  const handleSave = async () => {
    if (!user || !selectedRequest || saving) return;
    setSaving(true);
    setError('');
    try {
      const token = await user.getIdToken();
      const response = await fetch(`/api/manager/job-request/${selectedRequest.id}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(selectedRequest),
      });
      const updated = await response.json().catch(() => ({}));
      if (!response.ok) {
        throw new Error(updated.error || 'Failed to save job request.');
      }
      setPayload((current) => current ? ({
        ...current,
        columns: current.columns.map((column) => ({
          ...column,
          items: column.items.filter((item) => item.id !== updated.id),
        })).map((column) => (
          column.key === updated.status
            ? { ...column, items: [updated, ...column.items] }
            : column
        )),
        summary: {
          ...current.summary,
          open: current.columns
            .flatMap((column) => column.items)
            .filter((item) => item.id !== updated.id)
            .concat(updated)
            .filter((item) => item.status !== 'completed').length,
          highPriority: current.columns
            .flatMap((column) => column.items)
            .filter((item) => item.id !== updated.id)
            .concat(updated)
            .filter((item) => item.priority === 'high').length,
        },
      }) : current);
      setSelectedRequest(updated);
    } catch (err) {
      console.error('Job request save failed:', err);
      setError(String(err.message || err));
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="job-board-page">
      <HeaderBar page="Job Board" toggleSidebar={toggleSidebar} collapsed={collapsed} />

      <section className="job-board-hero">
        <div>
          <span className="job-board-kicker">Project Intake</span>
          <h2>Client Request Board</h2>
          <p>Track new work separately from scheduled calendar events and move requests through review, scheduling, and delivery.</p>
        </div>
        <div className="job-board-summary-grid">
          <article className="job-board-metric">
            <span>Total Requests</span>
            <strong>{summary.total}</strong>
          </article>
          <article className="job-board-metric">
            <span>Open Work</span>
            <strong>{summary.open}</strong>
          </article>
          <article className="job-board-metric">
            <span>High Priority</span>
            <strong>{summary.highPriority}</strong>
          </article>
        </div>
      </section>

      {loading ? <div className="job-board-state">Loading board...</div> : null}
      {error ? <div className="job-board-state job-board-error">{error}</div> : null}

      {!loading && !error ? (
        <section className="job-board-columns">
          {columns.map((column) => (
            <article className="job-board-column" key={column.key}>
              <div className="job-board-column-header">
                <h3>{column.label}</h3>
                <span>{column.items.length}</span>
              </div>
              <div className="job-board-card-stack">
                {column.items.length ? (
                  column.items.map((item) => (
                    <button
                      className={`job-board-card priority-${item.priority}`}
                      key={item.id}
                      onClick={() => setSelectedRequest(item)}
                    >
                      <div className="job-board-card-topline">
                        <strong>{item.title}</strong>
                        <span>{item.priority}</span>
                      </div>
                      <p>{item.clientName}</p>
                      <small>{item.preferredWindow || 'No preferred timing provided'}</small>
                    </button>
                  ))
                ) : (
                  <div className="job-board-empty">No requests in this column.</div>
                )}
              </div>
            </article>
          ))}
        </section>
      ) : null}

      {selectedRequest ? (
        <div className="job-board-modal-backdrop" onClick={() => setSelectedRequest(null)}>
          <div className="job-board-modal" onClick={(event) => event.stopPropagation()}>
            <div className="job-board-modal-header">
              <div>
                <h3>{selectedRequest.title}</h3>
                <p>{selectedRequest.clientName}</p>
              </div>
              <span className={`job-board-status status-${selectedRequest.status}`}>{selectedStatusLabel}</span>
            </div>

            <div className="job-board-modal-grid">
              <label>
                <span>Status</span>
                <select name="status" value={selectedRequest.status} onChange={handleRequestChange}>
                  {statusOptions.map((option) => (
                    <option key={option.value} value={option.value}>{option.label}</option>
                  ))}
                </select>
              </label>
              <label>
                <span>Priority</span>
                <select name="priority" value={selectedRequest.priority} onChange={handleRequestChange}>
                  <option value="low">Low</option>
                  <option value="normal">Normal</option>
                  <option value="high">High</option>
                </select>
              </label>
              <label className="job-board-modal-wide">
                <span>Preferred Timing</span>
                <input name="preferredWindow" value={selectedRequest.preferredWindow || ''} onChange={handleRequestChange} />
              </label>
              <label className="job-board-modal-wide">
                <span>Service Address</span>
                <input name="serviceAddress" value={selectedRequest.serviceAddress || ''} onChange={handleRequestChange} />
              </label>
              <label className="job-board-modal-wide">
                <span>Client Notes</span>
                <textarea name="details" rows="5" value={selectedRequest.details || ''} onChange={handleRequestChange} />
              </label>
              <label className="job-board-modal-wide">
                <span>Manager Notes</span>
                <textarea name="managerNotes" rows="5" value={selectedRequest.managerNotes || ''} onChange={handleRequestChange} />
              </label>
            </div>

            <div className="job-board-modal-meta">
              <span>Created {formatDate(selectedRequest.createdAt)}</span>
              <span>Updated {formatDate(selectedRequest.updatedAt)}</span>
            </div>

            <div className="job-board-modal-actions">
              <button className="job-board-secondary" onClick={() => setSelectedRequest(null)}>
                Close
              </button>
              <button className="job-board-primary" onClick={handleSave} disabled={saving}>
                {saving ? 'Saving...' : 'Save Changes'}
              </button>
            </div>
          </div>
        </div>
      ) : null}
    </div>
  );
}
