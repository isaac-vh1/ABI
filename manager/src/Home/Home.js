import React, { useState, useEffect } from 'react';
import './home.css';
import HeaderBar from '../Components/HeaderBar';
import {auth} from '../firebase';

function Home({toggleSidebar, collapsed}) {
  const [update, setUpdate] = useState(true);
  const [quarterNumbers, setQuarterNumbers] = useState([]);
  const [error, setError] = useState(false);
  const [totalRevenue, setTotalRevenue] = useState(0);
  const [totalSalesTax, setTotalSalesTax] = useState(0);
  const [totalNetRevenue, setTotalNetRevenue] = useState(0);
  const [totalExpenses, setTotalExpenses] = useState(0);
  const user = auth.currentUser;
  //income, salestax, expenses
  useEffect(() => {
    if(update === true) {
      setUpdate(false);
    } else {
      return;
    }
    if (!user) return;
    user.getIdToken().then(token => {
      fetch('/api/manager/home', {
        method: 'GET',
        headers: {
          'Authorization': 'Bearer ' + token
        }
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
      .then(dataAPI => {
        setQuarterNumbers(dataAPI)
      })
      .catch(error => {console.error('Error fetching Data:', error); setError(true)})
      .finally(() => {setUpdate(false)});
    });
  }, [user]);

  useEffect(() => {
    const totals = quarterNumbers.reduce(
      (acc, [income, salesTax, expenses]) => {
        acc.revenue  += Number(income);
        acc.salesTax += Number(salesTax);
        acc.expenses += Number(expenses);
        return acc;
      },
      { revenue: 0, salesTax: 0, expenses: 0 }
    );

    setTotalRevenue(totals.revenue);
    setTotalSalesTax(totals.salesTax);
    setTotalExpenses(totals.expenses);
    setTotalNetRevenue(totals.revenue - totals.expenses); // profit
  }, [quarterNumbers]);

  return (
    <div>
      <HeaderBar page="Home" toggleSidebar={toggleSidebar} collapsed={collapsed} />
      <section className="dashboard-grid">
        <article className="dashboard-column">
          <div className="widget">
            <h3>Total Profit</h3>
            <p>{totalNetRevenue}</p>
          </div>
        </article>
        <article className="dashboard-column">
          <div className="widget">
            <h3>Sales Tax</h3>
            <p>{totalSalesTax}</p>
          </div>
        </article>
        <article className="dashboard-column">
          <div className="widget">
            <h3>Expenses</h3>
            <p>{totalExpenses}</p>
          </div>
        </article><article className="dashboard-column">
          <div className="widget">
            <h3>Total Revenues</h3>
            <p>{totalRevenue}</p>
          </div>
        </article>
      </section>
    </div>
  );
}

export default Home;