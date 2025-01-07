import React, { useState } from 'react';
import { Calendar as BigCalendar, stringOrData, Views, momentLocalizer } from 'react-big-calendar';
import moment from 'moment';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import 'react-big-calendar/lib/addons/dragAndDrop/styles.css';
import './Calendar.css';
import withDragAndDrop from 'react-big-calendar/lib/addons/dragAndDrop';

const localizer = momentLocalizer(moment);
const DnDCalendar = withDragAndDrop(BigCalendar);

export default function CalendarContainer({toggleSidebar, collapsed}) {
  // ====== 1) SCHEDULED EVENTS IN THE CALENDAR  ======
  const [events, setEvents] = useState([
    {
      id: 1,
      title: 'Existing Event',
      start: new Date(),
      end: new Date(new Date().setHours(new Date().getHours() + 1)),
    },
  ]);

  // ====== 2) RBC STATE: DATE & VIEW  ======
  const [currentDate, setCurrentDate] = useState(new Date());
  const [currentView, setCurrentView] = useState(Views.MONTH);

  // ====== 3) UNSCHEDULED EVENTS (SIDEBAR) ======
  // Each has a duration in hours, an invoice, etc.
  const [unscheduledEvents] = useState([
    {
      id: 101,
      title: 'Cleaning Service',
      durationHours: 1,
    },
    {
      id: 102,
      title: 'Maintenance',
      durationHours: 2,
    },
    {
      id: 103,
      title: 'Consultation',
      durationHours: 3,
    },
  ]);

  // ====== 4) DRAG / RESIZE EXISTING EVENTS ON CALENDAR ======
  const onEventChange = ({ event, start, end }) => {
    const updated = events.map((evt) =>
      evt.id === event.id ? { ...evt, start, end } : evt
    );
    setEvents(updated);
  };

  const [selectedEvent, setSelectedEvent] = useState(null);

  // 4. Input states for editing the event
  const [editTitle, setEditTitle] = useState('');
  const [editDescription, setEditDescription] = useState('');

  // 5. Handle event click
  const handleSelectEvent = (event) => {
    setSelectedEvent(event);
    // Pre-fill form inputs with event data
    setEditTitle(event.title || '');
    setEditDescription(event.description || '');
  };

  // 6. Close the modal without saving
  const handleCloseModal = () => {
    setSelectedEvent(null);
  };


  const handleSaveChanges = () => {
    if (selectedEvent) {
      // Update the selected event in the 'events' array
      const updatedEvents = events.map((evt) => {
        if (evt.id === selectedEvent.id) {
          return {
            ...evt,
            title: editTitle,
            description: editDescription
          };
        }
        return evt;
      });
      setEvents(updatedEvents);
    }
    // Close the modal
    setSelectedEvent(null);
  };

  // ====== 5) MANUAL PARSE ON DROP FROM OUTSIDE (SIDEBAR) ======
  const handleDropFromOutside = (obj) => {
    const { start, end, allDay, nativeEvent } = obj;

    const draggedJson = nativeEvent.dataTransfer.getData('draggedEvent');
    if (!draggedJson) return; // if nothing found, stop.

    const draggedEvent = JSON.parse(draggedJson);
    if (!draggedEvent.durationHours) return; // safety check

    // Build new event using the 'durationHours'
    const startDate = new Date(start);
    const hours = parseInt(draggedEvent.durationHours, 10) || 1;
    const endDate = new Date(startDate);
    endDate.setHours(endDate.getHours() + hours);

    const newEvent = {
      id: new Date().getTime(),
      title: `${draggedEvent.title}`,
      start: startDate,
      end: endDate,
    };
    setEvents((prev) => [...prev, newEvent]);
  };

  // ====== 6) MONTH-DAY CLICK => GO TO WEEK VIEW ON THAT DAY ======
  const handleSelectSlot = (slotInfo) => {
    setCurrentDate(slotInfo.start);
    setCurrentView(Views.WEEK);
  };

  return (
    <div>
      <head>
        <title>Calendar</title>
      </head>
    <div style={{ display: 'flex' }}>
      {/* ===== SIDEBAR ===== */}
      <button className={`calendar-toggle ${collapsed ? 'collapsed' : ''}`} onClick={toggleSidebar}>☰</button>
      <div className={`calendar-drop-bar ${collapsed ? 'collapsed' : ''}`}>
        <h2>Unscheduled</h2>
        {unscheduledEvents.map((item) => (
          <div
            key={item.id}
            className='sidebar-widget'
            draggable
            onDragStart={(e) => {
              
            }}
          >
            <strong>{item.title}</strong>
            <div>ID: {item.id}</div>
            <div>Duration: {item.durationHours} hr(s)</div>
          </div>
        ))}
      </div>

      {/* ===== CALENDAR ===== */}
      <div className={`my-custom-calendar-container ${collapsed ? 'collapsed' : ''}`}>
      <article>
        <div className='top-bar'>
        <button className={`top-bar-button`} onClick={toggleSidebar}>☰</button>
        </div>
      </article>
      <article>
        <DnDCalendar
          className="my-custom-calendar" /* for our custom CSS */
          localizer={localizer}
          events={events}
          date={currentDate}
          view={currentView}
          onNavigate={(date) => setCurrentDate(date)}
          onView={(view) => setCurrentView(view)}
          defaultView={Views.MONTH}
          views={[Views.MONTH, Views.WEEK, Views.DAY]}
          selectable
          onSelectSlot={handleSelectSlot}
          onEventDrop={onEventChange}
          onEventResize={onEventChange}
          onSelectEvent={handleSelectEvent}
          onDropFromOutside={handleDropFromOutside}
          dragFromOutsideItem={null}
          startAccessor="start"
          endAccessor="end"
        />
        </article>
        </div>
        </div>
        {selectedEvent && (
        <div className="modal-backdrop" onClick={handleCloseModal}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>Edit Event</h2>
            <label>
              Title:
              <input
                type="text"
                value={editTitle}
                onChange={(e) => setEditTitle(e.target.value)}
              />
            </label>
            <label>
              Description:
              <textarea
                rows="3"
                value={editDescription}
                onChange={(e) => setEditDescription(e.target.value)}
              />
            </label>

            <div className="modal-buttons">
              <button onClick={handleSaveChanges}>Save</button>
              <button onClick={handleCloseModal}>Cancel</button>
            </div>
          </div>
        </div>)}
        </div>
  );
}