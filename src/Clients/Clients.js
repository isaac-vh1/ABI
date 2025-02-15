import React, { useState, useEffect } from 'react';

function Client({ toggleSidebar }) {
  const [clients, setClients] = useState([]);
  const [filteredClients, setFilteredClients] = useState([]);
  const [searchFilter, setSearchFilter] = useState('');
  const [selectedClient, setSelectedClient] = useState(null); // Selected client for popup

    useEffect(() => {
      fetch('http://www.pi.acresbyisaac.com:5000/api/clients')
        .then(response => {
          if (!response.ok) {
            throw new Error('Network response was not ok ' + response.statusText);
          }
          return response.json();
        })
        .then(data => setClients(data))
        .catch(error => console.error('Error fetching users:', error));
        console.log(clients)
    }, []);

  // Handle filter changes
  const handleFilterChange = (e) => {
    const value = e.target.value.toLowerCase();
    setSearchFilter(value);

    const filtered = clients.filter((client) =>
      client.name.toLowerCase().includes(value) ||
      client.email.toLowerCase().includes(value) ||
      client.phone.includes(value) ||
      client.address.toLowerCase().includes(value)
    );
    setFilteredClients(filtered);
  };

  // Open client popup
  const handleRowClick = (client) => {
    setSelectedClient(client);
  };

  // Close client popup
  const closePopup = () => {
    setSelectedClient(null);
  };

  // Handle input change in the popup
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setSelectedClient((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  return (
    <div className="client-container">
        <head>
            <title>Clients</title>
        </head>
      <section>
        <button className="menu-toggle" onClick={toggleSidebar}>☰</button>
        <h1>Clients</h1>
      </section>
      <input
        type="text"
        placeholder="Search by name, email, phone, or address..."
        value={searchFilter}
        onChange={handleFilterChange}
        className="filter-input"
      />
      <ul>
        {clients.map(client => (
          <li key={client.id}>
            {client.name} ({client.age})
          </li>
        ))}
      </ul>

      <table className="client-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Email</th>
            <th>Phone</th>
          </tr>
        </thead>
        <tbody>
          {filteredClients.map((client) => (
            <tr key={client.id} onClick={() => handleRowClick(client)}>
              <td>{client.name}</td>
              <td>{client.email}</td>
              <td>{client.phone}</td>
            </tr>
          ))}
        </tbody>
      </table>

      {selectedClient && (
        <div className="client-popup">
          <div className="popup-content">
            <button className="close-popup" onClick={closePopup}>
              &times;
            </button>
            <h2>Client Details</h2>
            <form>
              <label>
                Name:
                <input
                  type="text"
                  name="name"
                  value={selectedClient.name}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Email:
                <input
                  type="email"
                  name="email"
                  value={selectedClient.email}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Phone:
                <input
                  type="text"
                  name="phone"
                  value={selectedClient.phone}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Street Address:
                <input
                  type="text"
                  name="streetAddress"
                  value={selectedClient.streetAddress || ''}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                City:
                <input
                  type="text"
                  name="city"
                  value={selectedClient.city || ''}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Zip Code:
                <input
                  type="text"
                  name="zipCode"
                  value={selectedClient.zipCode || ''}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Picture Preference:
                <input
                  type="text"
                  name="picturePreference"
                  value={selectedClient.picturePreference || ''}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Email Preference:
                <input
                  type="text"
                  name="emailPreference"
                  value={selectedClient.emailPreference || ''}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Total Dollars Spent:
                <input
                  type="number"
                  name="totalSpent"
                  value={selectedClient.totalSpent || 0}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Balance on Past Invoices:
                <input
                  type="number"
                  name="balance"
                  value={selectedClient.balance || 0}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Notifications:
                <textarea
                  name="notifications"
                  value={selectedClient.notifications || ''}
                  onChange={handleInputChange}
                />
              </label>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

export default Client;