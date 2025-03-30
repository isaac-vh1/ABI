import React, { useState, useEffect, useRef } from 'react';
import './InvoicePage.css';
import { useLocation } from 'react-router-dom';
import { auth } from '../firebase';
import html2canvas from 'html2canvas';
import jsPDF from 'jspdf';

const InvoicePage = () => {
  const [invoiceData, setInvoiceData] = useState(null);
  const location = useLocation();
  const invoiceNum = location.hash.slice(1);
  const user = auth.currentUser;
  const invoiceRef = useRef(null);

  useEffect(() => {
    setInvoiceData(null);
    if (!user) return;
    user.getIdToken().then(token => {
      fetch('https://www.pi.acresbyisaac.com/api/invoice/' + invoiceNum, {
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
        setInvoiceData(dataAPI);
      })
      .catch(error => console.error('Error fetching Data:', error));
    });
  }, [invoiceNum, user]);

  const handleDownloadPdf = async () => {
    if (!invoiceRef.current) return;

    try {
      // 1. Capture the invoice container with html2canvas
      const canvas = await html2canvas(invoiceRef.current, { scale: 2 });
      const imageData = canvas.toDataURL('image/png');

      // 2. Initialize jsPDF (portrait, mm units, A4 page size)
      const pdf = new jsPDF('p', 'mm', 'a4');
      const pdfWidth = pdf.internal.pageSize.getWidth();
      const pdfHeight = pdf.internal.pageSize.getHeight();

      // 3. Get the original width/height of the canvas image
      const imgProps = pdf.getImageProperties(imageData);
      const originalWidth = imgProps.width;
      const originalHeight = imgProps.height;

      // 4. Scale the image to fit the PDF width, preserving aspect ratio
      let renderedWidth = pdfWidth; // fill full width
      let renderedHeight = (originalHeight * pdfWidth) / originalWidth;

      // 5. If the scaled height is too long for one page, scale to fit height instead
      if (renderedHeight > pdfHeight) {
        renderedHeight = pdfHeight;
        renderedWidth = (originalWidth * pdfHeight) / originalHeight;
      }

      // 6. Add the image to the PDF at (x=0, y=0)
      pdf.addImage(imageData, 'PNG', 0, 0, renderedWidth, renderedHeight);

      // 7. Save (download) the PDF with the invoice number in filename
      pdf.save(`invoice-${invoiceNum}.pdf`);
    } catch (err) {
      console.error('Failed to download PDF', err);
    }
  };

  // Error handling
  if (
    !invoiceData ||
    invoiceData === 'Invoice not found' ||
    invoiceData === 'All Data not found'
  ) {
    return <h1>Error Retrieving Invoice, Please try again</h1>;
  }

  return (
    <div>
      {/* The "invoiceRef" captures everything below for the PDF */}
      <div className="container" ref={invoiceRef}>
        <h1 className="title">Invoice #{invoiceNum}</h1>

        <section className="section">
          <h2 className="companyName">Acres By Isaac</h2>
          <p>156 NE 193rd St.</p>
          <p>Shoreline, WA 98155</p>
          <p>(206) 595-5831</p>
        </section>

        <section className="section">
          <h3 className="heading">Invoice Details</h3>
          <p>
            <strong>Date of Completion:</strong> {invoiceData[0]}
          </p>
          <p>
            <strong>Due Date:</strong> {invoiceData[1]}
          </p>
        </section>

        <section className="section">
          <h3 className="heading">Billed To</h3>
          <p>
            {invoiceData[9]} {invoiceData[10]}
          </p>
          <p>{invoiceData[11]}</p>
          <p>
            {invoiceData[12]}, WA {invoiceData[13]}
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
              {invoiceData.slice(14).map((item, index, slicedArray) => {
                if (index % 2 === 0) {
                  const description = slicedArray[index];
                  const price = slicedArray[index + 1];
                  return (
                    <tr key={index}>
                      <td className="leftAlign">{description}</td>
                      <td className="rightAlign">{price}</td>
                    </tr>
                  );
                }
                return null;
              })}
            </tbody>
          </table>
        </section>

        <section className="section">
          <div className="totals">
            <p>
              <strong>Subtotal:</strong> ${invoiceData[2]}
            </p>
            <p>
              <strong>
                Sales Tax (
                {(parseFloat(invoiceData[3]) / parseFloat(invoiceData[2])).toFixed(2)}
                ):
              </strong>{' '}
              ${invoiceData[3]}
            </p>
            <p>
              <strong>Total:</strong>{' '}
              {parseFloat(invoiceData[2]) + parseFloat(invoiceData[3])}
            </p>
            <p>
              <strong>Balance Due:</strong> ${invoiceData[4]}
            </p>
            {invoiceData[5] !== 0 ? (
              <p>
                <strong>Tips:</strong> ${invoiceData[5]}
              </p>
            ) : (
              <p></p>
            )}
          </div>
        </section>

        <section className="section">
          <h3 className="heading">Payment Methods</h3>
          <ul>
            <li>Apple Pay (206) 595-5831</li>
            <li>Venmo (isaac_vh)</li>
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

      <button onClick={handleDownloadPdf} style={{ marginTop: '20px' }}>
        Download PDF of This Page
      </button>
    </div>
  );
};

export default InvoicePage;