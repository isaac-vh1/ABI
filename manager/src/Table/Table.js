import React, { useState, useEffect } from 'react';
import HamburgerMenu from '../Components/HamburgerMenu';
import "./Table.css"
import { Helmet } from "react-helmet";
import { auth } from '../firebase';

function Table({ page, toggleSidebar, collapsed }) {
  const [update, setUpdate] = useState(false);
  const [data, setData] = useState([]);
  const [dataHeader, setDataHeader] = useState([]);
  const [filteredData, setFilteredData] = useState([]);
  const [searchFilter, setSearchFilter] = useState('');
  const [selectedItem, setSelectedItem] = useState(null);
  const [error, setError] = useState(false);
  const user = auth.currentUser;

  useEffect(() => {
    setData(null);
    setFilteredData(null);
    user.getIdToken().then(token => {
      fetch('https://www.pi.acresbyisaac.com/api/manager/' + page, {
        method: 'GET',
        headers: {
          'Authorization': 'Bearer ' + token
        }
      })
      .then(response => {
        if (!response.ok) {
          return response.json().then(err => {
            console.error('Error response:', err);
            throw new Error(err.error);
          });
        }
        return response.json();
      })
      .then(dataAPI => {
        dataHandler(dataAPI)
      })
      .catch(error => console.error('Error fetching Data:', error))
      .finally(() => {setUpdate(false)});
    });
  }, [page, update]);
  const dataHandler = (dataAPI) => {
    var head = dataAPI.shift()
    setDataHeader(head)
    setData(dataAPI)
    setFilteredData(dataAPI)
  }
  const handleFilterChange = (e) => {
    const value = e.target.value.toLowerCase();
    setSearchFilter(value);
    const filtered = data.filter((item) => {
      const field1 = item[1] ? item[1].toLowerCase() : '';
      const field2 = item[2] ? item[2].toLowerCase() : '';
      const field3 = item[3] ? item[3].toString().toLowerCase() : '';
      const field4 = item[4] ? item[4].toLowerCase() : '';
      const searchTerm = value.toLowerCase();
      return (
        field1.includes(searchTerm) ||
        field2.includes(searchTerm) ||
        field3.includes(searchTerm) ||
        field4.includes(searchTerm)
      );
    });
    setFilteredData(filtered);
  };
  const handleRowClick = (item) => {
    setSelectedItem(item)
  };
  const closePopup = () => {

    setSelectedItem(null);
  };
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setSelectedItem(prev => ({
      ...prev,
      [name]: value,
    }));
  }
  function capitalize(str) {
    return str
      .split(' ')
      .map(word => {
        if (word.length === 0) return '';
        return word.charAt(0).toUpperCase() + word.slice(1);
      })
      .join(' ');
  }
  const newItem = () => {
    const item = {};
    dataHeader.forEach(head => {
      item[head] = '';
    });
    setSelectedItem(item);
  }
  const saveChanges = () => {
    try {
      user.getIdToken().then(token => {
        fetch('https://www.pi.acresbyisaac.com/api/manager/update/' + page, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
          },
          body: JSON.stringify({selectedItem}),
        }).then(response => {
          const result = response.json();
          console.log(result)
        })
        setUpdate(true)
      });
    } catch (err) {
      setError(true);
    }
    closePopup();
  }
  return (
    <div className="table-container">
      <Helmet>
        <title>{capitalize(page)}</title>
      </Helmet>
         <header className="Bar">
           <div className="menu-toggle" onClick={toggleSidebar}>
             <HamburgerMenu collapsed={collapsed} />
           </div>
           <h1>{capitalize(page)}</h1>
           <button onClick={newItem}>New {page}</button>
         </header>
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
            {dataHeader.slice(1).map((head, index) => (
              <th key={index}>{capitalize(head[0].replace('_', ' '))}</th>
            ))}
          </tr>
        </thead>
        <tbody>
          {filteredData.map((row, rowIndex) => (
            <tr key={row[0] || rowIndex} onClick={() => handleRowClick(row)}>
              {row.slice(1).map((cell, cellIndex) => (
                <td key={cellIndex}>{cell}</td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
      <h3 className={`error ${error ? 'show': ' '}`}>Error Loading Data</h3>

      {selectedItem && (
        <div className="table-popup" onClick={closePopup}>
          <div className="popup-content" onClick={(e) => e.stopPropagation()}>
            <h2>{capitalize(page)} Details</h2>
            {dataHeader.slice(1).map((head, index) => (
              <label key={index}>
                {capitalize(head[0].replace('_', ' '))}:
                <input
                  type="text"
                  name={head[0]}
                  value={selectedItem[index + 1]}
                  onChange={(e) =>
                    setSelectedItem(prev => ({
                      ...prev,
                      [index + 1]: e.target.value,
                    }))
                  }
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
                Total Dollars Spent:
                <input
                  type="number"
                  name="totalSpent"
                  value={selectedItem.totalSpent || 0}
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
              <div className="popup-buttons">
                <button className="cancel-button" onClick={closePopup}>Cancel</button>
                <button className="button" onClick={saveChanges}>Save</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default Table;