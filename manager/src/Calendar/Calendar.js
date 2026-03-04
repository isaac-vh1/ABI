import React, { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Calendar, Views, momentLocalizer } from "react-big-calendar";
import withDragAndDrop from "react-big-calendar/lib/addons/dragAndDrop";
import moment from "moment";
import { Helmet } from "react-helmet";
import { Button, Spinner } from "react-bootstrap";

import "react-big-calendar/lib/css/react-big-calendar.css";
import "react-big-calendar/lib/addons/dragAndDrop/styles.css";
import "./Calendar.css";

import { auth } from "../firebase";
import HamburgerMenu from "../Components/HamburgerMenu";
import CustomEvent from "./CustomEvent";
import DraggableEvent from "./DraggableEvent";

const localizer = momentLocalizer(moment);
const DnDCalendar = withDragAndDrop(Calendar);

const CALENDAR_TABLE = "calendar";

function normalizeEvent(row) {
  const start = row.start ? new Date(row.start) : null;
  const end = row.end ? new Date(row.end) : null;
  const durationHours = start && end ? (end - start) / (60 * 60 * 1000) : 1;
  return {
    id: row.id,
    client_id: row.client_id ?? row.clientId ?? null,
    title: row.title ?? "",
    description: row.description ?? "",
    calendar: row.calendar,
    start,
    end,
    duration: Number.isFinite(durationHours) && durationHours > 0 ? durationHours : 1,
  };
}

function formatDateTimeLocal(date) {
  if (!date) return "";
  const pad = (n) => (n < 10 ? `0${n}` : `${n}`);
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(
    date.getHours()
  )}:${pad(date.getMinutes())}`;
}

export default function CalendarContainer({ toggleSidebar, collapsed }) {
  const [scheduledEvents, setScheduledEvents] = useState([]);
  const [unscheduledEvents, setUnscheduledEvents] = useState([]);
  const [workerSchedule, setWorkerSchedule] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [currentDate, setCurrentDate] = useState(new Date());
  const [currentView, setCurrentView] = useState(Views.MONTH);
  const [selectedEvent, setSelectedEvent] = useState(null);
  const [draggedEvent, setDraggedEvent] = useState(null);
  const eventIdRef = useRef(1000);

  const user = auth.currentUser;

  const getToken = async () => {
    if (!user) {
      throw new Error("No authenticated user found.");
    }
    return user.getIdToken();
  };

  const authorizedFetch = useCallback(
    async (url, options = {}) => {
      const token = await getToken();
      const merged = {
        ...options,
        headers: {
          ...(options.headers || {}),
          Authorization: `Bearer ${token}`,
        },
      };
      return fetch(url, merged);
    },
    [user]
  );

  const loadCalendarData = useCallback(async () => {
    if (!user) return;
    setLoading(true);
    setError("");
    try {
      const response = await authorizedFetch("/api/manager/events");
      if (!response.ok) {
        throw new Error("Failed to load calendar events.");
      }
      const data = await response.json();
      setScheduledEvents((data[0] || []).map(normalizeEvent));
      setUnscheduledEvents((data[1] || []).map(normalizeEvent));
      setWorkerSchedule((data[2] || []).map(normalizeEvent));
      eventIdRef.current = data[4] || 1000;
    } catch (e) {
      console.error(e);
      setError("Failed to load calendar data.");
    } finally {
      setLoading(false);
    }
  }, [authorizedFetch, user]);

  useEffect(() => {
    loadCalendarData();
  }, [loadCalendarData]);

  const allScheduledLikeEvents = useMemo(
    () => [...scheduledEvents, ...workerSchedule],
    [scheduledEvents, workerSchedule]
  );

  const getCalendarSetter = (calendarName) => {
    if (calendarName === "scheduledEvents") return setScheduledEvents;
    if (calendarName === "unscheduledEvents") return setUnscheduledEvents;
    return setWorkerSchedule;
  };

  const persistEvent = useCallback(
    async (eventToSave) => {
      const payload = {
        id: eventToSave.id,
        calendar: eventToSave.calendar,
        title: eventToSave.title,
        description: eventToSave.description ?? "",
        client_id: eventToSave.client_id ?? null,
        start: eventToSave.start ? new Date(eventToSave.start).toISOString() : null,
        end: eventToSave.end ? new Date(eventToSave.end).toISOString() : null,
      };

      const response = await authorizedFetch("/api/manager/calendar/save", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });
      if (!response.ok) {
        throw new Error("Failed to save event.");
      }
      const value = await response.json();
      if (value !== "true") {
        throw new Error("Backend rejected event save.");
      }
    },
    [authorizedFetch]
  );

  const removeEvent = async (eventToDelete) => {
    setError("");
    try {
      const response = await authorizedFetch("/api/manager/delete", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify([CALENDAR_TABLE, eventToDelete.id]),
      });
      if (!response.ok) {
        throw new Error("Failed to delete event.");
      }
      const setter = getCalendarSetter(eventToDelete.calendar);
      setter((prev) => prev.filter((e) => e.id !== eventToDelete.id));
      setSelectedEvent(null);
    } catch (e) {
      console.error(e);
      setError("Failed to delete event.");
    }
  };

  const eventStyleGetter = (event) => {
    let backgroundColor = "#6c757d";
    if (event.calendar === "unscheduledEvents") backgroundColor = "#ffffff";
    if (event.calendar === "scheduledEvents") backgroundColor = "#28a745";
    if (event.calendar === "workerSchedule") backgroundColor = "#2f4fdb";
    return {
      style: {
        backgroundColor,
        color: event.calendar === "unscheduledEvents" ? "#000" : "#fff",
      },
    };
  };

  const onEventChange = async ({ event, start, end }) => {
    const updated = { ...event, start: new Date(start), end: new Date(end) };
    const setter = getCalendarSetter(event.calendar);
    setter((prev) => prev.map((evt) => (evt.id === event.id ? updated : evt)));
    try {
      await persistEvent(updated);
    } catch (e) {
      console.error(e);
      setError("Failed to update event.");
      loadCalendarData();
    }
  };

  const handleDropFromOutside = useCallback(
    async ({ start }) => {
      if (!draggedEvent) return;

      const startDate = new Date(start);
      const duration = draggedEvent.duration ?? 1;
      const endDate = new Date(startDate.getTime() + duration * 60 * 60 * 1000);
      const moved = {
        ...draggedEvent,
        calendar: "scheduledEvents",
        start: startDate,
        end: endDate,
      };

      setUnscheduledEvents((prev) => prev.filter((evt) => evt.id !== draggedEvent.id));
      setScheduledEvents((prev) => [...prev.filter((evt) => evt.id !== moved.id), moved]);
      setDraggedEvent(null);

      try {
        await persistEvent(moved);
      } catch (e) {
        console.error(e);
        setError("Failed to move event.");
        loadCalendarData();
      }
    },
    [draggedEvent, loadCalendarData, persistEvent]
  );

  const handleNewUnscheduledEvent = () => {
    setSelectedEvent({
      id: eventIdRef.current++,
      title: "",
      description: "",
      client_id: null,
      start: null,
      end: null,
      duration: 1,
      calendar: "unscheduledEvents",
    });
  };

  const handleSaveChanges = async () => {
    if (!selectedEvent) return;
    const setter = getCalendarSetter(selectedEvent.calendar);
    const nextEvent = { ...selectedEvent };
    if (nextEvent.start && !nextEvent.end) {
      nextEvent.end = new Date(nextEvent.start.getTime() + (nextEvent.duration || 1) * 60 * 60 * 1000);
    }

    setter((prev) => {
      const exists = prev.some((evt) => evt.id === nextEvent.id);
      if (exists) {
        return prev.map((evt) => (evt.id === nextEvent.id ? nextEvent : evt));
      }
      return [...prev, nextEvent];
    });

    try {
      await persistEvent(nextEvent);
      setSelectedEvent(null);
    } catch (e) {
      console.error(e);
      setError("Failed to save event.");
      loadCalendarData();
    }
  };

  if (loading) return <Spinner className="m-5" />;

  return (
    <div>
      <Helmet>
        <title>Calendar</title>
      </Helmet>
      <div>
        <div className={`calendar-toggle ${collapsed ? "collapsed" : ""}`} onClick={toggleSidebar}>
          <HamburgerMenu collapsed={collapsed} />
        </div>
        <div className={`calendar-drop-bar ${collapsed ? "collapsed" : ""}`}>
          <h2>
            Unscheduled <Button onClick={handleNewUnscheduledEvent}>+</Button>
          </h2>
          {!unscheduledEvents.length ? (
            <p className="noJob">No Jobs to Schedule</p>
          ) : (
            unscheduledEvents.map((item) => (
              <DraggableEvent
                key={item.id}
                event={item}
                onDragStart={(event) => setDraggedEvent(event)}
                onClick={() => setSelectedEvent(item)}
              />
            ))
          )}
          {error ? <p className="noJob">{error}</p> : null}
        </div>

        <div className={`my-custom-calendar-container ${collapsed ? "collapsed" : ""}`}>
          <div className="top-bar">
            <div className={`top-bar-button ${collapsed ? "collapsed" : ""}`} onClick={toggleSidebar}>
              <HamburgerMenu collapsed={collapsed} />
            </div>
            <Button className="calendar-button" onClick={() => setCurrentView(Views.MONTH)}>
              Month
            </Button>
            <Button className="calendar-button" onClick={handleNewUnscheduledEvent}>
              New Job
            </Button>
          </div>

          <DnDCalendar
            className="my-custom-calendar"
            localizer={localizer}
            events={allScheduledLikeEvents}
            eventPropGetter={eventStyleGetter}
            date={currentDate}
            view={currentView}
            onNavigate={setCurrentDate}
            onView={setCurrentView}
            defaultView={Views.MONTH}
            views={[Views.MONTH, Views.WEEK, Views.DAY]}
            step={15}
            timeslots={4}
            selectable
            resizable
            dragFromOutsideItem={() => draggedEvent}
            onDropFromOutside={handleDropFromOutside}
            onSelectSlot={({ start }) => {
              setCurrentDate(start);
              if (currentView !== Views.DAY) setCurrentView(Views.WEEK);
            }}
            onEventDrop={onEventChange}
            onEventResize={onEventChange}
            onSelectEvent={setSelectedEvent}
            startAccessor="start"
            endAccessor="end"
            components={{
              event: (props) => <CustomEvent {...props} onDoubleClick={setSelectedEvent} />,
            }}
          />
        </div>
      </div>

      {selectedEvent && (
        <div className="modal-backdrop" onClick={() => setSelectedEvent(null)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>Edit Event</h2>
            <label>
              Title:
              <input
                type="text"
                value={selectedEvent.title}
                onChange={(e) => setSelectedEvent({ ...selectedEvent, title: e.target.value })}
              />
            </label>
            <label>
              Description:
              <textarea
                rows="3"
                value={selectedEvent.description}
                onChange={(e) => setSelectedEvent({ ...selectedEvent, description: e.target.value })}
              />
            </label>
            <label>
              Start:
              <input
                type="datetime-local"
                value={selectedEvent.start ? formatDateTimeLocal(selectedEvent.start) : ""}
                onChange={(e) =>
                  setSelectedEvent({
                    ...selectedEvent,
                    start: e.target.value ? new Date(e.target.value) : null,
                  })
                }
              />
            </label>
            <label>
              Duration (hours):
              <input
                type="number"
                min="0.25"
                step="0.25"
                value={selectedEvent.duration || 1}
                onChange={(e) => {
                  const duration = Number(e.target.value) || 1;
                  setSelectedEvent((prev) => ({
                    ...prev,
                    duration,
                    end: prev.start ? new Date(prev.start.getTime() + duration * 60 * 60 * 1000) : prev.end,
                  }));
                }}
              />
            </label>
            <label>
              End:
              <input
                type="datetime-local"
                value={selectedEvent.end ? formatDateTimeLocal(selectedEvent.end) : ""}
                onChange={(e) => {
                  const end = e.target.value ? new Date(e.target.value) : null;
                  setSelectedEvent((prev) => ({
                    ...prev,
                    end,
                    duration: prev.start && end ? (end - prev.start) / (60 * 60 * 1000) : prev.duration,
                  }));
                }}
              />
            </label>
            <div className="modal-buttons">
              <Button className="delete-button" onClick={() => removeEvent(selectedEvent)}>
                Delete
              </Button>
              <Button className="cancel-button" onClick={() => setSelectedEvent(null)}>
                Cancel
              </Button>
              <Button className="calendar-button" onClick={handleSaveChanges}>
                Save
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
