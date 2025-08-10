import React, { useRef, useState, useEffect } from 'react';
import { Card, Button, Form, Image, Spinner, ProgressBar } from 'react-bootstrap';
import { createWorker } from 'tesseract.js';

/**
 * ReceiptScanner – capture a receipt photo, run OCR **locally** (Tesseract.js),
 * auto‑fill the expense form, and send only the parsed JSON data to the server.
 *
 * POST /api/expenses
 * Body (JSON): { expense_date, category, amount, description }
 */
export default function ReceiptScanner() {
  const fileRef = useRef(null);
  const [preview, setPreview] = useState(null);
  const [loading, setLoading] = useState(false);
  const [progress, setProgress] = useState(0);
  const [form, setForm] = useState({
    expense_date: '',
    category: '',
    amount: '',
    description: '',
  });

  useEffect(() => {
    if (!preview) return;

    (async () => {
      setLoading(true);
      setProgress(0);
      const worker = await createWorker({ logger: (m) => m.progress && setProgress(Math.round(m.progress * 100)) });
      try {
        await worker.loadLanguage('eng');
        await worker.initialize('eng');
        const { data } = await worker.recognize(preview);
        parseReceiptText(data.text);
      } catch (err) {
        console.error('OCR error:', err);
        alert('Could not read the receipt. Please enter the data manually.');
      } finally {
        await worker.terminate();
        setLoading(false);
      }
    })();
  }, [preview]);

  const parseReceiptText = (text) => {
    const dateMatch = text.match(/(\d{4}-\d{2}-\d{2})|((?:0?[1-9]|1[0-2])[\/.-](?:0?[1-9]|[12][0-9]|3[01])[\/.-]\d{2,4})/);
    const amountMatch = text.match(/\$?\s*(\d+[,.]\d{2})/);

    setForm((f) => ({
      ...f,
      expense_date: dateMatch ? standardizeDate(dateMatch[0]) : f.expense_date,
      amount: amountMatch ? amountMatch[1].replace(/,/, '.') : f.amount,
      description: text.split('\n').slice(0, 3).join(' ').slice(0, 255), // first few lines
    }));
  };

  const standardizeDate = (s) => {
    if (/\d{4}-\d{2}-\d{2}/.test(s)) return s; // already yyyy-mm-dd
    const [m, d, y] = s.replace(/\./g, '/').split(/[\/]/);
    const year = y.length === 2 ? '20' + y : y;
    return `${year}-${m.padStart(2, '0')}-${d.padStart(2, '0')}`;
  };

  /* ───────── FORM HANDLERS ───────── */
  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((f) => ({ ...f, [name]: value }));
  };

  const handleFile = (e) => {
    const file = e.target.files?.[0];
    if (!file) return;
    setPreview(URL.createObjectURL(file));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      setLoading(true);
      const res = await fetch('/api/manager/update/expenses', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(form),
      });
      if (!res.ok) throw new Error(await res.text());
      alert('Expense saved!');
      resetAll();
    } catch (err) {
      alert(`Upload failed: ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  const resetAll = () => {
    setForm({ expense_date: '', category: '', amount: '', description: '' });
    setPreview(null);
    setProgress(0);
    if (fileRef.current) fileRef.current.value = '';
  };

  /* ───────── UI ───────── */
  return (
    <div className="d-flex justify-content-center p-4">
      <Card className="w-100" style={{ maxWidth: 640 }}>
        <Card.Body>
          <Card.Title className="text-center mb-4">Scan Receipt (Offline)</Card.Title>

          {/* Image capture */}
          <div className="mb-4 text-center">
            {preview ? (
              <Image src={preview} thumbnail className="w-100 mb-3" />
            ) : (
              <div className="border border-secondary rounded p-5 text-muted">No image captured</div>
            )}
            <Form.Control
              ref={fileRef}
              type="file"
              accept="image/*"
              capture="environment"
              onChange={handleFile}
            />
            {loading && progress > 0 && progress < 100 && (
              <ProgressBar now={progress} label={`${progress}%`} className="mt-2" />
            )}
          </div>

          {/* Expense form */}
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Date</Form.Label>
              <Form.Control
                type="date"
                name="expense_date"
                value={form.expense_date}
                onChange={handleChange}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Category</Form.Label>
              <Form.Control
                name="category"
                placeholder="e.g. Office Supplies"
                value={form.category}
                onChange={handleChange}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Amount</Form.Label>
              <Form.Control
                type="number"
                step="0.01"
                name="amount"
                placeholder="0.00"
                value={form.amount}
                onChange={handleChange}
                required
              />
            </Form.Group>

            <Form.Group className="mb-4">
              <Form.Label>Description</Form.Label>
              <Form.Control
                as="textarea"
                rows={2}
                name="description"
                value={form.description}
                onChange={handleChange}
              />
            </Form.Group>

            <Button type="submit" disabled={loading || progress < 100 && preview} className="w-100">
              {loading ? (
                <><Spinner as="span" animation="border" size="sm" /> Saving…</>
              ) : (
                'Save Expense'
              )}
            </Button>
          </Form>
        </Card.Body>
      </Card>
    </div>
  );
}
