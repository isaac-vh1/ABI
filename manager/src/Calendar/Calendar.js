import React, { useCallback, useState, useMemo, useRef, useEffect } from 'react';
import { Calendar, Views, momentLocalizer } from 'react-big-calendar';
import moment from 'moment';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import 'react-big-calendar/lib/addons/dragAndDrop/styles.css';
import './Calendar.css';
import withDragAndDrop from 'react-big-calendar/lib/addons/dragAndDrop';
import CustomEvent from './CustomEvent';
import DraggableEvent from './DraggableEvent';
import HamburgerMenu from '../Components/HamburgerMenu';
import { auth } from '../firebase';
import { Helmet } from 'react-helmet';
import { Button, Spinner } from 'react-bootstrap';

const localizer = momentLocalizer(moment);
const DnDCalendar = withDragAndDrop(Calendar);

export default function CalendarContainer({ toggleSidebar, collapsed }) {
  const [scheduledEvents, setScheduledEvents] = useState([]);
  const [unscheduledEvents, setUnscheduledEvents] = useState([]);
  const [workerSchedule, setWorkerSchedule] = useState([]);
  const [clients, setClients] = useState([]);
  const [error, setError] = useState(false);
  const [loading, setLoading] = useState(true);
  const user = auth.currentUser;
  const eventIdRef = useRef(1000);
  const [currentDate, setCurrentDate] = useState(new Date());
  const [currentView, setCurrentView] = useState(Views.MONTH);
  const [selectedEvent, setSelectedEvent] = useState(null);
  const [draggedEvent, setDraggedEvent] = useState(null);

  // ===== FETCH EVENTS =====
  useEffect(() => {
    if (!user) return;
    user.getIdToken().then(token => {
      fetch('/api/manager/events', {
        headers: { Authorization: `Bearer ${token}` },
      })
        .then((r) => r.json())
        .then((data) => {
          setScheduledEvents(formatEvents(data[0]) ?? []);
          setUnscheduledEvents(formatEvents(data[1]) ?? []);
          setWorkerSchedule(formatEvents(data[2]) ?? []);
          setClients(formatEvents(data[3]) ?? []);
          eventIdRef.current = data[4] ?? 1000;
        })
        .catch((e) => {
          console.error(e);
          setError(true);
        })
        .finally(() => setLoading(false));
    });
  }, [user]);

  // ===== FORMAT EVENTS =====
  const formatEvents = (rows = []) =>
    rows.map(
      ({
        id,
        client_id,
        title,
        description,
        calendar,
        start,
        end,
        duration_h,
      }) => {
        const startDate = new Date(start);
        const endDate =
          end != null
            ? new Date(end)
            : new Date(startDate.getTime() + (duration_h ?? 1) * 60 * 60 * 1000);
            console.log(events.map(e => [e.start, e.end, typeof e.start, typeof e.end]));
        return {
          id,
          client_id,
          title,
          start: startDate,
          end: endDate,
          calendar,
          description: description ?? '',
        };
      }
    );

  // ===== SAVE CHANGES =====
  const saveChanges = (items) => {
    if (!user) return;
    const selectedItem = Object.fromEntries(
      Object.entries(items).map(([key, value]) => [
        key,
        {
          ...value,
          start: new Date(value.start).toUTCString(),
        },
      ])
    );

    user.getIdToken().then(async (token) => {
      try {
        const res = await fetch('/api/manager/update/calendar', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({ selectedItem }),
        });
        const data = await res.json();
        if (data !== "true") {
          setError(true);
          throw new Error('Update failed');
        }
      } catch (e) {
        console.error(e);
        setError(true);
      }
    });
  };

  // ===== COMBINE EVENTS =====
  const events = useMemo(() => [
    ...(scheduledEvents || []).map((event) => ({ ...event, calendar: 'scheduledEvents' })),
    ...(workerSchedule || []).map((event) => ({ ...event, calendar: 'workerSchedule' })),
  ], [scheduledEvents, workerSchedule]);

  // ===== CALENDAR HELPERS =====
  const getCalendar = (event) => {
    switch (event.calendar) {
      case 'unscheduledEvents': return unscheduledEvents;
      case 'scheduledEvents': return scheduledEvents;
      case 'workerSchedule': return workerSchedule;
      default: alert("Calendar Not Found!!"); return [];
    }
  };

  const updateCalendar = (event, updated) => {
    switch (event.calendar) {
      case 'unscheduledEvents': setUnscheduledEvents(updated); break;
      case 'scheduledEvents': setScheduledEvents(updated); break;
      case 'workerSchedule': setWorkerSchedule(updated); break;
      default: alert("Calendar Not Updated!!");
    }
  };

  const eventStyleGetter = (event) => {
    let backgroundColor;
    switch (event.calendar) {
      case 'unscheduledEvents': backgroundColor = '#FFFFFF'; break;
      case 'scheduledEvents': backgroundColor = '#28a745'; break;
      case 'workerSchedule': backgroundColor = 'blue'; break;
      default: backgroundColor = '#6c757d';
    }
    return {
      style: { backgroundColor, color: event.calendar === 'unscheduledEvents' ? '#000' : '#fff' }
    };
  };

  const formatDateTimeLocal = (date) => {
    if (!date) return '';
    const pad = n => n < 10 ? '0' + n : n;
    return `${date.getFullYear()}-${pad(date.getMonth()+1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  };

  // ===== EVENT HANDLERS =====
  const handleDoubleClickEvent = (event) => setSelectedEvent(event);
  const handleCloseModal = () => setSelectedEvent(null);
  const handleSelectSlot = (slotInfo) => {
    setCurrentDate(slotInfo.start);
    if (currentView !== Views.DAY) setCurrentView(Views.WEEK);
  };

  const onEventChange = ({ event, start, end }) => {
    const calendar = getCalendar(event);
    const updated = calendar.map((evt) =>
      evt.id === event.id ? { ...evt, start: new Date(start), end: new Date(end) } : evt
    );
    updateCalendar(event, updated);
    saveChanges({ ...event, start: new Date(start), end: new Date(end) });
  };

  const handleOnDragStart = useCallback((item) => {
    setDraggedEvent(item);
  }, []);

  const handleDropFromOutside = useCallback(({ start }) => {
    if (!draggedEvent) return;
    const startDate = new Date(start);
    const duration = draggedEvent.duration ?? 1;
    const endDate = new Date(startDate.getTime() + duration * 60*60*1000);

    const newEvent = {
      ...draggedEvent,
      start: startDate,
      end: endDate,
      calendar: 'scheduledEvents',
    };

    const isDuplicate = scheduledEvents.some(
      evt => evt.title === newEvent.title && evt.start.getTime() === newEvent.start.getTime()
    );

    if (!isDuplicate) setScheduledEvents(prev => [...prev, newEvent]);
    setUnscheduledEvents(prev => prev.filter(evt => evt.id !== draggedEvent.id));
    saveChanges(newEvent);
    setDraggedEvent(null);
  }, [draggedEvent, scheduledEvents]);

  const handleNewUnscheduledEvent = () => {
    const newEvent = {
      id: eventIdRef.current++,
      title: '',
      description: '',
      client_id: null,
      start: null,
      end: null,
      duration: 1,
      calendar: 'unscheduledEvents',
    };
    setSelectedEvent(newEvent);
  };

  const handleSaveChanges = () => {
    if (!selectedEvent) return;
    const calendar = getCalendar(selectedEvent);
    const updated = calendar.map(evt =>
      evt.id === selectedEvent.id ? selectedEvent : evt
    );
    updateCalendar(selectedEvent, updated);
    saveChanges(selectedEvent);
    setSelectedEvent(null);
  };

  const deleteItem = (selectedEvent) => {
    setLoading(true);
    const calendar = getCalendar(selectedEvent)
    user.getIdToken().then(token => {
      fetch('/api/manager/delete', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ' + token
        },
        body: JSON.stringify([ "calendar", selectedEvent.id ])
      })
      .then(response => {
        if (!response.ok) {
          return response.json().then(err => {
            console.error('Error response:', err);
            setError(true);
            throw new Error(err.error);
          });
        }
        return response.json();
      })
      .catch(error => {console.error('Error fetching Data:', error); setError(true)})
      .finally(() => {setLoading(false)});
    });
    calendar.map((row, index) => {
      if(row[0] === selectedEvent[0]) {
        calendar.splice(index, 1);
      }
    });
  }

  if (loading) return <Spinner className="m-5" />;

  return (
    <div>
      <Helmet><title>Calendar</title></Helmet>
      <div>
        {/* ===== SIDEBAR ===== */}
        <div className={`calendar-toggle ${collapsed ? 'collapsed' : ''}`} onClick={toggleSidebar}>
          <HamburgerMenu collapsed={collapsed} />
        </div>
        <div className={`calendar-drop-bar ${collapsed ? 'collapsed' : ''}`}>
          <h2>Unscheduled <Button onClick={handleNewUnscheduledEvent}>+</Button></h2>
          {(!unscheduledEvents || unscheduledEvents.length === 0) ? <p className='noJob'>No Jobs to Schedule</p> :
            unscheduledEvents.map(item => (
              <DraggableEvent key={item.id} event={item} onDragStart={handleOnDragStart} onClick={handleDoubleClickEvent} />
            ))
          }
        </div>

        {/* ===== CALENDAR ===== */}
        <div className={`my-custom-calendar-container ${collapsed ? 'collapsed' : ''}`}>
          <div className='top-bar'>
            <div className={`top-bar-button ${collapsed ? 'collapsed' : ''}`} onClick={toggleSidebar}>
              <HamburgerMenu collapsed={collapsed} />
            </div>
            <Button className="calendar-button">View</Button>
            <Button className="calendar-button" onClick={handleNewUnscheduledEvent}>New Job</Button>
          </div>

          <DnDCalendar
            className="my-custom-calendar"
            localizer={localizer}
            events={events}
            eventPropGetter={eventStyleGetter}
            date={currentDate}
            view={currentView}
            onNavigate={date => setCurrentDate(date)}
            onView={view => setCurrentView(view)}
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
            onSelectEvent={handleDoubleClickEvent}
            startAccessor="start"
            endAccessor="end"
            components={{
              event: (props) => <CustomEvent {...props} onDoubleClick={handleDoubleClickEvent} />
            }}
          />
        </div>
      </div>

      {/* ===== MODAL ===== */}
      {selectedEvent && (
        <div className="modal-backdrop" onClick={handleCloseModal}>
          <div className="modal-content" onClick={e => e.stopPropagation()}>
            <h2>Edit Event</h2>
            <label>
              Title:
              <input
                type="text"
                value={selectedEvent.title}
                onChange={e => setSelectedEvent({ ...selectedEvent, title: e.target.value })}
              />
            </label>
            <label>
              Description:
              <textarea
                rows="3"
                value={selectedEvent.description}
                onChange={e => setSelectedEvent({ ...selectedEvent, description: e.target.value })}
              />
            </label>
            <label>
              Start:
              <input
                type="datetime-local"
                value={selectedEvent.start ? formatDateTimeLocal(selectedEvent.start) : ''}
                onChange={e => setSelectedEvent({ ...selectedEvent, start: e.target.value ? new Date(e.target.value) : null })}
              />
            </label>
            <label>
              Duration (hours):
              <input
                type="number"
                value={selectedEvent.duration || 1}
                onChange={e => {
                  const dur = Number(e.target.value) || 1;
                  setSelectedEvent({
                    ...selectedEvent,
                    duration: dur,
                    end: selectedEvent.start ? new Date(selectedEvent.start.getTime() + dur*60*60*1000) : null,
                  });
                }}
              />
            </label>
            <label>
              End:
              <input
                type="datetime-local"
                value={selectedEvent.end ? formatDateTimeLocal(selectedEvent.end) : ''}
                onChange={e => {
                  const endDate = e.target.value ? new Date(e.target.value) : null;
                  setSelectedEvent({
                    ...selectedEvent,
                    end: endDate,
                    duration: selectedEvent.start && endDate ? (endDate - selectedEvent.start)/(60*60*1000) : selectedEvent.duration
                  });
                }}
              />
            </label>
            <div className="modal-buttons">
              <Button className="delete-button" onClick={() => {deleteItem(selectedEvent); handleCloseModal();}}>Delete</Button>
              <Button className="cancel-button" onClick={handleCloseModal}>Cancel</Button>
              <Button className="calendar-button" onClick={handleSaveChanges}>Save</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}