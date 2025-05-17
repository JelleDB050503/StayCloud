import React, { useEffect, useState } from "react";
import api from "../api/AxiosClient";

function MyBookings() {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchBookings = async () => {
      try {
        const res = await api.get("/api/booking/my");
        setBookings(res.data);
      } catch (err) {
        console.error("Fout bij ophalen boekingen:", err);
        setError("Kon boekingen niet ophalen.");
      } finally {
        setLoading(false);
      }
    };

    fetchBookings();
  }, []);

  if (loading) return <p>Boekingen laden...</p>;
  if (error) return <p style={{ color: "red" }}>{error}</p>;

  return (
    <div>
      <h2>Mijn Goedgekeurde Boekingen</h2>
      {bookings.length === 0 ? (
        <p>Je hebt nog geen goedgekeurde boekingen.</p>
      ) : (
        <table border="1" cellPadding="5">
          <thead>
            <tr>
              <th>Accommodatie</th>
              <th>Verblijf</th>
              <th>Seizoen</th>
              <th>Aantal nachten</th>
              <th>Prijs (€)</th>
              <th>Bevestiging</th>
              <th>Contract</th>
            </tr>
          </thead>
          <tbody>
            {bookings.map((b) => (
              <tr key={b.id}>
                <td>{b.accommodationType}</td>
                <td>{b.stayType}</td>
                <td>{b.season}</td>
                <td>{b.totalNights}</td>
                <td>{b.totalPrice}</td>
                <td>{b.confirmationCode}</td>
                <td>
                  <a
                    href={`http://localhost:5140/api/booking/contract/${b.id}`}
                    target="_blank"
                    rel="noreferrer"
                  >
                    Download
                  </a>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default MyBookings;
