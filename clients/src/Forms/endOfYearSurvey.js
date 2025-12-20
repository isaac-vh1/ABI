
// src/components/AcresFeedbackForm.jsx
import React, { useState } from "react";
import {
  Container,
  Row,
  Col,
  Form,
  Button,
  Badge,
  InputGroup,
} from "react-bootstrap";
import styles from "../Forms/endOfYearSurvey.module.css";

const SERVICES = [
  "Lawn mowing & edging",
  "Garden bed maintenance",
  "Seasonal cleanups (spring/fall)",
  "Tree & shrub trimming",
  "Irrigation setup & tuning",
  "Mulch/soil delivery & spread",
  "Aeration & dethatching",
  "Fertilization & weed control",
  "Snow & ice management",
];

const initialState = {
  clientName: "",
  services: [],
  ratings: {
    quality: 0,
    professionalism: 0,
    schedulingEase: 0,
    paymentEase: 0,
    timeliness: 0,
    costValue: 0,
  },
  hoursWorked: "",
  visitsCount: "",
  totalCost: "",
  likedMost: "",
  improveNextYear: "",
  additionalComments: "",
  continueServices: "",
  reasonStop: "",
  newServices: "",
};

export default function AcresFeedbackForm({ onSubmit }) {
  const [data, setData] = useState(initialState);
  const [submitting, setSubmitting] = useState(false);

  const toggleService = (service) => {
    setData((prev) => {
      const selected = new Set(prev.services);
      selected.has(service) ? selected.delete(service) : selected.add(service);
      return { ...prev, services: Array.from(selected) };
    });
  };

  const updateRating = (key, value) => {
    setData((prev) => ({
      ...prev,
      ratings: { ...prev.ratings, [key]: Number(value) },
    }));
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);

    // Assemble payload
    const payload = {
      ...data,
      submittedAt: new Date().toISOString(),
    };

    try {
      // Wire this up to your API/email/Sheets/etc.
      // For now we'll just log it.
      console.log("Feedback payload:", payload);

      if (typeof onSubmit === "function") {
        await onSubmit(payload);
      }

      // Clear form on success
      setData(initialState);
    } finally {
      setSubmitting(false);
    }
  };

  const RatingField = ({ id, label }) => (
    <Form.Group className="mb-3">
      <Form.Label className="d-flex justify-content-between align-items-center">
        <span>{label}</span>
        {/* Optional helper badge */}
        <Badge bg="secondary" pill>
          1–5
        </Badge>
      </Form.Label>
      <div
        className="star-rating"
        role="group"
        aria-label={`${label} rating from 1 to 5`}
      >
        {[1, 2, 3, 4, 5].map((n) => (
          <React.Fragment key={`${id}-${n}`}>
            <input
              className="visually-hidden"
              type="radio"
              name={id}
              id={`${id}-${n}`}
              value={n}
              checked={data.ratings[id] === n}
              onChange={(e) => updateRating(id, e.target.value)}
            />
            <label
              htmlFor={`${id}-${n}`}
              className="star"
              aria-label={`${n} star${n > 1 ? "s" : ""}`}
            >
              ★
            </label>
          </React.Fragment>
        ))}
      </div>
      <div className="sr-only" aria-live="polite">
        {data.ratings[id] ? `${label}: ${data.ratings[id]} of 5` : "No rating"}
      </div>
    </Form.Group>
  );

  return (
    <Container fluid="md" className="py-4">
      <Row className="justify-content-center">
        <Col xs={12} lg={10} xl={8}>
          <div className="p-3 p-md-4 bg-white rounded shadow-sm">
            <header className="mb-4 text-center">
              <h1 className="h4 mb-1">Acres by Isaac</h1>
              <p className="text-muted mb-0">
                Year‑End Client Feedback (Optional Name)
              </p>
            </header>

            <Form onSubmit={handleSubmit} noValidate>
              {/* Client Name (Optional) */}
              <Form.Group className="mb-3" controlId="clientName">
                <Form.Label>Client Name (Optional)</Form.Label>
                <Form.Control
                  type="text"
                  name="clientName"
                  placeholder="e.g., Jane Doe"
                  value={data.clientName}
                  onChange={handleChange}
                />
              </Form.Group>

              {/* Services used */}
              <Form.Group className="mb-3">
                <Form.Label>Which services did you use this year?</Form.Label>
                <Row xs={1} sm={2} md={3} className="g-2">
                  {SERVICES.map((service) => (
                    <Col key={service}>
                      <Form.Check
                        type="checkbox"
                        id={`svc-${service}`}
                        label={service}
                        checked={data.services.includes(service)}
                        onChange={() => toggleService(service)}
                      />
                    </Col>
                  ))}
                </Row>
              </Form.Group>

              {/* Ratings */}
              <Row xs={1} md={2} className="g-3">
                <Col>
                  <RatingField id="quality" label="Quality of work" />
                </Col>
                <Col>
                  <RatingField id="professionalism" label="Professionalism" />
                </Col>
                <Col>
                  <RatingField
                    id="schedulingEase"
                    label="Ease of scheduling"
                  />
                </Col>
                <Col>
                  <RatingField id="paymentEase" label="Ease of payment" />
                </Col>
                <Col>
                  <RatingField id="timeliness" label="Timeliness of completion" />
                </Col>
                <Col>
                  <RatingField id="costValue" label="Cost / value for money" />
                </Col>
              </Row>

              {/* Hours / Quantity / Cost */}
              <Row xs={1} md={3} className="g-3 mt-1">
                <Col>
                  <Form.Group controlId="hoursWorked">
                    <Form.Label>
                      Approx. total hours of service (this season)
                    </Form.Label>
                    <Form.Control
                      type="number"
                      name="hoursWorked"
                      min="0"
                      step="0.1"
                      placeholder="e.g., 12.5"
                      value={data.hoursWorked}
                      onChange={handleChange}
                      inputMode="decimal"
                    />
                  </Form.Group>
                </Col>
                <Col>
                  <Form.Group controlId="visitsCount">
                    <Form.Label>Approx. number of service visits</Form.Label>
                    <Form.Control
                      type="number"
                      name="visitsCount"
                      min="0"
                      step="1"
                      placeholder="e.g., 8"
                      value={data.visitsCount}
                      onChange={handleChange}
                      inputMode="numeric"
                    />
                  </Form.Group>
                </Col>
                <Col>
                  <Form.Group controlId="totalCost">
                    <Form.Label>Total cost (optional)</Form.Label>
                    <InputGroup>
                      <InputGroup.Text>$</InputGroup.Text>
                      <Form.Control
                        type="number"
                        name="totalCost"
                        min="0"
                        step="0.01"
                        placeholder="e.g., 425.00"
                        value={data.totalCost}
                        onChange={handleChange}
                        inputMode="decimal"
                      />
                    </InputGroup>
                  </Form.Group>
                </Col>
              </Row>

              {/* Open feedback */}
              <Form.Group className="mt-3" controlId="likedMost">
                <Form.Label>What did you like most about our services?</Form.Label>
                <Form.Control
                  as="textarea"
                  rows={3}
                  name="likedMost"
                  placeholder="Tell us what stood out—in work quality, communication, or results."
                  value={data.likedMost}
                  onChange={handleChange}
                />
              </Form.Group>

              <Form.Group className="mt-3" controlId="improveNextYear">
                <Form.Label>What can we improve for next year?</Form.Label>
                <Form.Control
                  as="textarea"
                  rows={3}
                  name="improveNextYear"
                  placeholder="Scheduling, pricing, specific services, etc."
                  value={data.improveNextYear}
                  onChange={handleChange}
                />
              </Form.Group>

              <Form.Group className="mt-3" controlId="additionalComments">
                <Form.Label>Other comments</Form.Label>
                <Form.Control
                  as="textarea"
                  rows={3}
                  name="additionalComments"
                  placeholder="Anything else you’d like us to know."
                  value={data.additionalComments}
                  onChange={handleChange}
                />
              </Form.Group>

              {/* Continue services */}
              <Form.Group className="mt-4" controlId="continueServices">
                <Form.Label>Continue services next season?</Form.Label>
                <div className="d-flex flex-wrap gap-3">
                  {["Yes", "No", "Maybe"].map((opt) => (
                    <Form.Check
                      key={opt}
                      inline
                      type="radio"
                      name="continueServices"
                      id={`cont-${opt}`}
                      label={opt}
                      value={opt}
                      checked={data.continueServices === opt}
                      onChange={handleChange}
                    />
                  ))}
                </div>
              </Form.Group>

              {data.continueServices === "No" && (
                <Form.Group className="mt-3" controlId="reasonStop">
                  <Form.Label>
                    If “No”, why—and what could earn back your business?
                  </Form.Label>
                  <Form.Control
                    as="textarea"
                    rows={3}
                    name="reasonStop"
                    placeholder="Share your reasons and how we can improve."
                    value={data.reasonStop}
                    onChange={handleChange}
                  />
                </Form.Group>
              )}

              {/* New services */}
              <Form.Group className="mt-3" controlId="newServices">
                <Form.Label>
                  Any services you’d like us to offer next season?
                </Form.Label>
                <Form.Control
                  as="textarea"
                  rows={3}
                  name="newServices"
                  placeholder="e.g., drip irrigation, native plant installs, composting, etc."
                  value={data.newServices}
                  onChange={handleChange}
                />
              </Form.Group>

              {/* Actions */}
              <div className="mt-4 d-flex flex-column flex-sm-row gap-3">
                <Button
                  type="submit"
                  variant="success"
                  className="w-100"
                  disabled={submitting}
                >
                  {submitting ? "Submitting…" : "Submit Feedback"}
                </Button>
                <Button
                  type="button"
                  variant="outline-secondary"
                  className="w-100"
                  onClick={() => setData(initialState)}
                  disabled={submitting}
                >
                  Reset Form
                </Button>
              </div>
            </Form>
          </div>
        </Col>
      </Row>
    </Container>
  );
}
