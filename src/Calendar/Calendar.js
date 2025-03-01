// src/components/CalendarContainer.js

import React, { useCallback, useState, useMemo, useRef} from 'react';
import { Calendar, Views, momentLocalizer } from 'react-big-calendar';
import moment from 'moment';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import 'react-big-calendar/lib/addons/dragAndDrop/styles.css';
import './Calendar.css';
import withDragAndDrop from 'react-big-calendar/lib/addons/dragAndDrop';
import CustomEvent from './CustomEvent';
import DraggableEvent from './DraggableEvent';
import HamburgerMenu from '../Components/HamburgerMenu';

import {
  scheduledEvents as initialScheduledEvents,
  unscheduledEvents as initialUnscheduledEvents,
  workerSchedule as initialWorkerSchedule
} from './events';

const localizer = momentLocalizer(moment);
const DnDCalendar = withDragAndDrop(Calendar);

export default function CalendarContainer({ toggleSidebar, collapsed }) {
  // ====== 1) INITIALIZE EVENTS ======
  const [scheduledEvents, setScheduledEvents] = useState(initialScheduledEvents || []);
  const [unscheduledEvents, setUnscheduledEvents] = useState(initialUnscheduledEvents || []);
  const [workerSchedule, setWorkerSchedule] = useState(initialWorkerSchedule || []);

  // ====== 2) UNIQUE ID COUNTER ======
  const eventIdRef = useRef(1000); // Starting from 1000 to avoid conflicts

  // ====== 3) COMBINE EVENTS ======
  const events = useMemo(() => [
    ...(scheduledEvents || []).map((event) => ({
      ...event,
      calendar: 'scheduledEvents',
    })),
    ...(workerSchedule || []).map((event) => ({
      ...event,
      calendar: 'workerSchedule',
    }))
  ], [scheduledEvents, workerSchedule]);

  const getCalendar = (event) => {
    switch (event.calendar) {
      case 'unscheduledEvents':
        return unscheduledEvents;
      case 'scheduledEvents':
        return scheduledEvents;
      case 'workerSchedule':
        return workerSchedule;
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

  // ====== 4) EVENT STYLE GETTER ======
  const eventStyleGetter = (event) => {
    let backgroundColor;
    switch (event.calendar) {
      case 'unscheduledEvents':
        backgroundColor = '#FFFFFF'; // White
        break;
      case 'scheduledEvents':
        backgroundColor = '#28a745'; // Green
        break;
      case 'workerSchedule':
        backgroundColor = 'blue'; // Blue
        break;
      default:
        backgroundColor = '#6c757d';  // Gray
    }

    return {
      style: {
        backgroundColor,
        color: event.calendar === 'unscheduledEvents' ? '#000000' : '#FFFFFF',
      },
    };
  };

  // ====== 5) RBC STATE: DATE & VIEW ======
  const [currentDate, setCurrentDate] = useState(new Date());
  const [currentView, setCurrentView] = useState(Views.MONTH);

  // ====== 6) SELECTED EVENT STATE ======
  const [selectedEvent, setSelectedEvent] = useState(null);
  const [editTitle, setEditTitle] = useState('');
  const [editDescription, setEditDescription] = useState('');
  const [editStart, setEditStart] = useState('');
  const [editEnd, setEditEnd] = useState('');

  // ====== 7) HANDLE EVENT SELECTION ======
  const handleDoubleClickEvent = (event) => {
    setSelectedEvent(event);
    setEditTitle(event.title || '');
    setEditDescription(event.description || '');
    setEditStart(event.start ? formatDateTimeLocal(event.start) : '');
    setEditEnd(event.end ? formatDateTimeLocal(event.end) : '');
  };

  const formatDateTimeLocal = (date) => {
    const pad = (n) => (n < 10 ? '0' + n : n);
    const year = date.getFullYear();
    const month = pad(date.getMonth() + 1);
    const day = pad(date.getDate());
    const hours = pad(date.getHours());
    const minutes = pad(date.getMinutes());
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  };

  // ====== 8) CLOSE THE MODAL ======
  const handleCloseModal = () => {
    setSelectedEvent(null);
  };

  // ====== 9) SAVE CHANGES ======
  const handleSaveChanges = () => {
    if (selectedEvent) {
      const updatedStart = editStart ? new Date(editStart) : selectedEvent.start;
      const updatedEnd = editEnd ? new Date(editEnd) : selectedEvent.end;

      const updatedEvent = {
        ...selectedEvent,
        title: editTitle,
        description: editDescription,
        start: updatedStart,
        end: updatedEnd,
      };

      var calendar = getCalendar(selectedEvent);
      const updated = calendar.map((evt) =>
        evt.id === selectedEvent.id ? updatedEvent : evt
      );
      updateCalendar(selectedEvent, updated);
    }
    setSelectedEvent(null);
  };

  // ====== 10) DRAGGED EVENT STATE ======
  const [draggedEvent, setDraggedEvent] = useState(null);

  // ====== 11) HANDLE DROP FROM OUTSIDE ======
  const handleDropFromOutside = useCallback(
    ({ start, end, allDay: isAllDay }) => {
      if (!draggedEvent) return;

      const startDate = new Date(start);
      const duration = draggedEvent.duration || 1;
      const endDate = new Date(startDate);
      endDate.setHours(endDate.getHours() + duration);

      // Generate a unique ID
      const newEventId = eventIdRef.current++;

      const newEvent = {
        id: newEventId, // Ensure unique ID
        title: draggedEvent.title,
        start: startDate,
        end: endDate,
        calendar: 'scheduledEvents',
        allDay: isAllDay, // Include allDay property
      };

      const isDuplicate = scheduledEvents.some(
        (evt) =>
          evt.title === newEvent.title &&
          evt.start.getTime() === newEvent.start.getTime()
      );

      if (!isDuplicate) {
        setScheduledEvents((prevEvents) => [...prevEvents, newEvent]);
        console.log('New event added:', newEvent);
      } else {
        console.log('Duplicate event detected, not adding:', newEvent);
      }

      setUnscheduledEvents((prevEvents) =>
        prevEvents.filter((evt) => evt.id !== draggedEvent.id)
      );

      setDraggedEvent(null);
    },
    [scheduledEvents, draggedEvent, setUnscheduledEvents]
  );

  // ====== 13) HANDLE SLOT SELECTION ======
  const handleSelectSlot = (slotInfo) => {
    setCurrentDate(slotInfo.start);
    if (currentView !== Views.DAY) {
      setCurrentView(Views.WEEK);
    }
  };

  const handleSelectEvent = () =>  {
    var cal = 1 + 1;
    console.log(cal);
  };

  const onEventChange = ({ event, start, end, isAllDay }) => {
    //if(!isAllDay && start.getDate() == end.getDate()) {
      var calendar = getCalendar(event);
      const updated = calendar.map((evt) =>
        evt.id === event.id ? { ...evt, start, end } : evt
      );
      updateCalendar(event, updated);
    //}
  }

  const handleOnDragStart = useCallback((item) => {
    const updatedEvent = {
      ...item,
      start: new Date(),
      end: new Date() + item.duration
    };
    console.log(updatedEvent);
    setDraggedEvent(item);
    console.log('Dragging event:', item);
  }, []);

  return (
    <div>
      <head>
        <title>Calendar</title>
      </head>
      <div>
        {/* ===== SIDEBAR ===== */}
        <div className={`calendar-toggle ${collapsed ? 'collapsed' : ''}`} onClick={toggleSidebar}><HamburgerMenu collapsed={collapsed} /></div>
        <div className={`calendar-drop-bar ${collapsed ? 'collapsed' : ''}`}>
          <h2>Unscheduled</h2>
          {(!unscheduledEvents || unscheduledEvents.length === 0 )? <p className='noJob'>No Jobs to Schedule</p> :
          unscheduledEvents.map((item) => (
            <DraggableEvent key={item.id} event={item} onDragStart={handleOnDragStart}/>
          ))}
        </div>

        {/* ===== CALENDAR ===== */}
        <div className={`my-custom-calendar-container ${collapsed ? 'collapsed' : ''}`}>
          <article>
            <div className='top-bar'>
              <div className={`top-bar-button ${collapsed ? 'collapsed' : ''}`} onClick={toggleSidebar}><HamburgerMenu collapsed={collapsed} /></div>
              <button className="button">View</button>
              <button className="button" onClick={handleDoubleClickEvent}>New Job</button>
            </div>
          </article>
          <article>
            <DnDCalendar
              className="my-custom-calendar"
              localizer={localizer}
              events={events}
              eventPropGetter={eventStyleGetter}
              date={currentDate}
              view={currentView}
              onNavigate={(date) => setCurrentDate(date)}
              onView={(view) => setCurrentView(view)}
              defaultView={Views.MONTH}
              views={[Views.MONTH, Views.WEEK, Views.DAY]}
              step={15}
              timeslots={4}
              selectable
              resizable
              dragFromOutsideItem={() => null}
              onDropFromOutside={handleDropFromOutside}
              onSelectSlot={handleSelectSlot}
              onEventDrop={onEventChange}
              onEventResize={onEventChange}
              onSelectEvent={handleSelectEvent}
              startAccessor="start"
              endAccessor="end"
              components={{
                event: (props) => (
                  <CustomEvent {...props} onDoubleClick={handleDoubleClickEvent} />
                ),
              }}
            />
          </article>
        </div>
      </div>

      {/* ===== MODAL FOR EDITING EVENTS ===== */}
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
              <input
                type="datetime-local"
                value={editStart}
                onChange={(e) => setEditStart(e.target.value)}
              />
            </label>
            <label>
              End:
              <input
                type="datetime-local"
                value={editEnd}
                onChange={(e) => setEditEnd(e.target.value)}
              />
            </label>

            <div className="modal-buttons">
              <button className="cancel-button" onClick={handleCloseModal}>Cancel</button>
              <button className="button" onClick={handleSaveChanges}>Save</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}