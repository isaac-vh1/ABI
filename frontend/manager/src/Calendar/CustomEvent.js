// src/components/CustomEvent.js

import React from 'react';

const CustomEvent = ({ event, onDoubleClick }) => {
  const handleDoubleClick = (e) => {
    e.preventDefault();
    onDoubleClick(event);
  };

  return (
    <div onDoubleClick={handleDoubleClick} style={{ cursor: 'pointer', height:'100%' }}>
      <strong>{event.title}</strong>
      <h5>{event.description}</h5>
    </div>
  );
};

export default CustomEvent;