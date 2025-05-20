import React, { useState } from "react";
import api from "../api/AxiosClient";
import { Link } from "react-router-dom";

function Register() {
  const [form, setForm] = useState({ username: "", email: "", password: "" });

  //   const handleSubmit = async (e) => {
  //     e.preventDefault();
  //     try {
  //       await api.post("/auth/register", form);
  //       alert("Registratie gelukt! Log in.");
  //       window.location.href = "/login";
  //     } catch (err) {
  //       alert("Registratie mislukt. Controleer je invoer.");
  //     }
  //   };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await api.post("api/Auth/register", form);
      alert("Registratie gelukt! Log in.");
      window.location.href = "/login";
    } catch (err) {
      console.error("Fout bij registratie:", err);

      let foutmelding = "Registratie mislukt.";
      const data = err.response?.data;

      if (Array.isArray(data?.errors)) {
        foutmelding += " " + data.errors.join(", ");
      } else if (typeof data === "string") {
        foutmelding += " " + data;
      } else if (typeof data?.message === "string") {
        foutmelding += " " + data.message;
      }

      alert(foutmelding);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input
        placeholder="Gebruikersnaam"
        onChange={(e) => setForm({ ...form, username: e.target.value })}
      />
      <input
        placeholder="E-mail"
        onChange={(e) => setForm({ ...form, email: e.target.value })}
      />
      <input
        placeholder="Wachtwoord"
        type="password"
        onChange={(e) => setForm({ ...form, password: e.target.value })}
      />
      <button type="submit">Registreren</button>
      <p>
        Heb je al een account? <Link to="/login">Login hier</Link>
      </p>
    </form>
  );
}

export default Register;
