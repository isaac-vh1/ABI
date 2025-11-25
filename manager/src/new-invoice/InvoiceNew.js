import React, { useState, useEffect } from 'react';
import './InvoiceNew.css';
import { useNavigate } from 'react-router-dom';
import { auth } from '../firebase';
import DatePicker from 'react-datepicker';
import { Helmet } from 'react-helmet';
import { DateTime } from 'luxon';
import HamburgerMenu from '../Components/HamburgerMenu';
import { Spinner }from 'react-bootstrap';

const InvoiceNew = ({collapsed, toggleSidebar}) => {
  const today   = DateTime.local();          // 2025-05-18T22:05:…
  const dueDate = today.plus({ days: 30 });
  const [invoiceData, setInvoiceData] = useState([
    '','80','','',                         // client ID, invoiceNumber, location code
    today.toUTC().toHTTP(),      // Completion Date (today's date)
    dueDate.toUTC().toHTTP(), // Due Date (30 days from today)
    '','0.00',                       // Subtotal (default as 0)
    '0.00',                       // Sales Tax Amount (default as 0)
    '0.00',                       // Total (default as 0)
    '0.00',                       // Tips (default as 0)
    'pending', //status
    '0'  //Location Code - DOR reference
  ]);
  const[invoiceItems, setInvoiceItems] = useState([['',"0.00"]]);
  const[clientData, setClientData] = useState([]);
  const[locationData, setLocationData] = useState([]);
  const[tempData, setTempData] = useState(['10.0', 0, 0, 0]); //Sales tax, client index, location index, locationCode-DOR number tracking
  const [loading, setLoading] = useState(true);
  const user = auth.currentUser;
  const navigate = useNavigate();

  useEffect(() => {
    if (!user) return;
    user.getIdToken().then(token => {
      fetch('/api/manager/new-invoice', {
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
        setInvoiceData((prev) => {
          const newData = [...prev];
          newData[2] = dataAPI[0];
          return newData;
        });
        setClientData(dataAPI[1])
        setLocationData(dataAPI[2])
      })
      .catch(error => console.error('Error fetching Data:', error))
      .finally(() => setLoading(false));
    });
  },[user]);
  useEffect(() => {
    const total = invoiceItems.reduce((sum, item) => {
      return sum + (parseFloat(item[1]) || 0);
    }, 0);
    setInvoiceData((prev) => {
      const newData = [...prev];
      newData[7] = Math.round(total * 100) / 100;
      newData[8] = Math.round(((tempData[0] / 100) * total)*100)/100;
      newData[9] = newData[7] + newData[8];
      return newData;
    });
  }, [invoiceItems, tempData[0]]);
  useEffect(()=>{
    if (!user) return;
    user.getIdToken().then(token => {
      fetch('/api/manager/sales-tax-lookup', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ' + token,
          body: JSON.stringify([locationData[tempData[2]][1], locationData[tempData[2]][2],locationData[tempData[2]][3]])
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
        setTempData([dataAPI[0], tempData[1], tempData[2]]);
        const newData = [...invoiceData];
        newData[12] = dataAPI[1];
        setInvoiceData(newData);

      })
      .catch(error => console.error('Error fetching Data:', error))
      .finally(() => setLoading(false))
    });
  }, [invoiceData[0]]);

  const adjustForTimezone = (date) => {
    const offset = date.getTimezoneOffset();
    return new Date(date.getTime() + offset * 60000);
  };

  const saveInvoice = () => {
    try {
        user.getIdToken().then(token => {
        fetch('/api/manager/update/new-invoice', {
          method: 'POST',
          headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ' + token
          },
          body: JSON.stringify([invoiceData, invoiceItems]),
        }).then(response => response.json()
        ).then(result => {
          console.log("Response from server:", result);
          if(result == "true") {
            navigate('/invoice-dashboard');
          }
        }).catch(err => console.error('Fetch chain failed:', err))});
    } catch (err) {
        console.error("Error: " + err)
    }
  }
  const addItem = () => {
    const newItem = ['', '0.00'];
    setInvoiceItems((prev) => [...prev, newItem]);
  };

  if (loading) return <Spinner className="m-5" />;

  return (
    <div className='invoice-scope'>
      <Helmet><title>New Invoice</title></Helmet>
      <div className='top-bar'>
        <div className={`top-bar-button ${collapsed ? 'collapsed' : ''}`} onClick={toggleSidebar}><HamburgerMenu collapsed={collapsed} /></div>
      </div>
      <input className="form-control" list="datalistOptions" id="exampleDataList" placeholder="Type to search..."
        onChange={(e) => {
          const inputValue = e.target.value;
          const foundIndex = clientData.findIndex((client) => {
            const fullName = `${client[1] || ""} ${client[2] || ""}`.trim();
            return fullName === inputValue;
          });
          if (foundIndex !== -1) {
            setTempData((prev) => {
              const newData = [...prev];
              newData[1] = foundIndex;
              newData[2] = locationData.findIndex((item) => item[0] === clientData[foundIndex][9]);
              return newData;
            });
            setInvoiceData((prev) => {
              const newData = [...prev];
              const client = clientData[foundIndex];
              if (client) {
                newData[1] = client[0];
                newData[3] = client[9];
              } else {
                console.warn("clientData[foundIndex] is undefined");
              }
              return newData;
            });
          }
        }}
      />

<datalist id="datalistOptions">
  {clientData.map((client, index) => {
    const value = client ? `${client[1] || ""} ${client[2] || ""}`.trim() : "";
    return <option key={index} value={value} />;
  })}
</datalist>
      <div className='invoiceBorder'>
        <div className="invoiceContainer">
          <label className='title'>Invoice #</label>
          <input
            type="text"
            name={"InvoiceNum"}
            value={invoiceData[2]}
            onChange={(e) =>
              setInvoiceData((prev) => {
                const newData = [...prev];
                newData[2] = e.target.value;
                return newData;
              })
            }
          />
          <img src="/favicon.ico" className='invoiceLogo' alt={"Acres by Isaac logo"} />
          <section className="section">
            <h2 className="companyName">Acres By Isaac</h2>
            <p>156 NE 193rd St.</p>
            <p>Shoreline, WA 98155</p>
            <p>(206) 595-5831</p>
          </section>

          <section className="section">
            <h3 className="heading">Invoice Details</h3>
            <p><strong>Date of Completion: </strong><DatePicker
                selected={
                  invoiceData[4]
                    ? adjustForTimezone(new Date(invoiceData[4]))
                    : null
                }
                onChange={(date) =>
                  setInvoiceData((prev) => {
                    const newData = [...prev];   // Copy the array
                    newData[4] = date.toUTCString(); // Update index 2
                    return newData;              // Return the new array
                  })
                }
                dateFormat="yyyy-MM-dd"/></p>
            <p><strong>Due Date: </strong><DatePicker
                selected={
                  invoiceData[5]
                    ? adjustForTimezone(new Date(invoiceData[5]))
                    : null
                }
                onChange={(date) =>
                  setInvoiceData((prev) => {
                    const newData = [...prev];   // Copy the array
                    newData[5] = date.toUTCString(); // Update index 2
                    return newData;              // Return the new array
                  })
                }
                dateFormat="yyyy-MM-dd"
              />
            </p>
          </section>
          <section className="section">
            <h3 className="heading">Bill To</h3>
            <p>
            {clientData[tempData[1]]
                    ? (clientData[tempData[1]][1] || "")
                    : ""
                  } {clientData[tempData[1]]
                    ? (clientData[tempData[1]][2] || "")
                    : ""
                  }
            </p>
            <p>{locationData[tempData[2]]
                    ? (locationData[tempData[2]][1] || "")
                    : ""
                  }</p>
            <p>
            {locationData[tempData[2]]
                    ? (locationData[tempData[2]][2] || "")
                    : ""
                  }, WA, {locationData[tempData[2]]
                    ? (locationData[tempData[2]][3] || "")
                    : ""
                  }
            </p>
          </section>

          <section className="section">
            <h3 className="heading">Items</h3>
            <table className="invoiceTable">
              <thead>
                <tr>
                  <th className="tableHeader">Description</th>
                  <th className="tableHeader">Price</th>
                </tr>
              </thead>
              <tbody>
                {invoiceItems.map((item, index, slicedArray) => {
                  if (index === 0) {
                    return (
                      <tr key={index}>
                        <td className="leftAlign"><input
                          type="text"
                          name={"description"}
                          value={invoiceItems[index][0]}
                          onChange={(e) => {
                            setInvoiceItems((prev) => {
                              const newItems = [...prev];
                              newItems[index] = [...newItems[index]];
                              newItems[index][0] = e.target.value;
                              return newItems;
                            });
                          }}
                        /></td>
                        <td className="rightAlign">$<input
                          type="text"
                          name={"price"}
                          value={invoiceItems[index][1]}
                          onChange={(e) => {
                            const numericValue = e.target.value;
                            setInvoiceItems((prev) => {
                              const newItems = [...prev];
                              newItems[index] = [...newItems[index]];
                              newItems[index][1] = numericValue;
                              return newItems;
                            });
                          }}
                        /></td>
                      </tr>
                    );
                  }
                  return null;
                })}
              </tbody>
            </table>
            <button className={"button"} onClick={addItem} style={{ marginTop: '20px' }}>
                Add Item
            </button>
          </section>

          <section className="section">
            <div className="totals">
              <p>
                <strong>Subtotal:</strong> ${invoiceData[7]}
              </p>
              <p>
                <strong>
                  Sales Tax ({tempData[0]}%):
                </strong>{' '}
                ${invoiceData[8]}
              </p>
              <p className='total'>
                ${invoiceData[9]}
              </p>
              {invoiceData[6] !== 0 ? (<p></p>):(<p><strong>Tips:</strong> ${invoiceData[6]}</p>)}
            </div>
          </section>

          <section className="section">
            <h3 className="heading">Payment Methods</h3>
            <ul>
              <li>Zelle (acres.by.isaac@gmail.com, Neat Nature LLC)</li>
              <li>Debit/Credit Card accepted, text (206) 595-5831 to get started, (3% surcharge will apply)</li>
              <li>Cash or Check</li>
            </ul>
          </section>

          <section className="section">
            <p className="note">
              Note: A late fee of 20% of the subtotal may apply if payment is not
              received by the due date.
            </p>
          </section>
        </div>
      </div>
      <button className={"button"} onClick={saveInvoice} style={{ marginTop: '20px' }}>Save</button>
    </div>
  );
};

export default InvoiceNew;