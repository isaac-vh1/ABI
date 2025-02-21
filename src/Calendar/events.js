// src/data/events.js

// Scheduled Events
export const scheduledEvents = [
  {
      id: 1,
      title: 'Existing Event',
      start: new Date(),
      end: new Date(new Date().setHours(new Date().getHours() + 1)),
      calendar: 'scheduledEvents',
      description: 'This is a pre-scheduled event.', // Optional: Add description if needed
    },
    // Add more scheduled events as needed
  ];
  // Unscheduled Events (e.g., Events available for scheduling)
  export const unscheduledEvents = [
    {
      id: 101,
      title: 'Cleaning Service',
      duration: 1,
      calendar: 'unscheduledEvents',
      description: 'Cleaning service for residential properties.',
    },
    {
      id: 102,
      title: 'Maintenance',
      duration: 2,
      calendar: 'unscheduledEvents',
      description: 'General maintenance services.',
    },
    {
      id: 103,
      title: 'Consultation',
      duration: 3,
      calendar: 'unscheduledEvents',
      description: 'Professional consultation session.',
    },
    // Add more unscheduled events as needed
  ];
  
  // Worker Schedule Events
  export const workerSchedule = [
    {
      id: 201,
      workerId: 1,
      title: 'Existing Worker Event',
      start: new Date(),
      end: new Date(new Date().setHours(new Date().getHours() + 1)),
      calendar: 'workerSchedule',
      description: 'Scheduled work session for worker 1.',
    },
    // Add more worker schedule events as needed
  ];