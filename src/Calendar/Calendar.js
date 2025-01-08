import React, { use, useCallback, useState } from 'react';
import { Calendar as BigCalendar, stringOrData, Views, momentLocalizer, Day } from 'react-big-calendar';
import moment from 'moment';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import 'react-big-calendar/lib/addons/dragAndDrop/styles.css';
import './Calendar.css';
import withDragAndDrop from 'react-big-calendar/lib/addons/dragAndDrop';

const localizer = momentLocalizer(moment);
const DnDCalendar = withDragAndDrop(BigCalendar);

export default function CalendarContainer({toggleSidebar, collapsed}) {
  // ====== 1) SCHEDULED EVENTS IN THE CALENDAR  ======
  const [scheduledEvents, setScheduledEvents] = useState([
    {
      id: 1,
      title: 'Existing Event',
      start: new Date(),
      end: new Date(new Date().setHours(new Date().getHours() + 1)),
    },
  ]);
  const [unscheduledEvents, setUnscheduledEvents] = useState([
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
  const [workerSchedule, setWorkerSchedule] = useState([
    {
      id:1,
      workerId: 1,
      title: 'Existing Event',
      start: new Date(),
      end: new Date(new Date().setHours(new Date().getHours() + .5))
    }
  ]);
  const events = [
    ...scheduledEvents.map((event) => ({
      ...event,
      calendar: 'scheduledEvents',
    })),
    ...workerSchedule.map((event) => ({
      ...event,
      calendar: 'workerSchedule',
    }))
  ]

  const getCalendar = (event) => {
    switch (event.calendar) {
      case 'unscheduledEvents':
        return unscheduledEvents;
        break;
      case 'scheduledEvents':
        return scheduledEvents;
        break;
      case 'workerSchedule':
        return workerSchedule;
        break;
      default:
        alert("Calendar Not Found!! Please try again or contact help.");
    }
  }

  const updateCalendar = (event, updated) => {
    switch (event.calendar) {
      case 'unscheduledEvents':
        setUnscheduledEvents(updated);
        break;
      case 'scheduledEvents':
        setScheduledEvents(updated);
        break;
      case 'workerSchedule':
        setWorkerSchedule(updated);
        break;
      default:
        alert("Calendar Not Updated!! Please try again or contact help.");
      };
  }

  const eventStyleGetter = (event) => {
    let backgroundColor;
    switch (event.calendar) {
      case 'unscheduledEvents':
        backgroundColor = '#FFFFFF'; // Blue
        break;
      case 'scheduledEvents':
        backgroundColor = '#28a745'; // Green
        break;
      case 'workerSchedule':
        backgroundColor = 'blue'; // Green
        break;
      default:
        backgroundColor = '#6c757d'; // Gray
    }
  
    return {
      style: {
        backgroundColor,
        color: 'white',
      },
    };
  };

  // ====== 2) RBC STATE: DATE & VIEW  ======
  const [currentDate, setCurrentDate] = useState(new Date());
  const [currentView, setCurrentView] = useState(Views.MONTH);

  // ====== 4) DRAG / RESIZE EXISTING EVENTS ON CALENDAR ======
  const onEventChange = ({ event, start, end }) => {
    var calendar = getCalendar(event);
    const updated = calendar.map((evt) =>
      evt.id === event.id ? { ...evt, start, end } : evt
    );
    updateCalendar(event, updated);
  }


  const [selectedEvent, setSelectedEvent] = useState(null);

  // 4. Input states for editing the event
  const [editTitle, setEditTitle] = useState('');
  const [editDescription, setEditDescription] = useState('');
  const [editStart, setEditStart] = useState('');
  const [editEnd, setEditEnd] = useState('');

  // 5. Handle event click
  const handleSelectEvent = (event) => {
    setSelectedEvent(event);
    // Pre-fill form inputs with event data
    setEditTitle(event.title || '');
    setEditDescription(event.description || '');
    setEditStart(event.start || '');
    setEditEnd(event.end || '');
  };

  // 6. Close the modal without saving
  const handleCloseModal = () => {
    setSelectedEvent(null);
  };


  const handleSaveChanges = () => {
    if (selectedEvent) {
      var calendar;
      var calendar = getCalendar(selectedEvent);
      const updatedEvents = calendar.map((evt) => {
        if (evt.id === selectedEvent.id) {
          return {
            ...evt,
            title: editTitle,
            description: editDescription
          };
        }
        return evt;
      });
      updateCalendar(selectedEvent, updatedEvents);
    }
    // Close the modal
    setSelectedEvent(null);
  };

  // ====== 5) MANUAL PARSE ON DROP FROM OUTSIDE (SIDEBAR) ======
  const [draggedEvent, setDraggedEvent] = useState(null);

  const handleDropFromOutside = useCallback(
    ({ start, end, allDay: isAllDay}) =>  {
    if (!draggedEvent) return;
    
    const startDate = new Date(start);
    const durationHours = draggedEvent.durationHours || 1;
    const endDate = new Date(startDate);
    endDate.setHours(endDate.getHours() + durationHours);
    
    const newEvent = {
      id: Date.now(), // Simple unique ID; consider using UUID for larger applications
      title: draggedEvent.title,
      start: startDate,
      end: endDate,
      calendar: 'scheduledEvents',
    };

    const isDuplicate = scheduledEvents.some(
      (evt) =>
        evt.title === newEvent.title &&
        evt.start.getTime() === newEvent.start.getTime()
    );

    if (!isDuplicate) {
      setScheduledEvents((prevEvents) => [...prevEvents, newEvent]);
    }
    setTimeout(() => setDraggedEvent(null), 0);
    },
    [draggedEvent, scheduledEvents, setDraggedEvent]
  );

  // ====== 6) MONTH-DAY CLICK => GO TO WEEK VIEW ON THAT DAY ======
  const handleSelectSlot = (slotInfo) => {
    setCurrentDate(slotInfo.start);
    if(currentView != Views.DAY) {
      setCurrentView(Views.WEEK);
    }
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
            onDragStart={() => setDraggedEvent(item)}
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
          eventPropGetter={eventStyleGetter}
          date={currentDate}
          view={currentView}
          onNavigate={(date) => setCurrentDate(date)}
          onView={(view) => setCurrentView(view)}
          defaultView={Views.MONTH}
          views={[Views.MONTH, Views.WEEK, Views.DAY]}
          selectable
          dragFromOutsideItem={() => draggedEvent}
          onDropFromOutside={handleDropFromOutside}
          onSelectSlot={handleSelectSlot}
          onEventDrop={onEventChange}
          onEventResize={onEventChange}
          onSelectEvent={handleSelectEvent}
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
            <label>
              Start:
              <textarea
                rows="3"
                value={editStart}
                onChange={(e) => setEditStart(e.target.value)}
              />
            </label>
            <label>
              End:
              <textarea
                rows="3"
                value={editEnd}
                onChange={(e) => setEditEnd(e.target.value)}
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