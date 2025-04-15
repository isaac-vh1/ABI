useEffect(() => {
  fetch('/api/events')
    .then((response) => response.json())
    .then((data) => {
      setScheduledEvents(data.scheduledEvents);
      setUnscheduledEvents(data.unscheduledEvents);
      setWorkerSchedule(data.workerSchedule);
    })
    .catch((error) => {
      console.error('Error fetching events:', error);
    });
}, []);