import React, { useEffect, useState } from "react";
import api from "../api/AxiosClient";

function ApproveBookings() {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const fetchBookings = async () => {
    try {
      const res = await api.get("/api/booking/all");
      const unapproved = res.data.filter((b) => !b.isApproved);
      setBookings(unapproved);
    } catch (err) {
      console.error("Fout bij ophalen boekingen:", err);
      setError("Kon boekingen niet laden.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchBookings();
  }, []);

  const handleApprove = async (id) => {
    try {
      await api.post(`/api/booking/approve/${id}`);
      await fetchBookings(); // refresh list
    } catch (err) {
      console.error("Fout bij goedkeuren:", err);
      alert("Goedkeuren mislukt.");
    }
  };

  if (loading) return <p>Boekingen laden...</p>;
  if (error) return <p style={{ color: "red" }}>{error}</p>;

  return (
    <div>
      <h2>Te Goedgekeuren Boekingen</h2>
      {bookings.length === 0 ? (
        <p>Er zijn geen boekingen die wachten op goedkeuring.</p>
      ) : (
        <table border="1" cellPadding="5">
          <thead>
            <tr>
              <th>Naam</th>
              <th>Email</th>
              <th>Accommodatie</th>
              <th>Verblijf</th>
              <th>Seizoen</th>
              <th>Nachten</th>
              <th>Prijs</th>
              <th>Bevestiging</th>
              <th>Actie</th>
            </tr>
          </thead>
          <tbody>
            {bookings.map((b) => (
              <tr key={b.id}>
                <td>{b.guestname}</td>
                <td>{b.email}</td>
                <td>{b.accommodationType}</td>
                <td>{b.stayType}</td>
                <td>{b.season}</td>
                <td>{b.totalNights}</td>
                <td>{b.totalPrice}</td>
                <td>{b.confirmationCode}</td>
                <td>
                  <button onClick={() => handleApprove(b.id)}>
                    ✅ Goedkeuren
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default ApproveBookings;
