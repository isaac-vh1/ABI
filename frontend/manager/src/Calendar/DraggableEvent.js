// src/components/DraggableEvent.js

import React, { useState } from 'react';
import './DraggableEvent.css'; // Import your CSS styles

const DraggableEvent = ({ event, onDragStart }) => {
  const [isDragging, setIsDragging] = useState(false);

  const handleDragStart = () => {
    setIsDragging(true);
    onDragStart(event);
  };

  const handleDragEnd = () => {
    console.log("Dragging End: ", event)
    setIsDragging(false);
  };

  return (
    <div
      className={`draggable-event ${isDragging ? 'dragging' : ''}`}
      draggable
      onDragStart={handleDragStart}
      onDragEnd={handleDragEnd}
      role="button"
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          handleDragStart();
        }
      }}
    >
      <strong>{event.title}</strong>
      <div>ID: {event.id}</div>
      <div>Duration: {event.duration} hr(s)</div>
    </div>
  );
};

export default DraggableEvent;