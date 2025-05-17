import React, { useState } from "react";
import api from "../api/AxiosClient";

function SearchBooking() {
  const [query, setQuery] = useState("");
  const [form, setForm] = useState(null);
  const [message, setMessage] = useState("");
  const [updating, setUpdating] = useState(false);

  const handleSearch = async () => {
    try {
      const res = await api.get(`/api/booking/search?name=${query}`);
      const booking = res.data[0];
      setForm({
        ...booking,
        weeks: booking?.weeks || 1,
        numGuests: booking?.numGuests || 1,
        numDogs: booking?.numDogs || 0,
      });
      setMessage("");
    } catch (err) {
      setForm(null);
      setMessage(`Geen boeking gevonden voor naam "${query}".`);
    }
  };

  const handleUpdate = async () => {
    setUpdating(true);
    try {
      await api.put(`/api/booking/update/${form.id}`, {
        guestName: form.guestname,
        email: form.email,
        accommodationType: form.accommodationType,
        stayType: form.stayType,
        weeks: form.weeks,
        season: form.season,
        numGuests: form.numGuests,
        numDogs: form.numDogs,
      });
      alert("Boeking succesvol bijgewerkt. Moet opnieuw worden goedgekeurd.");
      setForm(null);
    } catch (err) {
      alert("Fout bij updaten boeking.");
    } finally {
      setUpdating(false);
    }
  };

  return (
    <div>
      <h2>Boeking Zoeken</h2>
      <input
        placeholder="Zoek op naam"
        value={query}
        onChange={(e) => setQuery(e.target.value)}
      />
      <button onClick={handleSearch}>Zoeken</button>

      {message && <p>{message}</p>}

      {form && (
        <div>
          <h3>Bewerk Boeking</h3>
          <label>
            Naam:
            <input
              value={form.guestname}
              onChange={(e) => setForm({ ...form, guestname: e.target.value })}
            />
          </label>
          <br />
          <label>
            E-mail:
            <input
              value={form.email}
              onChange={(e) => setForm({ ...form, email: e.target.value })}
            />
          </label>
          <br />
          <label>
            Accommodatie:
            <select
              value={form.accommodationType}
              onChange={(e) =>
                setForm({ ...form, accommodationType: e.target.value })
              }
            >
              <option value="caravan-1">Caravan 1</option>
              <option value="caravan-2">Caravan 2</option>
              <option value="chalet">Chalet</option>
            </select>
          </label>
          <br />
          <label>
            Verblijfstype:
            <select
              value={form.stayType}
              onChange={(e) => setForm({ ...form, stayType: e.target.value })}
            >
              <option value="weekend">Weekend</option>
              <option value="midweek">Midweek</option>
              <option value="week">Week</option>
            </select>
          </label>
          <br />
          <label>
            Seizoen:
            <select
              value={form.season}
              onChange={(e) => setForm({ ...form, season: e.target.value })}
            >
              <option value="laagseizoen">Laagseizoen</option>
              <option value="pasen">Pasen</option>
              <option value="tussenseizoen">Tussenseizoen</option>
              <option value="hoogseizoen">Hoogseizoen</option>
              <option value="herfst">Herfst</option>
              <option value="kerst">Kerst</option>
              <option value="nieuwjaar">Nieuwjaar</option>
            </select>
          </label>
          <br />
          <label>
            Aantal weken:
            <input
              type="number"
              value={form.weeks}
              onChange={(e) =>
                setForm({ ...form, weeks: Number(e.target.value) })
              }
            />
          </label>
          <br />
          <label>
            Gasten:
            <input
              type="number"
              value={form.numGuests}
              onChange={(e) =>
                setForm({ ...form, numGuests: Number(e.target.value) })
              }
            />
          </label>
          <br />
          <label>
            Honden:
            <input
              type="number"
              value={form.numDogs}
              onChange={(e) =>
                setForm({ ...form, numDogs: Number(e.target.value) })
              }
            />
          </label>
          <br />
          <button onClick={handleUpdate} disabled={updating}>
            {updating ? "Bezig..." : "Boeking bijwerken"}
          </button>
        </div>
      )}
    </div>
  );
}

export default SearchBooking;
