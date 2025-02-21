import React, { useState, useEffect } from 'react';
import HeaderBar from '../Components/HeaderBar';
import "./Table.css"

function Table({ page, toggleSidebar, collapsed }) {
  const [data, setData] = useState([]);
  const [dataHeader] = useState([]);
  const [filteredData, setFilteredData] = useState([]);
  const [searchFilter, setSearchFilter] = useState('');
  const [selectedItem, setSelectedItem] = useState(null);
  const [error, setError] = useState(false);
  useEffect(() => {
    fetch('https://www.pi.acresbyisaac.com/api/' + page)
      .then(response => {
        if (!response.ok) {
          setError(true);
          throw new Error('Network response was not ok ' + response.statusText);
        }
        return response.json();
      })
      .then(dataAPI => {
        setData(dataAPI);
        setFilteredData(dataAPI)
        console.log()
      })
      .catch(error => console.error('Error fetching Data:', error));
  }, [page]);
  const handleFilterChange = (e) => {
    const value = e.target.value.toLowerCase();
    setSearchFilter(value);

    const filtered = data.filter((Item) =>
      Item[1].toLowerCase().includes(value) ||
      Item[2].toLowerCase().includes(value) ||
      Item[3].includes(value) ||
      Item[4].toLowerCase().includes(value)
    );
    setFilteredData(filtered);
  };
  const handleRowClick = (item) => {
    setSelectedItem(item);
  };
  const closePopup = () => {
    setSelectedItem(null);
  };
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setSelectedItem((prev) => ({
      ...prev,
      [name]: value,
    }));
  };
  const capitalize = (str) => {
    if (!str) return '';
    return str.charAt(0).toUpperCase() + str.slice(1);
  }
  return (
    <div className="table-container">
      <HeaderBar page={capitalize(page)} toggleSidebar={toggleSidebar} collapsed={collapsed} />
      <input
        type="text"
        placeholder="Search by name, email, phone, or address..."
        className="filter-input"
        value={searchFilter}
        onChange={handleFilterChange}
      />
      <table className="table">
        <thead>
          <tr>
            {dataHeader.map((head, index) => (
              <th key={index}>{head}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {filteredData.map((row, rowIndex) => (
            <tr key={row[0] || rowIndex} onClick={() => handleRowClick(row)}>
              {row.map((cell, cellIndex) => (
                <td key={cellIndex}>{cell}</td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
      <h3 className={`error ${error ? 'show': ''}`}>Error Loading Data</h3>

      {selectedItem && (
        <div className="table-popup">
          <div className="popup-content">
            <button className="close-popup" onClick={closePopup}>
              &times;
            </button>
            <h2>{capitalize(page)} Details</h2>
            <form>
              {dataHeader.map((head) => (
                <label>
                head
                <input
                  type="text"
                  name="name"
                  value={selectedItem.name}
                  onChange={handleInputChange}
                />
              </label>
              ))}
              <label>
                Email:
                <input
                  type="email"
                  name="email"
                  value={selectedItem.email}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Phone:
                <input
                  type="text"
                  name="phone"
                  value={selectedItem.phone}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Street Address:
                <input
                  type="text"
                  name="streetAddress"
                  value={selectedItem.streetAddress || ''}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                City:
                <input
                  type="text"
                  name="city"
                  value={selectedItem.city || ''}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Zip Code:
                <input
                  type="text"
                  name="zipCode"
                  value={selectedItem.zipCode || ''}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Picture Preference:
                <input
                  type="text"
                  name="picturePreference"
                  value={selectedItem.picturePreference || ''}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Email Preference:
                <input
                  type="text"
                  name="emailPreference"
                  value={selectedItem.emailPreference || ''}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Total Dollars Spent:
                <input
                  type="number"
                  name="totalSpent"
                  value={selectedItem.totalSpent || 0}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Balance on Past Invoices:
                <input
                  type="number"
                  name="balance"
                  value={selectedItem.balance || 0}
                  onChange={handleInputChange}
                />
              </label>
              <label>
                Notifications:
                <textarea
                  name="notifications"
                  value={selectedItem.notifications || ''}
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

export default Table;