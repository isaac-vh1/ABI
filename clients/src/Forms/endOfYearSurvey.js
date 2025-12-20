
import React, { useState } from 'react';
import {
  Form,
  Button,
  Row,
  Col,
  InputGroup,
  Badge,
  Stack,
  Alert
} from 'react-bootstrap';

/**
 * SeasonalServiceFeedbackForm
 * - Uses React Bootstrap components
 * - Client-friendly wording
 * - Clear grouping and accessible inputs
 */
export default function SeasonalServiceFeedbackForm({ onSubmit }) {
  // Example service options (customize to your business)
  const serviceOptions = [
    'Lawn Care',
    'Snow Removal',
    'Gutter Cleaning',
    'Tree Trimming',
    'Power Washing',
    'Landscape Design',
    'Other',
  ];

  // Controlled state
  const [form, setForm] = useState({
    clientName: '',
    servicesUsed: [],
    ratings: {
      quality: 0,
      professionalism: 0,
      schedulingEase: 0,
      paymentEase: 0,
      timelinessHours: '',
      costSatisfaction: 0,
      serviceVolume: '',
    },
    likesMost: '',
    improveNextYear: '',
    otherComments: '',
    continueServices: 'yes', // 'yes' | 'no' | 'unsure'
    discontinueReason: '',
    canEarnBack: '',
    newServicesWishList: '',
  });

  const [validated, setValidated] = useState(false);
  const [submitted, setSubmitted] = useState(false);

  const scaleLabels = ['Poor', 'Fair', 'Good', 'Very Good', 'Excellent'];

  const handleChange = (path, value) => {
    // Helper for nested state updates
    setForm(prev => {
      const keys = path.split('.');
      const copy = { ...prev };
      let cursor = copy;
      for (let i = 0; i < keys.length - 1; i++) {
        cursor[keys[i]] = { ...cursor[keys[i]] };
        cursor = cursor[keys[i]];
      }
      cursor[keys.at(-1)] = value;
      return copy;
    });
  };

  const toggleService = (svc) => {
    setForm(prev => {
      const set = new Set(prev.servicesUsed);
      if (set.has(svc)) set.delete(svc);
      else set.add(svc);
      return { ...prev, servicesUsed: Array.from(set) };
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    setValidated(true);

    // Minimal validation: at least one service selected
    const valid = form.servicesUsed.length > 0;

    if (!valid) return;

    setSubmitted(true);
    // Bubble up to parent or log
    onSubmit?.(form);
    // Optionally clear:
    // setForm({ ...initial });
  };

  return (
    <Form noValidate validated={validated} onSubmit={handleSubmit} className="p-3">
      <h4 className="mb-3">Seasonal Services Feedback</h4>
      <p className="text-muted">
        Your feedback helps us improve for next season. This takes ~2–3 minutes.
      </p>

      {/* Client Info */}
      <fieldset className="mb-4">
        <legend className="h6">Client Info</legend>
        <Form.Group controlId="clientName">
          <Form.Label>Client Name <span className="text-muted">(optional)</span></Form.Label>
          <Form.Control
            type="text"
            placeholder="e.g., Jane Doe"
            value={form.clientName}
            onChange={(e) => handleChange('clientName', e.target.value)}
          />
        </Form.Group>
      </fieldset>

      {/* Services Used */}
      <fieldset className="mb-4">
        <legend className="h6">Which services did you use? <Badge bg="secondary">Required</Badge></legend>
        <Row xs={1} sm={2} md={3}>
          {serviceOptions.map((svc) => (
            <Col key={svc} className="mb-2">
              <Form.Check
                type="checkbox"
                id={`svc-${svc}`}
                label={svc}
                checked={form.servicesUsed.includes(svc)}
                onChange={() => toggleService(svc)}
                required={form.servicesUsed.length === 0} // only triggers when none selected
              />
            </Col>
          ))}
        </Row>
      </fieldset>

      {/* Experience Ratings */}
      <fieldset className="mb-4">
        <legend className="h6">Your Experience</legend>

        {/* Helper component-like patterns for scales */}
        <Row className="gy-3">
          <Col md={6}>
            <Form.Label>Quality</Form.Label>
            <Stack direction="horizontal" gap={2}>
              {scaleLabels.map((lbl, idx) => (
                <Form.Check
                  key={lbl}
                  inline
                  type="radio"
                  name="quality"
                  id={`quality-${idx + 1}`}
                  label={lbl}
                  checked={form.ratings.quality === idx + 1}
                  onChange={() => handleChange('ratings.quality', idx + 1)}
                />
              ))}
            </Stack>
          </Col>

          <Col md={6}>
            <Form.Label>Professionalism</Form.Label>
            <Stack direction="horizontal" gap={2}>
              {scaleLabels.map((lbl, idx) => (
                <Form.Check
                  key={lbl}
                  inline
                  type="radio"
                  name="professionalism"
                  id={`professionalism-${idx + 1}`}
                  label={lbl}
                  checked={form.ratings.professionalism === idx + 1}
                  onChange={() => handleChange('ratings.professionalism', idx + 1)}
                />
              ))}
            </Stack>
          </Col>

          <Col md={6}>
            <Form.Label>Ease of Scheduling</Form.Label>
            <Stack direction="horizontal" gap={2}>
              {scaleLabels.map((lbl, idx) => (
                <Form.Check
                  key={lbl}
                  inline
                  type="radio"
                  name="schedulingEase"
                  id={`schedulingEase-${idx + 1}`}
                  label={lbl}
                  checked={form.ratings.schedulingEase === idx + 1}
                  onChange={() => handleChange('ratings.schedulingEase', idx + 1)}
                />
              ))}
            </Stack>
          </Col>

          <Col md={6}>
            <Form.Label>Ease of Payment</Form.Label>
            <Stack direction="horizontal" gap={2}>
              {scaleLabels.map((lbl, idx) => (
                <Form.Check
                  key={lbl}
                  inline
                  type="radio"
                  name="paymentEase"
                  id={`paymentEase-${idx + 1}`}
                  label={lbl}
                  checked={form.ratings.paymentEase === idx + 1}
                  onChange={() => handleChange('ratings.paymentEase', idx + 1)}
                />
              ))}
            </Stack>
          </Col>

          <Col md={6}>
            <Form.Label>Cost Satisfaction</Form.Label>
            <Stack direction="horizontal" gap={2}>
              {scaleLabels.map((lbl, idx) => (
                <Form.Check
                  key={lbl}
                  inline
                  type="radio"
                  name="costSatisfaction"
                  id={`costSatisfaction-${idx + 1}`}
                  label={lbl}
                  checked={form.ratings.costSatisfaction === idx + 1}
                  onChange={() => handleChange('ratings.costSatisfaction', idx + 1)}
                />
              ))}
            </Stack>
          </Col>

          <Col md={6}>
            <Form.Label>Timeliness (hours to completion)</Form.Label>
            <InputGroup>
              <Form.Control
                type="number"
                min="0"
                step="0.5"
                placeholder="e.g., 6"
                value={form.ratings.timelinessHours}
                onChange={(e) => handleChange('ratings.timelinessHours', e.target.value)}
              />
              <InputGroup.Text>hours</InputGroup.Text>
            </InputGroup>
            <Form.Text className="text-muted">
              Approximate total hours for your service completion.
            </Form.Text>
          </Col>

          <Col md={6}>
            <Form.Label>Amount of Services (count)</Form.Label>
            <InputGroup>
              <Form.Control
                type="number"
                min="0"
                step="1"
                placeholder="e.g., 3"
                value={form.ratings.serviceVolume}
                onChange={(e) => handleChange('ratings.serviceVolume', e.target.value)}
              />
              <InputGroup.Text>tasks</InputGroup.Text>
            </InputGroup>
          </Col>
        </Row>
      </fieldset>

      {/* Open-Ended Feedback */}
      <fieldset className="mb-4">
        <legend className="h6">Open Feedback</legend>

        <Form.Group className="mb-3" controlId="likesMost">
          <Form.Label>What did you like most?</Form.Label>
          <Form.Control
            as="textarea"
            rows={3}
            placeholder="Tell us what stood out positively…"
            value={form.likesMost}
            onChange={(e) => handleChange('likesMost', e.target.value)}
          />
        </Form.Group>

        <Form.Group className="mb-3" controlId="improveNextYear">
          <Form.Label>What could we improve for next season?</Form.Label>
          <Form.Control
            as="textarea"
            rows={3}
            placeholder="Suggestions help us get better…"
            value={form.improveNextYear}
            onChange={(e) => handleChange('improveNextYear', e.target.value)}
          />
        </Form.Group>

        <Form.Group className="mb-3" controlId="otherComments">
          <Form.Label>Other comments</Form.Label>
          <Form.Control
            as="textarea"
            rows={3}
            placeholder="Anything else we should know?"
            value={form.otherComments}
            onChange={(e) => handleChange('otherComments', e.target.value)}
          />
        </Form.Group>
      </fieldset>

      {/* Continuation */}
      <fieldset className="mb-4">
        <legend className="h6">Next Season</legend>

        <Form.Group className="mb-3" controlId="continueServices">
          <Form.Label>Would you like to continue services next season?</Form.Label>
          <Stack direction="horizontal" gap={3}>
            {['yes', 'unsure', 'no'].map((val) => (
              <Form.Check
                key={val}
                type="radio"
                inline
                name="continueServices"
                id={`continue-${val}`}
                label={val[0].toUpperCase() + val.slice(1)}
                checked={form.continueServices === val}
                onChange={() => handleChange('continueServices', val)}
              />
            ))}
          </Stack>
        </Form.Group>

        {form.continueServices === 'no' && (
          <Row className="gy-3">
            <Col md={6}>
              <Form.Group controlId="discontinueReason">
                <Form.Label>If no, why?</Form.Label>
                <Form.Control
                  as="textarea"
                  rows={3}
                  placeholder="Share your reason for discontinuing…"
                  value={form.discontinueReason}
                  onChange={(e) => handleChange('discontinueReason', e.target.value)}
                />
              </Form.Group>
            </Col>
            <Col md={6}>
              <Form.Group controlId="canEarnBack">
                <Form.Label>Can we earn back your business? How?</Form.Label>
                <Form.Control
                  as="textarea"
                  rows={3}
                  placeholder="What would change your mind?"
                  value={form.canEarnBack}
                  onChange={(e) => handleChange('canEarnBack', e.target.value)}
                />
              </Form.Group>
            </Col>
          </Row>
        )}

        <Form.Group className="mt-3" controlId="newServicesWishList">
          <Form.Label>Any services you’d like us to offer next season?</Form.Label>
          <Form.Control
            as="textarea"
            rows={2}
            placeholder="e.g., irrigation setup, holiday lighting, pest control…"
            value={form.newServicesWishList}
            onChange={(e) => handleChange('newServicesWishList', e.target.value)}
          />
        </Form.Group>
      </fieldset>

      {submitted && (
        <Alert variant="success" className="mb-3">
          Thanks for your feedback! We’ve recorded your responses.
        </Alert>
      )}

      <div className="d-flex gap-2">
        <Button type="submit" variant="primary">Submit Feedback</Button>
        <Button
          type="button"
          variant="outline-secondary"
          onClick={() => {
            setForm({
              clientName: '',
              servicesUsed: [],
              ratings: {
                quality: 0,
                professionalism: 0,
                schedulingEase: 0,
                paymentEase: 0,
                timelinessHours: '',
                costSatisfaction: 0,
                serviceVolume: '',
              },
              likesMost: '',
              improveNextYear: '',
              otherComments: '',
              continueServices: 'yes',
              discontinueReason: '',
              canEarnBack: '',
              newServicesWishList: '',
            });
            setValidated(false);
            setSubmitted(false);
          }}
        >
          Reset
        </Button>
      </div>
    </Form>
  );
}
