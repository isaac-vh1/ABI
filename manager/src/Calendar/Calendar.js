import React, { useCallback, useState, useMemo, useRef, useEffect} from 'react';
import { Calendar, Views, momentLocalizer } from 'react-big-calendar';
import moment, { duration } from 'moment';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import 'react-big-calendar/lib/addons/dragAndDrop/styles.css';
import './Calendar.css';
import withDragAndDrop from 'react-big-calendar/lib/addons/dragAndDrop';
import CustomEvent from './CustomEvent';
import DraggableEvent from './DraggableEvent';
import HamburgerMenu from '../Components/HamburgerMenu';
import { auth } from '../firebase';
import { Helmet } from 'react-helmet';
import { Button, Dropdown } from 'react-bootstrap';

const localizer = momentLocalizer(moment);
const DnDCalendar = withDragAndDrop(Calendar);

export default function CalendarContainer({ toggleSidebar, collapsed }) {
  const [scheduledEvents, setScheduledEvents] = useState([]);
  const [unscheduledEvents, setUnscheduledEvents] = useState( []);
  const [workerSchedule, setWorkerSchedule] = useState([]);
  const [clients, setClients] = useState([]);
  const [error, setError] = useState(false);
  const [loading, setLoading] = useState(true);
  const user = auth.currentUser;
  const eventIdRef = useRef(1000); // Use a ref to keep track of event IDs
  const [currentDate, setCurrentDate] = useState(new Date());
  const [currentView, setCurrentView] = useState(Views.MONTH);
  const [selectedEvent, setSelectedEvent] = useState(null);

  useEffect(() => {
      if (!user) return;
      user.getIdToken().then((token) => {
        fetch('/api/manager/events', {
          headers: { Authorization: `Bearer ${token}` },
        }).then((r) => r.json())
          .then((data) => {
            setScheduledEvents(data[0] ?? []);
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

function formatEvents(rows = []) {
  return rows.map(
    ({
      id,
      client_id: client_id,      // snake → camel
      title,
      description,
      calendar,
      start,
      end,
      duration_h,               // only used if end is NULL
    }) => {
      const startDate = new Date(start);          // DATETIME → JS Date
      const endDate =
        end != null
          ? new Date(end)
          : new Date(startDate.getTime() + (duration_h ?? 1) * 60 * 60 * 1000); // fallback

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
  }

  const saveChanges = (selectedItem) => {
    if (!user) return;
    user.getIdToken().then(async (token) => {
      try {
        const res = await fetch('/api/manager/update/calendar', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({selectedItem}),
        }).then((r) => r.json())
          .then((data) => {
            if (data != "true"){
              setError(true);
              throw new Error('Update failed');
            }
          });
        if (!res.ok) throw new Error('Update failed');
        return true;
      } catch (e) {
        console.error(e);
        setError(true);
      }
    });
  };
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

  const handleDoubleClickEvent = (event) => {
    setSelectedEvent(event);

  };
  const formatDateTimeLocal = (date) => {
    const pad = (n) => (n < 10 ? '0' + n : n);
    const year = date.getFullYear();
    const month = pad(date.getMonth() + 1);
    const day = pad(date);
    const hours = pad(date.getHours());
    const minutes = pad(date.getMinutes());
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  };

  const handleCloseModal = () => {
    setSelectedEvent(null);
  };

  const handleSaveChanges = () => {
    if (selectedEvent && saveChanges(selectedEvent)) {
      var calendar = getCalendar(selectedEvent);
      const updated = calendar.map((evt) =>
        evt.id === selectedEvent.id ? selectedEvent : evt
      );
      updateCalendar(selectedEvent, updated);
    }
  };

  const [draggedEvent, setDraggedEvent] = useState(null);

  const handleDropFromOutside = useCallback(
    ({ start, end, allDay: isAllDay }) => {
      if (!draggedEvent) return;
      const startDate = new Date(start);
      const duration = draggedEvent.duration || 1;
      const endDate = new Date(startDate);
      endDate.setHours(endDate.getHours() + duration);

      const newEvent = {
        id: draggedEvent.id,
        title: draggedEvent.title,
        description: draggedEvent.description,
        client_id: draggedEvent.client_id,
        duration: duration,
        start: startDate.toUTCString(),
        end: endDate.toUTCString(),
        calendar: 'scheduledEvents',
      };

      saveChanges(newEvent);
      const isDuplicate = scheduledEvents.some(
        (evt) =>
          evt.title === newEvent.title &&
          evt.start === newEvent.start
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
      var calendar = getCalendar(event);
      const updated = calendar.map((evt) =>
        evt.id === event.id ? { ...evt, start: start.toUTCString(), end: end.toUTCString() } : evt
      );
      updateCalendar(event, updated);
      saveChanges({ ...event, start: start.toUTCString(), end: end.toUTCString() });
  }

  const handleOnDragStart = useCallback((item) => {
    const updatedEvent = {
      ...item,
      start: new Date(),
      end: new Date(Date.now() + item.duration * 3600_000)
    };
    console.log(updatedEvent);
    setDraggedEvent(item);
    console.log('Dragging event:', item);
  }, []);

  const handleNewUnscheduledEvent = () => {
    const newEventId = eventIdRef.current++;
    const newEvent = {
      id: newEventId,
      title: '',
      description: '',
      client_id: null,
      start: null,
      end: null,
      duration: 1,
      calendar: 'unscheduledEvents',
    };
    setSelectedEvent(newEvent);
  }
  const getClientName = (clientNum) => {
    const clientRow = clients.find(
      (c) => String(c[0]) === String(clientNum)
    );
    const clientName = clientRow
      ? `${clientRow[1] || ''} ${clientRow[2] || ''}`.trim()
      : '(no match)';
  }

  return (
    <div>
      <Helmet><title>Calendar</title></Helmet>
      <div>
        {/* ===== SIDEBAR ===== */}
        <div className={`calendar-toggle ${collapsed ? 'collapsed' : ''}`} onClick={toggleSidebar}><HamburgerMenu collapsed={collapsed} /></div>
        <div className={`calendar-drop-bar ${collapsed ? 'collapsed' : ''}`}>
          <h2>Unscheduled<Button onClick={ handleNewUnscheduledEvent }>+</Button></h2>
          {(!unscheduledEvents || unscheduledEvents.length === 0 )? <p className='noJob'>No Jobs to Schedule</p> :
          unscheduledEvents.map((item) => (
            <DraggableEvent key={item.id} event={item} onDragStart={handleOnDragStart} onClick={handleDoubleClickEvent}/>
          ))}
        </div>

        {/* ===== CALENDAR ===== */}
        <div className={`my-custom-calendar-container ${collapsed ? 'collapsed' : ''}`}>
          <article>
            <div className='top-bar'>
              <div className={`top-bar-button ${collapsed ? 'collapsed' : ''}`} onClick={toggleSidebar}><HamburgerMenu collapsed={collapsed} /></div>
              <Button className="calendar-button">View</Button>
              <Button className="calendar-button" onClick={handleDoubleClickEvent}>New Job</Button>
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
                value={selectedEvent.title}
                onChange={(e) => setSelectedEvent({
                  ...selectedEvent,
                  title: e.target.value,
                })}
              />
            </label>
            <label>
              Description:
              <textarea
                rows="3"
                value={selectedEvent.description}
                onChange={(e) => setSelectedEvent({
                  ...selectedEvent,
                  description: e.target.value,
                })}
              />
            </label>
            <label>
              Client:
              <input className="form-control" list="datalistOptions" id="exampleDataList" placeholder="Type to search..."
                onChange={(e) => {
                  const inputValue = e.target.value;
                  const foundIndex = clients.findIndex((client) => {
                    const fullName = `${client[1] || ""} ${client[2] || ""}`.trim();
                    return fullName === inputValue;
                  });
                  if (foundIndex !== -1) {
                    setSelectedEvent({
                      ...selectedEvent,
                      client_id: clients[foundIndex][0],
                    });
                  }
                }}
              />
              <datalist id="datalistOptions">
                {clients.map((client, index) => {
                  const value = client ? `${client[1] || ""} ${client[2] || ""}`.trim() : "";
                  return <option key={index} value={value} />
                })}
              </datalist>
            </label>
            <label>
              Start:
              <input
                type="datetime-local"
                value={selectedEvent.start ? formatDateTimeLocal(selectedEvent.start) : ''}
                onChange={(e) => setSelectedEvent({
                  ...selectedEvent,
                  start: e.target.value ? new Date(e.target.value).toUTCString() : null,
                })}
              />
            </label>
            <label>
              Duration:
              <input
                type="number"
                value={selectedEvent.duration}
                onChange={(e) => setSelectedEvent({
                  ...selectedEvent,
                  duration: e.target.value,
                  end: e.target.value ? new Date(new Date(selectedEvent.start).getTime() + e.target.value * 60 * 60 * 1000).toUTCString() : null,
                })}
              />
            </label>
            <label>
              End:
              <input
                type="datetime-local"
                value={selectedEvent.end ? formatDateTimeLocal(selectedEvent.end) : ''}
                onChange={(e) => setSelectedEvent({
                  ...selectedEvent,
                  end: e.target.value ? new Date(e.target.value).toUTCString() : null,
                  duration: e.target.value ? (new Date(e.target.value) - new Date(selectedEvent.start)) / (60 * 60 * 1000) : 1,
                })}
              />
            </label>

            <div className="modal-buttons">
              <Button className="cancel-button" onClick={handleCloseModal}>Cancel</Button>
              <Button className="calendar-button" onClick={handleSaveChanges}>Save</Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}