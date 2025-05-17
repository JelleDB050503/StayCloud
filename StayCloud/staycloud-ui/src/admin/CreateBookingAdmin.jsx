import React, { useState } from "react";
import api from "../api/AxiosClient";

function CreateBookingAdmin() {
  const [form, setForm] = useState({
    accommodationType: "caravan-1",
    stayType: "weekend",
    season: "laagseizoen",
    weeks: 1,
    numGuests: 1,
    numDogs: 0,
    guestName: "",
    email: "",
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const res = await api.post("/api/booking/create", form);
      alert(
        `Boeking succesvol! Bevestigingscode: ${res.data.confirmationCode}`
      );
      // eventueel resetten:
      // setForm({ ... });
    } catch (err) {
      console.error("Fout bij aanmaken boeking:", err);
      alert("Boeking aanmaken mislukt.");
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h2>Admin - Boeking Aanmaken</h2>
      <label>
        Naam:
        <input
          type="text"
          required
          value={form.guestName}
          onChange={(e) => setForm({ ...form, guestName: e.target.value })}
        />
      </label>
      <br />
      <label>
        E-mail:
        <input
          type="email"
          required
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
          min={1}
          max={52}
          value={form.weeks}
          onChange={(e) => setForm({ ...form, weeks: Number(e.target.value) })}
        />
      </label>
      <br />
      <label>
        Aantal gasten:
        <input
          type="number"
          min={1}
          max={4}
          value={form.numGuests}
          onChange={(e) =>
            setForm({ ...form, numGuests: Number(e.target.value) })
          }
        />
      </label>
      <br />
      <label>
        Aantal honden:
        <input
          type="number"
          min={0}
          max={2}
          value={form.numDogs}
          onChange={(e) =>
            setForm({ ...form, numDogs: Number(e.target.value) })
          }
        />
      </label>
      <br />
      <button type="submit">Verzenden</button>
    </form>
  );
}

export default CreateBookingAdmin;
