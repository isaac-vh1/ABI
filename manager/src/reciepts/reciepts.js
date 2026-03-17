import React, { useEffect, useMemo, useRef, useState } from 'react';
import { Alert, Button, Card, Col, Form, Image, ProgressBar, Row, Spinner } from 'react-bootstrap';
import { createWorker } from 'tesseract.js';

import HeaderBar from '../Components/HeaderBar';
import { useAuth } from '../AuthContext';

const CATEGORY_KEYWORDS = [
  { category: 'Fuel', terms: ['shell', 'chevron', '76', 'gas', 'fuel', 'exxon'] },
  { category: 'Meals', terms: ['restaurant', 'cafe', 'coffee', 'burger', 'grill', 'pizza'] },
  { category: 'Office Supplies', terms: ['office', 'staples', 'depot', 'printer', 'paper'] },
  { category: 'Materials', terms: ['home depot', 'lowes', 'lumber', 'hardware', 'supply'] },
  { category: 'Travel', terms: ['hotel', 'inn', 'airlines', 'uber', 'lyft', 'parking'] },
  { category: 'Equipment', terms: ['tool', 'equipment', 'rental', 'repair'] },
];

const initialForm = {
  expense_date: '',
  category: '',
  amount: '',
  description: '',
  vendor_name: '',
  subtotal: '',
  tax_amount: '',
};

function standardizeDate(input) {
  if (!input) return '';
  if (/^\d{4}-\d{2}-\d{2}$/.test(input)) return input;
  const cleaned = input.replace(/\./g, '/').replace(/-/g, '/');
  const parts = cleaned.split('/');
  if (parts.length !== 3) return '';
  const [month, day, yearValue] = parts;
  const year = yearValue.length === 2 ? `20${yearValue}` : yearValue;
  return `${year}-${String(month).padStart(2, '0')}-${String(day).padStart(2, '0')}`;
}

function guessCategory(text) {
  const normalized = text.toLowerCase();
  const match = CATEGORY_KEYWORDS.find(({ terms }) => terms.some((term) => normalized.includes(term)));
  return match ? match.category : '';
}

function parseAmounts(text) {
  const matches = [...text.matchAll(/(?:\$|USD\s*)?(\d{1,4}(?:[.,]\d{2}))/gi)]
    .map((match) => Number(match[1].replace(',', '.')))
    .filter((value) => Number.isFinite(value));

  if (!matches.length) {
    return { amount: '', subtotal: '', tax_amount: '' };
  }

  const sorted = [...matches].sort((a, b) => b - a);
  const total = sorted[0];
  const maybeSubtotal = sorted.find((value) => value < total) ?? '';
  const maybeTax = maybeSubtotal !== '' ? Number((total - maybeSubtotal).toFixed(2)) : '';

  return {
    amount: total.toFixed(2),
    subtotal: maybeSubtotal === '' ? '' : maybeSubtotal.toFixed(2),
    tax_amount: maybeTax === '' || maybeTax < 0 ? '' : maybeTax.toFixed(2),
  };
}

function parseVendor(text) {
  const lines = text
    .split('\n')
    .map((line) => line.trim())
    .filter(Boolean);
  if (!lines.length) return '';

  const vendorLine = lines.find((line) => /[A-Za-z]{3,}/.test(line)) || lines[0];
  return vendorLine.replace(/[^A-Za-z0-9&.' -]/g, '').trim().slice(0, 120);
}

function parseReceiptText(text) {
  const normalized = text.replace(/\r/g, '');
  const dateMatch = normalized.match(
    /(\d{4}-\d{2}-\d{2})|((?:0?[1-9]|1[0-2])[./-](?:0?[1-9]|[12][0-9]|3[01])[./-](?:\d{2}|\d{4}))/
  );
  const amounts = parseAmounts(normalized);
  const description = normalized
    .split('\n')
    .map((line) => line.trim())
    .filter(Boolean)
    .slice(0, 5)
    .join(' | ')
    .slice(0, 255);

  return {
    expense_date: dateMatch ? standardizeDate(dateMatch[0]) : '',
    category: guessCategory(normalized),
    amount: amounts.amount,
    description,
    vendor_name: parseVendor(normalized),
    subtotal: amounts.subtotal,
    tax_amount: amounts.tax_amount,
  };
}

function ReceiptScanner({ toggleSidebar, collapsed }) {
  const fileRef = useRef(null);
  const workerRef = useRef(null);
  const { user, loading: authLoading } = useAuth();

  const [preview, setPreview] = useState(null);
  const [ocrText, setOcrText] = useState('');
  const [loading, setLoading] = useState(false);
  const [progress, setProgress] = useState(0);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [form, setForm] = useState(initialForm);

  useEffect(() => {
    return () => {
      if (preview) {
        URL.revokeObjectURL(preview);
      }
    };
  }, [preview]);

  const requiredFields = useMemo(
    () => ({
      expense_date: form.expense_date,
      category: form.category,
      amount: form.amount,
      description: form.description,
    }),
    [form]
  );

  const canSubmit = Object.values(requiredFields).every(Boolean) && !loading;

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  };

  const handleFile = async (e) => {
    const file = e.target.files?.[0];
    if (!file) return;

    if (preview) {
      URL.revokeObjectURL(preview);
    }

    const nextPreview = URL.createObjectURL(file);
    setPreview(nextPreview);
    setOcrText('');
    setError('');
    setSuccess('');
    setProgress(0);
    setLoading(true);

    try {
      if (!workerRef.current) {
        workerRef.current = await createWorker({
          logger: (message) => {
            if (message.progress) {
              setProgress(Math.round(message.progress * 100));
            }
          },
        });
        await workerRef.current.loadLanguage('eng');
        await workerRef.current.initialize('eng');
      }

      const { data } = await workerRef.current.recognize(file);
      setOcrText(data.text || '');
      const parsed = parseReceiptText(data.text || '');
      setForm((prev) => ({
        ...prev,
        ...Object.fromEntries(
          Object.entries(parsed).map(([key, value]) => [key, value || prev[key]])
        ),
      }));
      setProgress(100);
    } catch (err) {
      console.error('OCR error:', err);
      setError('Could not read the receipt automatically. Enter the values manually.');
    } finally {
      setLoading(false);
    }
  };

  const resetAll = () => {
    setForm(initialForm);
    setOcrText('');
    setError('');
    setSuccess('');
    setProgress(0);
    setLoading(false);
    if (preview) {
      URL.revokeObjectURL(preview);
      setPreview(null);
    }
    if (fileRef.current) {
      fileRef.current.value = '';
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (authLoading || !user) {
      setError('You must be signed in to save an expense.');
      return;
    }

    try {
      setLoading(true);
      setError('');
      setSuccess('');
      const token = await user.getIdToken();

      const selectedItem = {
        ...form,
        amount: String(form.amount),
        subtotal: form.subtotal ? String(form.subtotal) : '',
        tax_amount: form.tax_amount ? String(form.tax_amount) : '',
      };

      const response = await fetch('/api/manager/update/expenses', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ selectedItem }),
      });

      if (!response.ok) {
        const payload = await response.json().catch(() => ({}));
        throw new Error(payload.error || 'Failed to save expense.');
      }

      setSuccess('Expense saved.');
      resetAll();
    } catch (err) {
      console.error('Expense save failed:', err);
      setError(String(err.message || err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <HeaderBar page="Receipts" toggleSidebar={toggleSidebar} collapsed={collapsed} />

      <div className="d-flex justify-content-center p-4">
        <Card className="w-100" style={{ maxWidth: 980 }}>
          <Card.Body>
            <Card.Title className="mb-3">Receipt Scanner</Card.Title>
            <Card.Text className="text-muted">
              OCR runs locally in the browser. Review the parsed fields before saving.
            </Card.Text>

            {error ? <Alert variant="warning">{error}</Alert> : null}
            {success ? <Alert variant="success">{success}</Alert> : null}

            <Row className="g-4">
              <Col md={5}>
                <div className="mb-3 text-center">
                  {preview ? (
                    <Image src={preview} thumbnail className="w-100 mb-3" />
                  ) : (
                    <div className="border border-secondary rounded p-5 text-muted">
                      No image selected
                    </div>
                  )}

                  <Form.Control
                    ref={fileRef}
                    type="file"
                    accept="image/*"
                    capture="environment"
                    onChange={handleFile}
                  />

                  {loading && progress > 0 && progress < 100 ? (
                    <ProgressBar now={progress} label={`${progress}%`} className="mt-3" />
                  ) : null}
                </div>

                <Form.Group>
                  <Form.Label>OCR Text</Form.Label>
                  <Form.Control
                    as="textarea"
                    rows={10}
                    value={ocrText}
                    readOnly
                    placeholder="Scanned text will appear here."
                  />
                </Form.Group>
              </Col>

              <Col md={7}>
                <Form onSubmit={handleSubmit}>
                  <Row className="g-3">
                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>Expense Date</Form.Label>
                        <Form.Control
                          type="date"
                          name="expense_date"
                          value={form.expense_date}
                          onChange={handleChange}
                          required
                        />
                      </Form.Group>
                    </Col>

                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>Category</Form.Label>
                        <Form.Control
                          name="category"
                          value={form.category}
                          onChange={handleChange}
                          placeholder="Fuel, Materials, Meals..."
                          required
                        />
                      </Form.Group>
                    </Col>

                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>Total Amount</Form.Label>
                        <Form.Control
                          type="number"
                          step="0.01"
                          name="amount"
                          value={form.amount}
                          onChange={handleChange}
                          required
                        />
                      </Form.Group>
                    </Col>

                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>Vendor Name</Form.Label>
                        <Form.Control
                          name="vendor_name"
                          value={form.vendor_name}
                          onChange={handleChange}
                          placeholder="Optional unless schema supports it"
                        />
                      </Form.Group>
                    </Col>

                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>Subtotal</Form.Label>
                        <Form.Control
                          type="number"
                          step="0.01"
                          name="subtotal"
                          value={form.subtotal}
                          onChange={handleChange}
                        />
                      </Form.Group>
                    </Col>

                    <Col md={6}>
                      <Form.Group>
                        <Form.Label>Tax Amount</Form.Label>
                        <Form.Control
                          type="number"
                          step="0.01"
                          name="tax_amount"
                          value={form.tax_amount}
                          onChange={handleChange}
                        />
                      </Form.Group>
                    </Col>

                    <Col xs={12}>
                      <Form.Group>
                        <Form.Label>Description</Form.Label>
                        <Form.Control
                          as="textarea"
                          rows={4}
                          name="description"
                          value={form.description}
                          onChange={handleChange}
                          required
                        />
                      </Form.Group>
                    </Col>
                  </Row>

                  <div className="d-flex gap-2 mt-4">
                    <Button type="submit" disabled={!canSubmit} className="flex-grow-1">
                      {loading ? (
                        <>
                          <Spinner as="span" animation="border" size="sm" /> Saving…
                        </>
                      ) : (
                        'Save Expense'
                      )}
                    </Button>
                    <Button type="button" variant="outline-secondary" onClick={resetAll}>
                      Reset
                    </Button>
                  </div>
                </Form>
              </Col>
            </Row>
          </Card.Body>
        </Card>
      </div>
    </div>
  );
}

export default ReceiptScanner;
