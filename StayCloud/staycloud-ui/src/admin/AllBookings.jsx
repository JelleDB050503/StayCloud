import React, { useEffect, useState } from "react";
import api from "../api/AxiosClient";

function AllBookings() {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const fetchBookings = async () => {
    try {
      const response = await api.get("api/booking/all");
      setBookings(response.data);
    } catch (err) {
      console.error("Fout bij ophalen boekingen:", err);
      setError("Kon boekingen niet ophalen.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchBookings();
  }, []);

  const handleDelete = async (id) => {
    if (window.confirm("Weet je zeker dat je deze boeking wil verwijderen?")) {
      try {
        await api.delete(`api/booking/${id}`);
        await fetchBookings();
      } catch (err) {
        console.error("Fout bij verwijderen boeking:", err);
        alert("Verwijderen mislukt.");
      }
    }
  };

  const handleDownload = async (confirmationCode) => {
    try {
      const response = await api.get(
        `api/booking/contract/${confirmationCode}`,
        {
          responseType: "blob", // belangrijk voor downloads
        }
      );

      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `${confirmationCode}.txt`);
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    } catch (err) {
      console.error("Fout bij downloaden contract:", err);
      alert("Downloaden van contract mislukt.");
    }
  };

  if (loading) return <p>Boekingen laden...</p>;
  if (error) return <p style={{ color: "red" }}>{error}</p>;

  return (
    <div>
      <h2>Alle Boekingen</h2>
      {bookings.length === 0 ? (
        <p>Geen boekingen gevonden.</p>
      ) : (
        <table border="1" cellPadding="5">
          <thead>
            <tr>
              <th>Naam</th>
              <th>Email</th>
              <th>Type</th>
              <th>Verblijf</th>
              <th>Seizoen</th>
              <th>Aantal nachten</th>
              <th>Prijs (€)</th>
              <th>Bevestiging</th>
              <th>Status</th>
              <th>Acties</th>
            </tr>
          </thead>
          <tbody>
            {bookings.map((b) => (
              <tr key={b.id}>
                <td>{b.guestname}</td>
                <td>{b.email}</td>
                <td>{b.accommodationType}</td>
                <td>{b.stayType}</td>
                <td>{b.season || "–"}</td>
                <td>{b.totalNights}</td>
                <td>{b.totalPrice}</td>
                <td>{b.confirmationCode}</td>
                <td>
                  {b.isApproved ? "✅ Goedgekeurd" : "❌ Niet goedgekeurd"}
                </td>
                <td>
                  <button onClick={() => handleDelete(b.id)}>Verwijder</button>{" "}
                  <button onClick={() => handleDownload(b.confirmationCode)}>
                    Download
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

export default AllBookings;
